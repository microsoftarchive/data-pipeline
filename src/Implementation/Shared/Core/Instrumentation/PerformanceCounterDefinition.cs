namespace Microsoft.Practices.DataPipeline.Instrumentation
{
    using System.Diagnostics;

    public class PerformanceCounterDefinition
    {
        private string _categoryName;
        private string _counterName;
        private string _counterHelp;
        private PerformanceCounterType _counterType;

        internal PerformanceCounterDefinition(string categoryName, string counterName, string counterHelp, PerformanceCounterType counterType)
        {
            _categoryName = categoryName;
            _counterName = counterName;
            _counterHelp = counterHelp;
            _counterType = counterType;
        }

        public PerformanceCounter CreatePerformanceCounter(string instanceName)
        {
            return new PerformanceCounter(_categoryName, _counterName, instanceName, false);
        }

        internal CounterCreationData GetCreationData()
        {
            return new CounterCreationData(_counterName, _counterHelp, _counterType);
        }
    }
}
