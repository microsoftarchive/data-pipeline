// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.Dispatcher.Instrumentation
{
    using System;
    using System.Diagnostics;
    using Microsoft.Practices.DataPipeline.Instrumentation;
    using Microsoft.Practices.DataPipeline.Logging;

    public class DispatcherInstrumentationManager : InstrumentationManager
    {
        private const string DispatcherPerformanceCounterCategoryName = "EventHub Dispatcher";
        private static readonly ILogger Logger = LoggerFactory.GetLogger("Dispatcher.Instrumentation");
        private bool _instrumentationEnabled;

        public DispatcherInstrumentationManager(
            bool instrumentationEnabled = false,
            bool installInstrumentation = false)
            : base(DispatcherPerformanceCounterCategoryName, "", PerformanceCounterCategoryType.MultiInstance)
        {
            _instrumentationEnabled = instrumentationEnabled;

            TotalTasksFaultedCounterDefinition = this.AddDefinition(
                "Total tasks faulted",
                "Total tasks faulted",
                PerformanceCounterType.NumberOfItems64);
            TotalProcessedMessagesCounterDefinition = this.AddDefinition(
                "Total processed messages",
                "Total processed messages",
                PerformanceCounterType.NumberOfItems64);
            TotalTimeoutsCounterDefinition = this.AddDefinition(
                "Total timeouts",
                "Total timeouts",
                PerformanceCounterType.NumberOfItems64);
            CurrentTaskCountCounterDefinition = this.AddDefinition(
               "Current task count",
               "Current task count",
               PerformanceCounterType.NumberOfItems64);

            if (installInstrumentation)
            {
                CreateCounters();
            }
        }

        public IDispatcherInstrumentationPublisher CreatePublisher(string instanceName)
        {
            if (!_instrumentationEnabled)
            {
                return new NullDispatcherInstrumentationPublisher();
            }

            try
            {
                return new DispatcherInstrumentationPublisher(instanceName, this);
            }
            catch (Exception ex)
            {
                Logger.InitializingPerformanceCountersFailed(instanceName, ex);
                return new NullDispatcherInstrumentationPublisher();
            }
        }

        protected override ILogger DerivedLogger
        {
            get { return Logger; }
        }

        internal PerformanceCounterDefinition TotalTasksFaultedCounterDefinition { get; private set; }

        internal PerformanceCounterDefinition TotalProcessedMessagesCounterDefinition { get; private set; }

        internal PerformanceCounterDefinition TotalTimeoutsCounterDefinition { get; private set; }

        internal PerformanceCounterDefinition CurrentTaskCountCounterDefinition { get; private set; }

        private class NullDispatcherInstrumentationPublisher : IDispatcherInstrumentationPublisher
        {
            public void TaskFaulted()
            {
            }

            public void MessageProcessed()
            {
            }

            public void TimeoutOccured()
            {
            }

            public void TaskStarted()
            {
            }

            public void TaskEnded()
            {
            }
        }
    }
}