namespace Microsoft.Practices.DataPipeline.Dispatcher
{
    using System;

    public class DispatcherConfiguration
    {
        public string EventHubConnectionString { get; set; }
        public string EventHubName { get; set; }
        public string CheckpointStorageAccount { get; set; }
        public string ConsumerGroupName { get; set; }
        public int MaxBatchSize { get; set; }
        public int PrefetchCount { get; set; }
        public TimeSpan ReceiveTimeout { get; set; }
        public int MaxConcurrencyPerProcessor { get; set; }
        public string PoisonMessageStorageAccount { get; set; }
        public string PoisonMessageContainer { get; set; }
        public TimeSpan LongRunningTaskDuration { get; set; }

        // Circuit-breaker settings
        public int CircuitBreakerWarningLevel { get; set; }
        public int CircuitBreakerTripLevel { get; set; }
        public TimeSpan CircuitBreakerStallInterval { get; set; }
        public TimeSpan CircuitBreakerLogCooldownInterval { get; set; }

        public static DispatcherConfiguration GetCurrentConfiguration()
        {
            return new DispatcherConfiguration
            {
                EventHubConnectionString =
                    ConfigurationHelper.GetConfigValue<string>("Dispatcher.EventHubConnectionString"),
                EventHubName = ConfigurationHelper.GetConfigValue<string>("Dispatcher.EventHubName"),
                CheckpointStorageAccount =
                    ConfigurationHelper.GetConfigValue<string>("Dispatcher.CheckpointStorageAccount"),
                ConsumerGroupName = ConfigurationHelper.GetConfigValue<string>("Dispatcher.ConsumerGroupName"),
                MaxBatchSize = ConfigurationHelper.GetConfigValue<int>("Dispatcher.MaxBatchSize"),
                PrefetchCount = ConfigurationHelper.GetConfigValue<int>("Dispatcher.PrefetchCount"),
                ReceiveTimeout = ConfigurationHelper.GetConfigValue("Dispatcher.ReceiveTimeout",
                    TimeSpan.FromDays(7)),
                MaxConcurrencyPerProcessor =
                    ConfigurationHelper.GetConfigValue<int>("Dispatcher.MaxConcurrencyPerProcessor"),
                PoisonMessageStorageAccount = ConfigurationHelper.GetConfigValue<string>("Dispatcher.PoisonMessageStorageAccount"),
                PoisonMessageContainer = ConfigurationHelper.GetConfigValue<string>("Dispatcher.PoisonMessageContainer"),

                LongRunningTaskDuration = ConfigurationHelper.GetConfigValue(
                    "Handler.LongRunningTaskDuration",
                    TimeSpan.FromMinutes(1)),

                CircuitBreakerWarningLevel = ConfigurationHelper.GetConfigValue<int>("Dispatcher.CircuitBreaker.WarningLevel", 200),
                CircuitBreakerTripLevel = ConfigurationHelper.GetConfigValue<int>("Dispatcher.CircuitBreaker.TripLevel", 400),
                CircuitBreakerStallInterval = ConfigurationHelper.GetConfigValue<TimeSpan>("Dispatcher.CircuitBreaker.StallInterval", TimeSpan.FromSeconds(30)),
                CircuitBreakerLogCooldownInterval = ConfigurationHelper.GetConfigValue<TimeSpan>("Dispatcher.CircuitBreaker.LogCooldownInterval", TimeSpan.FromMinutes(15)),
            };
        }

        public override string ToString()
        {
            return String.Format(
                "DispatchingProcessor DispatcherConfiguration; Event hub name = {0}, Storage Account = {1}",
                EventHubName,
                CheckpointStorageAccount);
        }
    }
}