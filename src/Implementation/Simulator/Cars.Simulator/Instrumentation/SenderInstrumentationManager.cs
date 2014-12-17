namespace Microsoft.Practices.DataPipeline.Cars.Dispatcher.Simulator.Instrumentation
{
    using System;
    using System.Diagnostics;
    using Microsoft.Practices.DataPipeline.Instrumentation;
    using Microsoft.Practices.DataPipeline.Logging;

    public class SenderInstrumentationManager : InstrumentationManager
    {
        private const string SenderPerformanceCounterCategoryName = "EventHub Sender";

        private static readonly ILogger Logger = LoggerFactory.GetLogger("Simulator");
        private bool _instrumentationEnabled;

        public SenderInstrumentationManager(
            bool instrumentationEnabled = false,
            bool installInstrumentation = false)
            : base(SenderPerformanceCounterCategoryName, "", PerformanceCounterCategoryType.MultiInstance)
        {
            _instrumentationEnabled = instrumentationEnabled;

            TotalMessagesSentCounterDefinition = this.AddDefinition(
                "Total messages sent",
                "total messages sent",
                PerformanceCounterType.NumberOfItems64);
            TotalMessagesRequestedCounterDefinition = this.AddDefinition(
                "Total messages requested",
                "total messages sent",
                PerformanceCounterType.NumberOfItems64);
            MessagesSentPerSecondCounterDefinition = this.AddDefinition(
                "Messages sent per sec",
                "messages per second sent",
                PerformanceCounterType.RateOfCountsPerSecond64);
            MessagesRequestedPerSecondCounterDefinition = this.AddDefinition(
                "Messages requested per sec",
                string.Empty,
                PerformanceCounterType.RateOfCountsPerSecond64);
            TotalBytesSentCounterDefinition = this.AddDefinition(
                "total bytes sent",
                "total bytes sent",
                PerformanceCounterType.NumberOfItems64);
            BytesSentPerSecondCounterDefinition = this.AddDefinition(
                "Bytes sent per sec",
                "bytes per second sent",
                PerformanceCounterType.RateOfCountsPerSecond64);
            AverageMessageSendingTimeCounterDefinition = this.AddDefinition(
                "Avg. message sending time",
                string.Empty,
                PerformanceCounterType.RawFraction);
            AverageMessageSendingTimeBaseCounterDefinition = this.AddDefinition(
                "Avg. message sending time base",
                string.Empty,
                PerformanceCounterType.RawBase);

            if (installInstrumentation)
            {
                CreateCounters();
            }
        }

        public ISenderInstrumentationPublisher CreatePublisher(string instanceName)
        {
            if (!_instrumentationEnabled)
            {
                Logger.InstrumentationDisabled(instanceName);
                return new NullSenderInstrumentationPublisher();
            }

            try
            {
                return new SenderInstrumentationPublisher(instanceName, this);
            }
            catch (Exception ex)
            {
                Logger.InitializingPerformanceCountersFailed(instanceName, ex);
                return new NullSenderInstrumentationPublisher();
            }
        }

        protected override ILogger DerivedLogger
        {
            get { return Logger; }
        }

        internal PerformanceCounterDefinition TotalMessagesSentCounterDefinition { get; private set; }

        internal PerformanceCounterDefinition TotalMessagesRequestedCounterDefinition { get; private set; }

        internal PerformanceCounterDefinition MessagesSentPerSecondCounterDefinition { get; private set; }

        internal PerformanceCounterDefinition MessagesRequestedPerSecondCounterDefinition { get; private set; }

        internal PerformanceCounterDefinition TotalBytesSentCounterDefinition { get; private set; }

        internal PerformanceCounterDefinition BytesSentPerSecondCounterDefinition { get; private set; }

        internal PerformanceCounterDefinition AverageMessageSendingTimeCounterDefinition { get; private set; }

        internal PerformanceCounterDefinition AverageMessageSendingTimeBaseCounterDefinition { get; private set; }

        private class NullSenderInstrumentationPublisher : ISenderInstrumentationPublisher
        {
            public void MessageSendRequested()
            {
            }

            public void MessageSendCompleted(long length, TimeSpan elapsed)
            {
            }
        }
    }
}