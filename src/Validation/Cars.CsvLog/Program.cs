namespace Microsoft.Practices.DataPipeline.Cars.CsvLog
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    using Newtonsoft.Json;

    internal class Program
    {
        private const int BoundedCapacity = 1000;

        private static void Main(string[] args)
        {
            if (args == null || args.Length != 3)
            {
                Console.WriteLine("Usage: DumpToCsv.exe [storage connection string] [container name] [output path]");
                return;
            }

            var connectionString = args[0];
            var containerName = args[1];
            var path = args[2];
            var maxDegreeOfParalellism = 50;

            try
            {
                var fileName = "carstate-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".csv";
                var filePath = Path.Combine(path, fileName);

                var sw = new Stopwatch();
                sw.Start();

                Console.WriteLine("Scanning for blobs");
                Console.WriteLine();
                Console.WriteLine("Writing data to {0}", filePath);
                ShowProgress(sw);

                var blobs = ListBlobs(connectionString, containerName);
                var lines = ConvertToCsvLines(blobs, maxDegreeOfParalellism);
                WriteLinesToFile(filePath, lines);

                sw.Stop();

                Console.WriteLine();
                Console.WriteLine("Writing complete.");
                Console.WriteLine();
                Console.WriteLine(sw.Elapsed.TotalMinutes);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
        }

        private static IEnumerable<CloudBlockBlob> ListBlobs(string connectionString, string containerName)
        {
            var account = CloudStorageAccount.Parse(connectionString);

            var client = account.CreateCloudBlobClient();
            var container = client.GetContainerReference(containerName);

            var blobs = new BlockingCollection<CloudBlockBlob>(BoundedCapacity);

            Task.Run(async () =>
            {
                BlobContinuationToken continuationToken = null;

                try
                {
                    do
                    {
                        var segment = await container.ListBlobsSegmentedAsync(continuationToken);
                        continuationToken = segment.ContinuationToken;
                        foreach (var s in segment.Results.OfType<CloudBlockBlob>())
                        {
                            blobs.Add(s);
                        }
                    }
                    while (continuationToken != null);
                }
                catch (StorageException ex)
                {
                    Console.Error.WriteLine("Error listing blobs: {0}", ex);
                    System.Environment.Exit(1);
                }

                blobs.CompleteAdding();
            });

            return blobs.GetConsumingEnumerable();
        }

        private static IEnumerable<string> ConvertToCsvLines(IEnumerable<CloudBlockBlob> blobs, int maxDegreeOfParallelism)
        {
            var lines = new BlockingCollection<string>(BoundedCapacity);

            Task.Run(async () =>
                {
                    var actionBlock = new ActionBlock<CloudBlockBlob>(
                    async (b) =>
                    {
                        var line = await ConvertToCsvLineAsync(b);
                        lines.Add(line);
                    },
                    new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism });

                    foreach (var blob in blobs)
                    {
                        var postSuccess = actionBlock.Post(blob);
                    }

                    actionBlock.Complete();
                    await actionBlock.Completion;
                    lines.CompleteAdding();
                });

            return lines.GetConsumingEnumerable();
        }

        private static async Task<string> ConvertToCsvLineAsync(CloudBlockBlob blob)
        {
            try
            {
                var blobData = await blob.DownloadTextAsync();
                var b = JsonConvert.DeserializeObject<CarState>(blobData);

                var result = string.Format("{0},{1},{2},{3},{4},{5},{6}", blob.Name, b.LastUpdatedUtc, b.Speed, b.Latitude, b.Longitude, b.Altitude, b.Heading);
                return result;
            }
            catch (StorageException ex)
            {
                Console.Error.WriteLine("Error reading blob: {0}", ex);
                System.Environment.Exit(1);
                return default(string);
            }
        }

        private static void WriteLinesToFile(string path, IEnumerable<string> lines)
        {
            using (var sw = new StreamWriter(path))
            {
                var header = "car id,last updated,speed,latitude,longitude,altitude,heading";
                sw.WriteLine(header);

                foreach (var line in lines)
                {
                    sw.WriteLine(line);
                }
            }
        }

        private static void ShowProgress(Stopwatch sw)
        {
            Task.Run(() =>
            {
                while (sw.IsRunning)
                {
                    Console.Write(".");
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                }
            });
        }
    }
}
