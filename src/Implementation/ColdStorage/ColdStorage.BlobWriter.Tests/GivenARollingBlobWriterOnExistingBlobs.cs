namespace Microsoft.Practices.DataPipeline.ColdStorage.BlobWriter.Tests
{
    using System;
    using System.Configuration;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Practices.DataPipeline.ColdStorage.Instrumentation;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Moq;
    using Xunit;

    public class GivenARollingBlobWriterOnExistingBlobs : IDisposable
    {
        private const string Prefix = "prefix/";
        private const int BlocksAllowed = 6;
        private const int BlockSize = 1024 * 1024;

        private CloudStorageAccount _storageAccount;
        private string _containerName;
        private string _existingBlobContents;

        private RollingBlobWriter _sut;

        public GivenARollingBlobWriterOnExistingBlobs()
        {
            _storageAccount = TestUtilities.GetStorageAccount();
            _containerName = "testesistingblobs" + Guid.NewGuid().ToString("N").ToLowerInvariant();

            GetContainer().Create();

            var existingBlobWriter =
                new RollingBlobWriter(
                    Mock.Of<IBlobNamingStrategy>(ns => ns.GetNamePrefix() == Prefix),
                    Mock.Of<IColdStorageInstrumentationPublisher>(),
                    _storageAccount,
                    _containerName,
                    rollSizeMb: 1,
                    blocksAllowed: BlocksAllowed,
                    blockSize: BlockSize);

            // setup first blob
            existingBlobWriter.WriteAsync(
                new[] 
                {
                    TestUtilities.CreateBlockData(new string('z', 1024*1024), BlockSize),
                },
                CancellationToken.None).Wait();

            // setup a payload of 1023KB for second blob
            _existingBlobContents = new string('z', 1024 * 1024 - 1024);

            // setup second blob
            existingBlobWriter.WriteAsync(
                new[] 
                {
                    TestUtilities.CreateBlockData(_existingBlobContents, BlockSize),
                },
                CancellationToken.None).Wait();

            _sut =
                new RollingBlobWriter(
                    Mock.Of<IBlobNamingStrategy>(ns => ns.GetNamePrefix() == Prefix),
                    Mock.Of<IColdStorageInstrumentationPublisher>(),
                    _storageAccount,
                    _containerName,
                    rollSizeMb: 1,
                    blocksAllowed: BlocksAllowed,
                    blockSize: BlockSize);
        }

        [Fact]
        [Trait("Running time", "Long")]
        [Trait("Kind", "Integration")]
        public async Task WhenWritingSingleBlock_ThenBlockIsAppendedToLastBlob()
        {
            var payloadString = new string('a', 50);

            await _sut.WriteAsync(
                new[] 
                {
                    TestUtilities.CreateBlockData(payloadString, BlockSize)
                },
                CancellationToken.None);

            Assert.Equal(
                _existingBlobContents + payloadString,
                await GetContainer()
                        .GetBlockBlobReference(Prefix + "1")
                        .DownloadTextAsync(Encoding.UTF8, AccessCondition.GenerateEmptyCondition(), null, null));
        }

        [Fact]
        [Trait("Running time", "Long")]
        [Trait("Kind", "Integration")]
        public async Task WhenWritingMultipleBlocksAndBlobIsMaxedOut_ThenRollBlocksToNewBlob()
        {
            // existing blob 0 size: 1MB
            // existing blob 1 size: 1023KB
            var payloadBlock1 = new string('r', 1024);
            var payloadBlock2 = new string('t', 1024);

            await _sut.WriteAsync(
                new[] 
                {
                    TestUtilities.CreateBlockData(payloadBlock1, BlockSize),
                    TestUtilities.CreateBlockData(payloadBlock2, BlockSize)
                },
                CancellationToken.None);

            Assert.Equal(
                payloadBlock1 + payloadBlock2,
                await GetContainer()
                        .GetBlockBlobReference(Prefix + "2")
                        .DownloadTextAsync(Encoding.UTF8, AccessCondition.GenerateEmptyCondition(), null, null));
        }


        [Fact]
        [Trait("Running time", "Long")]
        [Trait("Kind", "Integration")]
        public async Task WhenWritingSingleMaxedOutBlock_ThenRollsToANewBlob()
        {
            // existing blob 0 size: 1MB
            // existing blob 1 size: 1023KB
            // 1MB payload block
            var payloadString = new string('a', 1024 * 1024);

            await _sut.WriteAsync(
                new[] 
                {
                    TestUtilities.CreateBlockData(payloadString, BlockSize)
                },
                CancellationToken.None);

            Assert.Equal(
                payloadString,
                await GetContainer()
                        .GetBlockBlobReference(Prefix + "2")
                        .DownloadTextAsync(Encoding.UTF8, AccessCondition.GenerateEmptyCondition(), null, null));
        }

        public void Dispose()
        {
            this.GetContainer().DeleteIfExists();
        }

        private CloudBlobContainer GetContainer()
        {
            var client = _storageAccount.CreateCloudBlobClient();
            return client.GetContainerReference(_containerName);
        }
    }
}
