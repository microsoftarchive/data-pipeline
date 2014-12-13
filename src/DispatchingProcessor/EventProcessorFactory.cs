namespace Microsoft.Practices.DataPipeline.Processor
{
    using System;

    using Microsoft.ServiceBus.Messaging;
    using Microsoft.Practices.DataPipeline.Processor.Instrumentation;

    /// <summary>
    /// Factory for passing custom arguments to the event host processor
    /// </summary>
    public class EventProcessorFactory : IEventProcessorFactory
    {
        /// <summary>
        /// Interface for resolving message types/content to handler functions
        /// </summary>
        private readonly IMessageHandlerResolver _handlerResolver;

        /// <summary>
        /// The maximum number of messages that will be processed concurrently
        /// </summary>
        private readonly int _maxConcurrency;

        /// <summary>
        /// TODO
        /// </summary>
        private readonly string _eventHubName;

        /// <summary>
        /// 
        /// </summary>
        private readonly IDispatcherInstrumentationPublisher _instrumentationPublisher = null;
        /// <summary>
        /// 
        /// </summary>

        private readonly Func<string, string, ICircuitBreaker> _circuitBreakerFactory;

        public EventProcessorFactory(
            IMessageHandlerResolver handlerResolver,            
            int maxConcurrency,
            string eventHubName,
            Func<string, string, ICircuitBreaker> circuitBreakerFactory,
            IDispatcherInstrumentationPublisher instrumentationPublisher)
        {
            _handlerResolver = handlerResolver;            
            _maxConcurrency = maxConcurrency;
            _eventHubName = eventHubName;
            _circuitBreakerFactory = circuitBreakerFactory;
            _instrumentationPublisher = instrumentationPublisher;
        }

        public IEventProcessor CreateEventProcessor(PartitionContext context)
        {
            return new EventProcessor(
                _handlerResolver,                
                _circuitBreakerFactory("processor", context.Lease.PartitionId),
                _maxConcurrency,
                _eventHubName,
                _instrumentationPublisher);
        }
    }
}