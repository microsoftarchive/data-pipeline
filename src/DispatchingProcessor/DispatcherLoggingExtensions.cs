namespace Microsoft.Practices.DataPipeline.Processor
{
    using System;
    using Microsoft.Practices.DataPipeline.Logging;

    internal static class DispatcherLoggingExtensions
    {
        public static void UnableToCheckpoint(
            this ILogger logger,
            Exception exception,
            string eventHubName,
            string partitionId,
            int batchEventCount,
            TimeSpan batchElapsedTime)
        {
            logger.TraceApi(
                "dispatcher-batch",
                batchElapsedTime,
                "{0}/{1}/{2}/ERR",
                eventHubName,
                partitionId,
                batchEventCount);

            logger.Warning(
                exception,
                "Partition {1} on hub {0} could not checkpoint.",
                eventHubName,
                partitionId);
        }

        public static void BatchCompleted(
            this ILogger logger,
            string eventHubName,
            string partitionId,
            int batchEventCount,
            TimeSpan batchElapsedTime)
        {
            logger.TraceApi(
                "dispatcher-batch",
                batchElapsedTime,
                "{0}/{1}/{2}/OK",
                eventHubName,
                partitionId,
                batchEventCount);
        }

        public static void LeaseObtained(
            this ILogger logger,
            string eventHubName,
            string partitionId)
        {
            logger.Info(
                "Lease obtained on partition {1} for hub {0}.",
                eventHubName,
                partitionId);
        }

        public static void LeaseLost(
            this ILogger logger,
            string eventHubName,
            string partitionId)
        {
            logger.Warning(
               "Lease lost on partition {1} for hub {0}.",
                eventHubName,
                partitionId);
        }

        public static void ShutDownInitiated(
           this ILogger logger,
           string eventHubName,
           string partitionId)
        {
            logger.Info(
                "Shutting down on partition {1} on hub {0}.",
                eventHubName,
                partitionId);
        }
    }
}
