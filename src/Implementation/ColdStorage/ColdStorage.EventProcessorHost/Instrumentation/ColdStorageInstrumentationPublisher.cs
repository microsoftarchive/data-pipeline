// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.ColdStorage.Instrumentation
{
    using System;
    using System.Diagnostics;

    internal class ColdStorageInstrumentationPublisher : IColdStorageInstrumentationPublisher
    {
        private readonly PerformanceCounter _currentCacheSizeInBlocksCounter = null;
        private readonly PerformanceCounter _totalWritesFailedCounter = null;
        private readonly PerformanceCounter _totalWritesSucceededCounter = null;
        private readonly PerformanceCounter _totalBlocksWrittenCounter = null;
        private readonly PerformanceCounter _totalConcurrentWritesFailedCounter = null;
        private readonly PerformanceCounter _totalWriteAttemptsCounter = null;
        private readonly PerformanceCounter _averageWriteTimeCounter = null;
        private readonly PerformanceCounter _averageWriteTimeBaseCounter = null;
        private readonly PerformanceCounter _totalLeasesLostCounter = null;
        private readonly PerformanceCounter _totalLeasesObtainedCounter = null;
        private readonly PerformanceCounter _totalEventsProcessedCounter = null;

        internal ColdStorageInstrumentationPublisher(
            string instanceName,
            ColdStorageInstrumentationManager instrumentationManager)
        {
            _currentCacheSizeInBlocksCounter =
                instrumentationManager.CurrentCacheSizeInBlocksCounterDefinition.CreatePerformanceCounter(instanceName);
            _totalWritesFailedCounter =
                instrumentationManager.TotalWritesFailedCounterDefinition.CreatePerformanceCounter(instanceName);
            _totalWritesSucceededCounter =
                instrumentationManager.TotalWritesSucceededDefinition.CreatePerformanceCounter(instanceName);
            _totalBlocksWrittenCounter =
                instrumentationManager.TotalBlocksWrittenCounterDefinition.CreatePerformanceCounter(instanceName);
            _totalConcurrentWritesFailedCounter =
                instrumentationManager.TotalConcurrentWritesFailedCounterDefinition.CreatePerformanceCounter(instanceName);
            _totalWriteAttemptsCounter =
                instrumentationManager.TotalWriteAttemptsCounterDefinition.CreatePerformanceCounter(instanceName);
            _averageWriteTimeCounter =
                instrumentationManager.AverageWriteTimeCounterDefinition.CreatePerformanceCounter(instanceName);
            _averageWriteTimeBaseCounter =
                instrumentationManager.AverageWriteTimeBaseCounterDefinition.CreatePerformanceCounter(instanceName);
            _totalLeasesLostCounter =
                instrumentationManager.TotalLeasesLostCounterDefinition.CreatePerformanceCounter(instanceName);
            _totalLeasesObtainedCounter =
                instrumentationManager.TotalLeasesObtainedCounterDefinition.CreatePerformanceCounter(instanceName);
            _totalEventsProcessedCounter =
                instrumentationManager.TotalEventsProcessedCounterDefinition.CreatePerformanceCounter(instanceName);

            _totalWritesFailedCounter.RawValue = 0L;
            _totalWritesSucceededCounter.RawValue = 0L;
            _totalConcurrentWritesFailedCounter.RawValue = 0L;
            _totalWriteAttemptsCounter.RawValue = 0L;
            _averageWriteTimeCounter.RawValue = 0L;
            _averageWriteTimeBaseCounter.RawValue = 0L;
            _currentCacheSizeInBlocksCounter.RawValue = 0L;
            _totalLeasesLostCounter.RawValue = 0L;
            _totalLeasesObtainedCounter.RawValue = 0L;
            _totalEventsProcessedCounter.RawValue = 0L;
        }

        public void WriteAttempted()
        {
            _totalWriteAttemptsCounter.Increment();
        }

        public void WriteSucceeded(int numBlocks, TimeSpan elapsed)
        {
            _totalWritesSucceededCounter.Increment();
            _totalBlocksWrittenCounter.IncrementBy(numBlocks);

            _averageWriteTimeCounter.IncrementBy(((long)elapsed.TotalMilliseconds) / 100L);
            _averageWriteTimeBaseCounter.Increment();
        }

        public void WriteFailed()
        {
            _totalWritesFailedCounter.Increment();
        }

        public void ConcurrencyFailed()
        {
            _totalConcurrentWritesFailedCounter.Increment();
        }

        public void FrameCached()
        {
            _currentCacheSizeInBlocksCounter.Increment();
        }

        public void FrameCacheFlushed(int count)
        {
            long delta = -1L * (long)count;
            _currentCacheSizeInBlocksCounter.IncrementBy(delta);
        }

        public void LeaseObtained()
        {
            _totalLeasesObtainedCounter.Increment();
        }

        public void LeaseLost()
        {
            _totalLeasesLostCounter.Increment();
        }

        public void EventsProcessed(int eventCount)
        {
            _totalEventsProcessedCounter.IncrementBy(eventCount);
        }
    }
}
