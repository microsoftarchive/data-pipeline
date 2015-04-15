// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.ColdStorage.Instrumentation
{
    using System;
    using System.Diagnostics;
    using Microsoft.Practices.DataPipeline.Instrumentation;
    using Microsoft.Practices.DataPipeline.Logging;

    public class ColdStorageInstrumentationManager : InstrumentationManager
    {
        private const string ColdStoragePerformanceCounterCategoryName = "EventHub ColdStorage";

        private static readonly ILogger Logger = LoggerFactory.GetLogger("ColdStorage.Instrumentation");

        private readonly bool _instrumentationEnabled;

        public ColdStorageInstrumentationManager(
            bool instrumentationEnabled = false,
            bool installInstrumentation = false)
            : base(ColdStoragePerformanceCounterCategoryName, "", PerformanceCounterCategoryType.MultiInstance)
        {
            _instrumentationEnabled = instrumentationEnabled;

            CurrentCacheSizeInBlocksCounterDefinition = this.AddDefinition(
                "Current cache size in blocks",
                "Current cache size in blocks",
                PerformanceCounterType.NumberOfItems64);
            TotalWritesSucceededDefinition = this.AddDefinition(
                "Total writes succeeded",
                "Total writes succeeded",
                PerformanceCounterType.NumberOfItems64);
            TotalBlocksWrittenCounterDefinition = this.AddDefinition(
                "Total blocks written",
                "Total blocks written",
                PerformanceCounterType.NumberOfItems64);
            TotalWritesFailedCounterDefinition = this.AddDefinition(
                "Total writes failed",
                "Total writes failed",
                PerformanceCounterType.NumberOfItems64);
            TotalLeasesLostCounterDefinition = this.AddDefinition(
                "Total leases lost",
                "Total leases lost",
                PerformanceCounterType.NumberOfItems64);
            TotalLeasesObtainedCounterDefinition = this.AddDefinition(
                "Total leases obtained",
                "Total leases obtained",
                PerformanceCounterType.NumberOfItems64);
            TotalConcurrentWritesFailedCounterDefinition = this.AddDefinition(
                "Total concurrent writes failed",
                "Total concurrent writes failed",
                PerformanceCounterType.NumberOfItems64);
            TotalWriteAttemptsCounterDefinition = this.AddDefinition(
                "Total write attempts",
                "Total write attempts",
                PerformanceCounterType.NumberOfItems64);
            AverageWriteTimeCounterDefinition = this.AddDefinition(
                "Average write time",
                "Average write time",
                PerformanceCounterType.RawFraction);
            AverageWriteTimeBaseCounterDefinition = this.AddDefinition(
                "Average write time base",
                "Average write time base",
                PerformanceCounterType.RawBase);
            TotalEventsProcessedCounterDefinition = this.AddDefinition(
                "Total events processed",
                "Total events processed",
                PerformanceCounterType.NumberOfItems64);

            if (installInstrumentation)
            {
                CreateCounters();
            }
        }

        public IColdStorageInstrumentationPublisher CreatePublisher(string instanceName)
        {
            if (!_instrumentationEnabled)
            {
                return new NullColdStorageInstrumentationPublisher();
            }

            try
            {
                return new ColdStorageInstrumentationPublisher(instanceName, this);
            }
            catch (Exception ex)
            {
                Logger.InitializingPerformanceCountersFailed(instanceName, ex);
                return new NullColdStorageInstrumentationPublisher();
            }
        }

        internal PerformanceCounterDefinition CurrentCacheSizeInBlocksCounterDefinition { get; private set; }

        internal PerformanceCounterDefinition TotalWritesFailedCounterDefinition { get; private set; }

        internal PerformanceCounterDefinition TotalWritesSucceededDefinition { get; private set; }

        internal PerformanceCounterDefinition TotalBlocksWrittenCounterDefinition { get; private set; }

        internal PerformanceCounterDefinition TotalConcurrentWritesFailedCounterDefinition { get; private set; }

        internal PerformanceCounterDefinition TotalWriteAttemptsCounterDefinition { get; private set; }

        internal PerformanceCounterDefinition AverageWriteTimeCounterDefinition { get; private set; }

        internal PerformanceCounterDefinition AverageWriteTimeBaseCounterDefinition { get; private set; }

        internal PerformanceCounterDefinition TotalLeasesLostCounterDefinition { get; private set; }

        internal PerformanceCounterDefinition TotalLeasesObtainedCounterDefinition { get; private set; }

        internal PerformanceCounterDefinition TotalEventsProcessedCounterDefinition { get; private set; }

        protected override ILogger DerivedLogger
        {
            get { return Logger; }
        }

        private class NullColdStorageInstrumentationPublisher : IColdStorageInstrumentationPublisher
        {
            public void WriteAttempted()
            {
            }

            public void WriteSucceeded(int numBlocks, TimeSpan elapsed)
            {
            }

            public void WriteFailed()
            {
            }

            public void ConcurrencyFailed()
            {
            }

            public void FrameCached()
            {
            }

            public void FrameCacheFlushed(int numFrames)
            {
            }

            public void LeaseObtained()
            {
            }

            public void LeaseLost()
            {
            }

            public void EventsProcessed(int eventCount)
            {
            }
        }
    }
}