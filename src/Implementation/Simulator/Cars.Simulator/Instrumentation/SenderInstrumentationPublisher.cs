namespace Microsoft.Practices.DataPipeline.Cars.Dispatcher.Simulator.Instrumentation
{
    using System;
    using System.Diagnostics;
    using System.Security;
    using Microsoft.Practices.DataPipeline.Logging;
    using Microsoft.Practices.DataPipeline.Instrumentation;

    internal class SenderInstrumentationPublisher : ISenderInstrumentationPublisher
    {
        private readonly PerformanceCounter _totalMessagesSentCounter;
        private readonly PerformanceCounter _totalMessagesRequestedCounter;
        private readonly PerformanceCounter _messagesSentPerSecondCounter;
        private readonly PerformanceCounter _messagesRequestedPerSecondCounter;
        private readonly PerformanceCounter _totalBytesSentCounter;
        private readonly PerformanceCounter _bytesPerSecondSentCounter;
        private readonly PerformanceCounter _averageMessageSendingTimeCounter;
        private readonly PerformanceCounter _averageMessageSendingTimeBaseCounter;

        internal SenderInstrumentationPublisher(string instanceName, SenderInstrumentationManager instrumentationManager)
        {
            _totalMessagesSentCounter = 
                instrumentationManager.TotalMessagesSentCounterDefinition.CreatePerformanceCounter(instanceName);
            _totalMessagesRequestedCounter = 
                instrumentationManager.TotalMessagesRequestedCounterDefinition.CreatePerformanceCounter(instanceName);
            _messagesSentPerSecondCounter = 
                instrumentationManager.MessagesSentPerSecondCounterDefinition.CreatePerformanceCounter(instanceName);
            _messagesRequestedPerSecondCounter = 
                instrumentationManager.MessagesRequestedPerSecondCounterDefinition.CreatePerformanceCounter(instanceName);
            _totalBytesSentCounter = 
                instrumentationManager.TotalBytesSentCounterDefinition.CreatePerformanceCounter(instanceName);
            _bytesPerSecondSentCounter = 
                instrumentationManager.BytesSentPerSecondCounterDefinition.CreatePerformanceCounter(instanceName);
            _averageMessageSendingTimeCounter = 
                instrumentationManager.AverageMessageSendingTimeCounterDefinition.CreatePerformanceCounter(instanceName);
            _averageMessageSendingTimeBaseCounter = 
                instrumentationManager.AverageMessageSendingTimeBaseCounterDefinition.CreatePerformanceCounter(instanceName);

            _totalMessagesSentCounter.RawValue = 0L;
            _totalMessagesRequestedCounter.RawValue = 0L;
            _totalBytesSentCounter.RawValue = 0L;
            _averageMessageSendingTimeCounter.RawValue = 0L;
            _averageMessageSendingTimeBaseCounter.RawValue = 0L;
        }

        public void MessageSendRequested()
        {
            _totalMessagesRequestedCounter.Increment();
            _messagesRequestedPerSecondCounter.Increment();
        }

        public void MessageSendCompleted(long length, TimeSpan elapsed)
        {
            _totalMessagesSentCounter.Increment();
            _totalBytesSentCounter.IncrementBy(length);
            _messagesSentPerSecondCounter.Increment();
            _bytesPerSecondSentCounter.IncrementBy(length);

            _averageMessageSendingTimeCounter.IncrementBy(((long)elapsed.TotalMilliseconds) / 100L);
            _averageMessageSendingTimeBaseCounter.Increment();
        }
    }
}