namespace Microsoft.Practices.DataPipeline.ColdStorage.BlobWriter.Tests
{
    using System.Configuration;
    using System.Text;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure;
    using System;

    public static class TestUtilities
    {
        public static CloudStorageAccount GetStorageAccount()
        {
            CloudStorageAccount storageAccount;

            const string ENV_VAR_NAME = "DataPipeline_BlobWriterTests_StorageAccount";

            var connectionString = Environment.GetEnvironmentVariable(ENV_VAR_NAME, EnvironmentVariableTarget.User);

            if (string.IsNullOrEmpty(connectionString)
                || !CloudStorageAccount.TryParse(connectionString, out storageAccount))
            {
                throw new ConfigurationErrorsException(
                    string.Format(
                    "Ensure the environmental variable with key '{0}' has a valid storage connection string as its value. It must be set in order to run integration tests.", ENV_VAR_NAME));
            }

            return storageAccount;
        }

        public static BlockData CreateBlockData(string payload, int blockSize)
        {
            var payloadFrame = new byte[blockSize];
            var payloadBytes = Encoding.UTF8.GetBytes(payload);
            payloadBytes.CopyTo(payloadFrame, 0L);
            return new BlockData(payloadFrame, payloadBytes.Length);
        }
    }
}