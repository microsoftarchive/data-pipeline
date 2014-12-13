namespace Microsoft.Practices.DataPipeline.ColdStorage
{
    using System;
    using Microsoft.Practices.DataPipeline.Logging;

    internal static class ColdStorageProcessorLoggingExtensions
    {
        public static void ProcessorFlushingOnTimeout(
            this ILogger logger,
            string eventHubName,
            string partitionId,
            int blockCount)
        {
            logger.Info(
                "Processor for partition {1} in hub {0} invoked without events. Flushing {2} cached blocks.",
                eventHubName,
                partitionId,
                blockCount);
        }

        public static void CheckpointCompleted(
            this ILogger logger,
            string processorName,
            string eventHubName,
            string partitionId,
            string offset)
        {
            logger.Info(
                "Processor {0} for partition {2} on hub {1} completed a checkpoint for offset {3}.",
                processorName,
                eventHubName,
                partitionId,
                offset);
        }

        public static void UnableToCheckpoint(
            this ILogger logger,
            Exception exception,
            string processorName,
            string eventHubName,
            string partitionId)
        {
            logger.Warning(
                exception,
                "Processor {0} for partition {2} on hub {1} could not checkpoint.",
                processorName,
                eventHubName,
                partitionId);
        }

        public static void LeaseObtained(
           this ILogger logger,
           string processorName,
           string eventHubName,
           string partitionId)
        {
            logger.Info(
                "Processor {0} obtained a lease on partition {2} on hub {1}.",
                processorName,
                eventHubName,
                partitionId);
        }

        public static void LeaseLost(
           this ILogger logger,
           string processorName,
           string eventHubName,
           string partitionId)
        {
            logger.Warning(
                "Processor {0} lost a lease on partition {2} on hub {1}.",
                processorName,
                eventHubName,
                partitionId);
        }

        public static void ShutDownInitiated(
           this ILogger logger,
           string processorName,
           string eventHubName,
           string partitionId)
        {
            logger.Info(
                "Processor {0} shutting down on partition {2} on hub {1}.",
                processorName,
                eventHubName,
                partitionId);
        }
    }
}