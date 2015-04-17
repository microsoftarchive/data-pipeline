// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.ColdStorage.BlobWriter
{
    using System;

    using Microsoft.Practices.DataPipeline.Logging;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    internal static class RollingBlobWriterLoggingExtensions
    {
        public static void WriteToBlobStarted(
            this ILogger logger,
            string blobName,
            int blockListLength,
            int numberOfBlocks,
            long bytesLength)
        {
            logger.Debug(
                "Started writing to blob {0} having {1} existing blocks writing {2} new blocks with total {3} bytes",
                blobName,
                blockListLength,
                numberOfBlocks,
                bytesLength);
        }

        public static void WriteToBlobEnded(
            this ILogger logger,
            string blobName,
            int blockListLength)
        {
            logger.Debug(
                "Completed Writing to blob {0} total block length is now {1}",
                blobName,
                blockListLength);
        }

        public static void WriteToBlobFailed(
            this ILogger logger,
            Exception exp,
            string blobName,
            int numberOfBlocks,
            long bytesLength)
        {
            logger.Error(exp,
                "Failed writing to blob {0} blocks {1} bytes {2}",
                blobName,
                numberOfBlocks,
                bytesLength);
        }

        public static void RollOccured(
            this ILogger logger,
            string oldBlobName,
            string newBlobName)
        {
            logger.Info(
                "Rolling over from blob {0} to blob {1}",
                oldBlobName,
                newBlobName);
        }

        public static void BlobEtagMissMatchOccured(
            this ILogger logger,
            string blobName)
        {
            logger.Error(
                "Etag mismatch writing to blob {0}",
                blobName);
        }

        public static void HardStorageExceptionCaughtWritingToBlob(
            this ILogger logger,
            StorageException storageException,
            CloudBlobClient blobClient,
            string containerName)
        {
            logger.Error(
                storageException,
                "Error attempting storage operation on container {1} for storage account {0}: {2}",
                GetStorageAccountName(blobClient),
                containerName,
                storageException.RequestInformation.ExtendedErrorInformation.ErrorCode);
        }

        public static void StorageExceptionCaughtWritingToBlob(
            this ILogger logger,
            StorageException storageException,
            CloudBlobClient blobClient,
            string containerName)
        {
            logger.Warning(
                storageException,
                "Error attempting storage operation on container {1} for storage account {0}: {2}. Request is ignored.",
                GetStorageAccountName(blobClient),
                containerName,
                storageException.RequestInformation.ExtendedErrorInformation.ErrorCode);
        }

        private static string GetStorageAccountName(CloudBlobClient blobClient)
        {
            return blobClient.StorageUri.PrimaryUri.Authority.Split('.')[0];
        }
    }
}