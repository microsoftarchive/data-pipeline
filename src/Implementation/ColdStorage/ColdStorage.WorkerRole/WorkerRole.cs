// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.ColdStorage
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Threading;

    using Microsoft.Practices.DataPipeline.ColdStorage.Instrumentation;
    using Microsoft.Practices.DataPipeline.Logging;
    using Microsoft.WindowsAzure.ServiceRuntime;

    public class WorkerRole : RoleEntryPoint
    {
        private static readonly ILogger Logger =
            LoggerFactory.GetLogger("Host");

        private readonly CancellationTokenSource _cancellationTokenSource =
            new CancellationTokenSource();

        private readonly ManualResetEvent _runCompleteEvent =
            new ManualResetEvent(false);

        private Configuration _configuration;
        private ColdStorageCoordinator _coordinator;

        public override bool OnStart()
        {
            try
            {
                // Set the maximum number of concurrent connections
                ServicePointManager.DefaultConnectionLimit = int.MaxValue;

                // Pull the configuration object from service and app configuration
                _configuration = Configuration.GetCurrentConfiguration();

                // Create the instrumentation (performance counter publisher)
                var instrumentationPublisher =
                    new ColdStorageInstrumentationManager(
                        instrumentationEnabled: true,
                        installInstrumentation: false)
                        .CreatePublisher("WaWorkerHost");

                // Activate the cold storage processor
                _coordinator = ColdStorageCoordinator.CreateAsync(
                    RoleEnvironment.CurrentRoleInstance.Id,
                    _configuration.EventHubName,
                    _configuration.ConsumerGroupName,
                    _configuration.EventHubConnectionString,
                    _configuration.CheckpointStorageAccount,
                    _configuration.MaxBatchSize,
                    _configuration.PreFetchCount,
                    _configuration.ReceiveTimeout,
                    new[] { _configuration.BlobWriterStorageAccount },
                    _configuration.ContainerName,
                    _configuration.RollSizeForBlobWriterMb,
                    _configuration.BlobPrefix,
                    _configuration.CircuitBreakerWarningLevel,
                    _configuration.CircuitBreakerTripLevel,
                    _configuration.CircuitBreakerStallInterval,
                    _configuration.CircuitBreakerLogCooldownInterval,
                    instrumentationPublisher
                    ).Result;

                bool result = base.OnStart();

                Trace.TraceInformation("Processor has been started");

                return result;
            }
            catch (Exception ex)
            {
                LogHelpers.HandleRoleException(Logger, "Run()", ex);
                throw;
            }
        }

        public override void Run()
        {
            try
            {
                Logger.Info("Cold storage processor is running");
                _cancellationTokenSource.Token.WaitHandle.WaitOne();
                _coordinator.Dispose();
                Logger.Info("Dispatching processor is complete");
            }
            catch (Exception ex)
            {
                LogHelpers.HandleRoleException(Logger, "Run()", ex);
            }
            finally
            {
                _runCompleteEvent.Set();
            }
        }

        public override void OnStop()
        {
            try
            {
                Logger.Info("OnStop() called for Cold Storage processor");

                _cancellationTokenSource.Cancel();
                _runCompleteEvent.WaitOne();

                base.OnStop();

                Logger.Info("OnStop() complete for Cold Storage processor");
            }
            catch (Exception ex)
            {
                LogHelpers.HandleRoleException(Logger, "OnStop()", ex);
                throw;
            }
        }
    }
}
