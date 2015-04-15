// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.Dispatcher.Instrumentation
{
    using System.Diagnostics;

    internal class DispatcherInstrumentationPublisher : IDispatcherInstrumentationPublisher
    {
        private PerformanceCounter _totalTasksFaultedCounter = null;
        private PerformanceCounter _totalMessagesProcessedCounter = null;
        private PerformanceCounter _totalTimeoutsCounter = null;
        private PerformanceCounter _currentTaskCountCounter = null;

        internal DispatcherInstrumentationPublisher(string instanceName,
            DispatcherInstrumentationManager instrumentationManager)
        {
            _totalTasksFaultedCounter =
                instrumentationManager.TotalTasksFaultedCounterDefinition.CreatePerformanceCounter(instanceName);
            _totalMessagesProcessedCounter =
                instrumentationManager.TotalProcessedMessagesCounterDefinition.CreatePerformanceCounter(instanceName);
            _totalTimeoutsCounter =
                instrumentationManager.TotalTimeoutsCounterDefinition.CreatePerformanceCounter(instanceName);
            _currentTaskCountCounter =
                instrumentationManager.CurrentTaskCountCounterDefinition.CreatePerformanceCounter(instanceName);
        }

        public void TaskFaulted()
        {
            _totalTasksFaultedCounter.Increment();
        }

        public void MessageProcessed()
        {
            _totalMessagesProcessedCounter.Increment();
        }

        public void TimeoutOccured()
        {
            _totalTimeoutsCounter.Increment();
        }

        public void TaskStarted()
        {
            _currentTaskCountCounter.Increment();
        }

        public void TaskEnded()
        {
            _currentTaskCountCounter.Decrement();
        }
    }
}
