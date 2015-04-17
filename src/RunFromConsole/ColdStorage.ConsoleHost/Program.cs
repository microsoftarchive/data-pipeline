// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.ColdStorage.ConsoleHost
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Practices.DataPipeline.ColdStorage;
    using Microsoft.Practices.DataPipeline.ColdStorage.Instrumentation;
    using Microsoft.Practices.DataPipeline.Tests;
    using Microsoft.ServiceBus;
    using Microsoft.WindowsAzure.Storage;

    internal class Program
    {

        private static void Main(string[] args)
        {
            ConsoleHost.WithOptions(new Dictionary<string, Func<CancellationToken, Task>>
            {
                { "Provision Resources", ProvisionResourcesAsync },
                { "Run Cold Storage Consumer", RunAsync }
            });
        }

        private static async Task ProvisionResourcesAsync(CancellationToken token)
        {
            var configuration = Configuration.GetCurrentConfiguration();

            var nsm = NamespaceManager.CreateFromConnectionString(configuration.EventHubConnectionString);

            Console.WriteLine("EventHub name/path: {0}", configuration.EventHubName);

            Console.WriteLine("Confirming consumer group: {0}", configuration.ConsumerGroupName);
            await nsm.CreateConsumerGroupIfNotExistsAsync(
                    eventHubPath: configuration.EventHubName,
                    name: configuration.ConsumerGroupName);

            Console.WriteLine("Consumer group confirmed");

            Console.WriteLine("Confirming blob container: {0}", configuration.ContainerName);

            var storageClient = CloudStorageAccount
                .Parse(configuration.BlobWriterStorageAccount)
                .CreateCloudBlobClient();

            var container = storageClient.GetContainerReference(configuration.ContainerName);
            await container.CreateIfNotExistsAsync(token);
            Console.WriteLine("container confirmed");
        }

        private static async Task RunAsync(CancellationToken token)
        {
            var configuration = Configuration.GetCurrentConfiguration();
            ColdStorageCoordinator processor = null;
            var instrumentationPublisher =
                new ColdStorageInstrumentationManager(true, true).CreatePublisher("Console");

            processor =
                await ColdStorageCoordinator.CreateAsync(
                    "Console",
                    configuration.EventHubName,
                    configuration.ConsumerGroupName,
                    configuration.EventHubConnectionString,
                    configuration.CheckpointStorageAccount,
                    configuration.MaxBatchSize,
                    configuration.PreFetchCount,
                    configuration.ReceiveTimeout,
                    new[] { configuration.BlobWriterStorageAccount },
                    configuration.ContainerName,
                    configuration.RollSizeForBlobWriterMb,
                    configuration.BlobPrefix,
                    configuration.CircuitBreakerWarningLevel,
                    configuration.CircuitBreakerTripLevel,
                    configuration.CircuitBreakerStallInterval,
                    configuration.CircuitBreakerLogCooldownInterval,
                    instrumentationPublisher);

            Console.WriteLine("Running processor");

            try
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, token);
            }
            catch (TaskCanceledException) { /* expected cancellation */ }

            processor.Dispose();
        }
    }
}

