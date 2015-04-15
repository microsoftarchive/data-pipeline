// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.Practices.DataPipeline.Logging;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    using Newtonsoft.Json;

    public static class PoisonMessageExtensions
    {
 
        private static readonly Lazy<IPoisonMessageHandler> Handler = new Lazy<IPoisonMessageHandler>(() =>
        {
            return DependencyResolverFactory.GetResolver().GetService(typeof(IPoisonMessageHandler))
                as IPoisonMessageHandler;
        }); 

        public static async Task HandlerThrewException(this ILogger log,
            string handlerName,
            ProcessingContext context,
            byte[] payload,
            IDictionary<string, string> properties,            
            Exception ex)

        {
            log.Warning(
                "The handler, {0}, has threw an exception for {1}: {2}",
                handlerName,
                context,
                ex
            );

            await Handler.Value.PublishAsync( 
                FailureMode.Error,
                context,
                payload,
                properties,       
                ex
            );
        }

        public static async Task LogHandlerTimeout(this ILogger logger,
            IMessageHandler handler,
            ProcessingContext context,
            byte[] payload,
            IDictionary<string, string> properties)
        {
            logger.Warning(
                "The handler, {0}, has exceeded the expected duration ({1}) for {2}.",
                handler.Name,
                handler.Timeout,
                context
            );

            await Handler.Value.PublishAsync(
                FailureMode.Timeout, 
                context,
                payload,
                properties);
        }
    }

    public class AzureBlobPoisonMessageHandler : IPoisonMessageHandler
    {
        private static readonly ILogger Logger = LoggerFactory.GetLogger("Dispatcher.PoisonHandler");

        private readonly CloudBlobClient _blobClient;

        private readonly string _containerName;

        private readonly CloudStorageAccount _storageAccount;

        private CloudBlobContainer _blobContainer;

        protected AzureBlobPoisonMessageHandler(CloudStorageAccount storageAccount, 
            string containerName)
        {
            _storageAccount = storageAccount;
            _containerName = containerName;
            _blobClient = _storageAccount.CreateCloudBlobClient();
        }

        public static async Task<AzureBlobPoisonMessageHandler> CreateAsync(
            CloudStorageAccount storageAccount,
            string containerName)
        {
            var handler = new AzureBlobPoisonMessageHandler(storageAccount, containerName);
            await handler.InitializeAsync();
            return handler;
        }

        public async Task PublishAsync(
            FailureMode failureMode, 
            ProcessingContext context, 
            byte[] payload,
            IDictionary<string, string> properties, 
            Exception ex = null)
        {
            var blobMetaData = new Dictionary<string, string>();

            // Construct a name for the blob that helps communicate 
            // the nature of the failure. We append a Guid in order
            // to avoid any naming collisions.
            var blobName = string.Concat(
                failureMode,
                "/",
                context.EventHubName,
                "/",
                context.PartitionId,
                "/",
                context.EventDataOffset,
                "_",
                Guid.NewGuid().ToString("N"));

            try
            {
                var blob = _blobContainer.GetBlockBlobReference(blobName);

                // Add the property dictionary as a JSON object
                var messageProperties = JsonConvert.SerializeObject(properties);
                blobMetaData.Add("messageProperties", messageProperties);

                // Upload the blob and properties
                await blob.UploadFromByteArrayAsync(payload, 0, payload.Length);
                foreach (var p in blobMetaData)
                {
                    blob.Metadata.Add(p.Key, p.Value);
                }
                await blob.SetMetadataAsync();

                // If the message has an attached exception publish as an associated blob
                if (ex != null)
                {
                    var blobEx = _blobContainer.GetBlockBlobReference(blobName + "_exception");
                    await blobEx.UploadTextAsync(ex.ToString());
                }
            }
            catch (Exception ex0)
            {
                var sb = new StringBuilder();
                foreach (var pair in blobMetaData)
                {
                    sb.AppendFormat("{0}:{1}", pair.Key, pair.Value);
                    sb.AppendLine();
                }

                Logger.Error(ex0,
                    "Could not publish poison message to blob container. Properties from message:" + sb
                    );
            }
        }

        private async Task InitializeAsync()
        {
            _blobContainer = _blobClient.GetContainerReference(_containerName);
            if (await _blobContainer.CreateIfNotExistsAsync())
            {
                Logger.Info(
                    "Created blob container {0} in account {1} for poison messages",
                    _containerName,
                    _storageAccount.BlobStorageUri);
            }
        }
    }
}