// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.ColdStorage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;

    using Microsoft.Practices.DataPipeline.ColdStorage.BlobWriter;
    using Microsoft.Practices.DataPipeline.ColdStorage.Instrumentation;
    using Microsoft.Practices.DataPipeline.Logging;
    using Microsoft.WindowsAzure.Storage;

    public class ColdStorageCoordinator : IDisposable
    {
        private static readonly ILogger Logger =
            LoggerFactory.GetLogger("ColdStorage.MessageProcessor");

        private EventProcessorHost _host;

        private ColdStorageCoordinator(EventProcessorHost host)
        {
            _host = host;
        }

        public static async Task<ColdStorageCoordinator> CreateAsync(
            string hostName,
            string eventHubName,
            string consumerGroupName,
            string eventHubConnectionString,
            string checkpointStorageAccount,
            int maxBatchSize,
            int prefetchCount,
            TimeSpan receiveTimeout,
            IReadOnlyList<string> blobWriterStorageAccounts,
            string containerName,
            int rollSizeMb,
            string blobPrefix,
            int warningLevel,
            int tripLevel,
            TimeSpan stallInterval,
            TimeSpan logCooldownInterval,
            IColdStorageInstrumentationPublisher instrumentationPublisher)
        {
            Logger.Info("Initializing event hub listener for {0} ({1})", eventHubName, consumerGroupName);

            var storageAccounts = blobWriterStorageAccounts
                .Select(CloudStorageAccount.Parse)
                .ToList();

            Func<string, IBlobWriter> blobWriterFactory =
                    partitionId =>
                        new RollingBlobWriter(new PartitionAndDateNamingStrategy(partitionId, blobPrefix),
                            instrumentationPublisher,
                            storageAccounts[Int32.Parse(partitionId) % storageAccounts.Count],
                            containerName,
                            rollSizeMb);

            var ns = NamespaceManager.CreateFromConnectionString(eventHubConnectionString);
            try
            {
                await ns.GetConsumerGroupAsync(eventHubName, consumerGroupName);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Invalid consumer group name {0} in event hub {1}", consumerGroupName, eventHubName);
                throw;
            }

            Logger.Info("Found consumer group {1} for {0}", eventHubName, consumerGroupName);

            var eventHubId = ConfigurationHelper.GetEventHubName(ns.Address, eventHubName);

            var factory = new ColdStorageEventProcessorFactory(
                blobWriterFactory,
                instrumentationPublisher,
                CancellationToken.None,
                warningLevel,
                tripLevel,
                stallInterval,
                logCooldownInterval,
                eventHubId
            );

            var options = new EventProcessorOptions()
            {
                MaxBatchSize = maxBatchSize,
                PrefetchCount = prefetchCount,
                ReceiveTimeOut = receiveTimeout,
                InvokeProcessorAfterReceiveTimeout = true
            };

            options.ExceptionReceived += 
                (s, e) => Logger.Error(
                    e.Exception, 
                    "Error on message processing, action {0}",
                    e.Action);
          
            var host = new EventProcessorHost(
                hostName,
                consumerGroupName: consumerGroupName,
                eventHubPath: eventHubName,
                eventHubConnectionString: eventHubConnectionString,
                storageConnectionString: checkpointStorageAccount);


            await host.RegisterEventProcessorFactoryAsync(factory, options);

            return new ColdStorageCoordinator(host);
        }

        public void Dispose()
        {
            if (_host != null)
            {
                _host.UnregisterEventProcessorAsync().Wait();
                _host = null;
            }
        }
    }
}
