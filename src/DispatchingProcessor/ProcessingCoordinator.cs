namespace Microsoft.Practices.DataPipeline.Processor
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Practices.DataPipeline.Logging;
    using Microsoft.Practices.DataPipeline.Processor.Instrumentation;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;

    public class ProcessingCoordinator : IDisposable
    {
        private static readonly ILogger Logger =
            LoggerFactory.GetLogger("Dispatcher.Coordinator");

        /// <summary>
        /// The event processor host we are responsible for managing
        /// </summary>
        private EventProcessorHost _host;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostName">The name of the host, most like a worker role.</param>
        /// <param name="eventHubName">The name of the event hub.</param>
        /// <param name="consumerGroupName">The name of the consumer group.</param>
        /// <param name="eventHubConnectionString">The connection string from the event hub.</param>
        /// <param name="checkpointStorageAccount">The connection string for the storage account used when checkpointing for the event hub.</param>
        /// <param name="maxBatchSize">The maximum size of the batch of events that the event processor will receive.</param>
        /// <param name="prefetchCount">The number of events that the underlying event hub client will attempt to prefetch.</param>
        /// <param name="receiveTimeout">The length of time the event processor host will wait before invoking ProcessEventsAsync when no events have been received.</param>
        /// <param name="maxConcurrencyPerProcessor">The number of events a single event processor will process concurrently.</param>
        /// <param name="typesToSearch">The collection of types to search for registered event handlers.</param>
        /// <param name="circuitBreakerFactory">Factory function for creating a the circuit breaker.</param>
        /// <param name="instrumentationPublisher">Used for custom performance counters.</param>
        /// <returns></returns>
        public async static Task<ProcessingCoordinator> CreateAsync(
            string hostName,
            string eventHubName,
            string consumerGroupName,
            string eventHubConnectionString,
            string checkpointStorageAccount,
            int maxBatchSize,
            int prefetchCount,
            TimeSpan receiveTimeout,
            int maxConcurrencyPerProcessor,
            IEnumerable<Type> typesToSearch,
            Func<string, string, ICircuitBreaker> circuitBreakerFactory,
            IDispatcherInstrumentationPublisher instrumentationPublisher)
        {
            var mp = new ProcessingCoordinator();
            await mp.InitializeAsync(
                hostName,
                eventHubName, consumerGroupName, eventHubConnectionString, 
                checkpointStorageAccount, maxBatchSize, prefetchCount, 
                receiveTimeout, maxConcurrencyPerProcessor,
                typesToSearch, circuitBreakerFactory, instrumentationPublisher);
            return mp;
        }

        protected ProcessingCoordinator()
        { }

        protected async Task InitializeAsync(
            string hostName,
            string eventHubName,
            string consumerGroupName,
            string eventHubConnectionString,
            string checkpointStorageAccount,
            int maxBatchSize,
            int prefetchCount,
            TimeSpan receiveTimeout,
            int maxConcurrencyPerProcessor,
            IEnumerable<Type> typesToSearch,
            Func<string, string, ICircuitBreaker> circuitBreakerFactory,
            IDispatcherInstrumentationPublisher instrumentationPublisher)
        {
            Logger.Info("Initializing event hub listener for {0} ({1})", eventHubName, consumerGroupName);

            // Get the consumer group via the Service Bus namespace (identifies the 
            // consumer)
            var ns = NamespaceManager.CreateFromConnectionString(eventHubConnectionString);
            try
            {
                await ns.GetConsumerGroupAsync(eventHubName, consumerGroupName);
                Logger.Info("Found consumer group {1} for {0}", eventHubName, consumerGroupName);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Could not establish connection to {0} in event hub {1}", consumerGroupName, eventHubName);
                throw;
            }

            var eventHubId = ConfigurationHelper.GetEventHubName(ns.Address, eventHubName);
          

            // Use a custom event processor factory to pass parameters to the 
            // event host processor
            var factory = new EventProcessorFactory(
                handlerResolver:    new MessageHandlerResolver(typesToSearch),
                maxConcurrency:     maxConcurrencyPerProcessor,
                circuitBreakerFactory: circuitBreakerFactory,
                eventHubName:       eventHubId,
                instrumentationPublisher: instrumentationPublisher);

            var options = new EventProcessorOptions
            {
                MaxBatchSize = maxBatchSize,
                PrefetchCount = prefetchCount,
                ReceiveTimeOut = receiveTimeout
            };
            options.ExceptionReceived += options_ExceptionReceived;

            // Create the event processor host and register via the factory            
            _host = new EventProcessorHost(
                hostName,
                consumerGroupName: consumerGroupName,
                eventHubPath: eventHubName,
                eventHubConnectionString: eventHubConnectionString,
                storageConnectionString: checkpointStorageAccount
            );
            await _host.RegisterEventProcessorFactoryAsync(factory, options);
            Logger.Info("Event processor registered for {0} ({1})", eventHubName, consumerGroupName);
        }

        /// <summary>
        /// Receive and log callback exceptions from the event host processor
        /// </summary>
        private void options_ExceptionReceived(object sender, ExceptionReceivedEventArgs e)
        {
            Logger.Error(e.Exception, "Error on message processing, action {0}", e.Action);
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