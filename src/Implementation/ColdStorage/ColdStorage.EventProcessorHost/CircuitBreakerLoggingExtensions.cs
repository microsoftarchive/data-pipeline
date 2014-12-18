namespace Microsoft.Practices.DataPipeline.ColdStorage
{
    using System;
    using Microsoft.Practices.DataPipeline.Logging;

    internal static class CircuitBreakerLoggingExtensions
    {
        public static void CircuitBreakerInitialized(
            this ILogger logger,
            string processorName,
            string partitionId,
            int warningLevel,
            int tripLevel,
            TimeSpan stallInterval,
            TimeSpan logCooldownInterval)
        {
            logger.Info(
                "Processor {0} for partition {1} circuit breaker initialized. Warning level {2}, break level {3}, stall interval {4}, log cooldown interval {5}.",
                processorName,
                partitionId,
                warningLevel,
                tripLevel,
                stallInterval,
                logCooldownInterval);
        }

        public static void CircuitBreakerClosed(this ILogger logger, string processorName, string partitionId, int currentLevel)
        {
            logger.Debug(
                "Processor {0} for partition {1} circuit closed. Current level {2}.",
                processorName,
                partitionId,
                currentLevel);
        }

        public static void CircuitBreakerWarning(this ILogger logger, string processorName, string partitionId, int warningLevel, int currentLevel)
        {
            logger.Warning(
                "Processor {0} for partition {1} over warning level {2}. Current level {3}.",
                processorName,
                partitionId,
                warningLevel,
                currentLevel);
        }

        public static void CircuitBreakerTripped(this ILogger logger, string processorName, string partitionId, int tripLevel, int currentLevel)
        {
            logger.Error(
                "Processor {0} for partition {1} over break level {2}. Circuit broken, current level {3}.",
                processorName,
                partitionId,
                tripLevel,
                currentLevel);
        }

        public static void CircuitBreakerStalling(this ILogger logger, string processorName, string partitionId, int tripLevel, int currentLevel, TimeSpan stallInterval)
        {
            logger.Debug(
                "Processor {0} for partition {1} stalling for {2}. Current level {3}.",
                processorName,
                partitionId,
                stallInterval,
                currentLevel);
        }

        public static void CircuitBreakerRestored(this ILogger logger, string processorName, string partitionId, int warningLevel, int currentLevel)
        {
            logger.Info(
                "Processor {0} for partition {1} under warning level {2}. Processing restored, current level {3}.",
                processorName,
                partitionId,
                warningLevel,
                currentLevel);
        }
    }
}