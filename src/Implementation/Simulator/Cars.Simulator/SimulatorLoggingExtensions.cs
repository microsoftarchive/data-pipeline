// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.Cars.Dispatcher.Simulator
{
    using System;

    using Microsoft.Practices.DataPipeline.Logging;

    public static class SimulatorLoggingExtensions
    {
        public static void SimulationStarted(this ILogger logger, string hostName, string scenario)
        {
            logger.Info("Started simulation on host {0} running scenario {1}", hostName, scenario);
        }

        public static void SimulationEnded(this ILogger logger, string hostName)
        {
            logger.Info("Ended simulation on host {0}", hostName);
        }

        public static void WarmingUpFor(this ILogger logger, string deviceId, TimeSpan waitBeforeStarting)
        {
            logger.Debug("Waiting to start {0} for {1}", deviceId, waitBeforeStarting);
        }

        public static void UnableToSend(this ILogger logger, Exception e, string partitionKey, object msg)
        {
            logger.Error(e, "An error occurred when sending with parition key {0} for event {1}", partitionKey, msg);
        }

        public static void ServiceThrottled(this ILogger logger, Exception e, string partitionKey)
        {
            logger.Error(e, 
                "Event Hub is throttled and unable to receive event with partition key {0}. " + Environment.NewLine +
                "This is likely due to to insufficient throughput units.",
                partitionKey);
        }

        public static void CarStarting(this ILogger logger, string deviceId)
        {
            logger.Info("Starting car {0}", deviceId);
        }

        public static void CarUnexpectedFailure(this ILogger logger, Exception e, string deviceId)
        {
            logger.Error(e, "An error occurred while simulating car {0}", deviceId);
        }

        public static void CarStopping(this ILogger logger, string deviceId)
        {
            logger.Info("Stopping car {0}", deviceId);
        }

        public static void WorkerRoleStartedWith(this ILogger logger, SimulatorConfiguration simulatorConfiguration)
        {
            logger.Info("Simulation started in worker role with SimulatorConfiguration: ", simulatorConfiguration);
        }

        public static void WorkerRoleStopping(this ILogger logger)
        {
            logger.Info("Worker role is stopping");
        }

        public static void WorkerRoleStopped(this ILogger logger)
        {
            logger.Info("Worker role has stopped");
        }

        public static void WorkerRoleRunning(this ILogger logger)
        {
            logger.Info("Worker role is running");
        }

        public static void WorkerRoleErrorWhileStarting(this ILogger logger, Exception e)
        {
            logger.Error(e, "Worker role encountered an error while starting up");
        }

        public static void WorkerRoleErrorWhileRunning(this ILogger logger, Exception e)
        {
            logger.Error(e, "Worker role encountered an error while running");
        }

        public static void TotalSimulationTook(this ILogger logger, TimeSpan elapsedTime)
        {
            logger.Info("The total time that the simulation ran was {0} seconds.", elapsedTime.TotalSeconds);
        }

        public static void SpinningAfterScenario(this ILogger logger)
        {
            logger.Info("The simulator is spinning after completing the scenario.");
        }

        public static void UnknownScenario(this ILogger logger, string scenario, Exception e)
        {
            logger.Error(e, "Attempt to load an unrecognized scenario: {0}", scenario);
        }
    }
}