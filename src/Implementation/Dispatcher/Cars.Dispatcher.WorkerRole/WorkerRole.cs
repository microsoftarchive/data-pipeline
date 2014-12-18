namespace Microsoft.Practices.DataPipeline.Cars.Dispatcher
{
    using System;
    using System.Net;
    using System.Threading;

    using Microsoft.Practices.DataPipeline.Cars.Handlers;
    using Microsoft.WindowsAzure.ServiceRuntime;

    using Microsoft.Practices.DataPipeline;
    using Microsoft.Practices.DataPipeline.Logging;
    using Microsoft.Practices.DataPipeline.Dispatcher;
    using Microsoft.Practices.DataPipeline.Dispatcher.Instrumentation;

    public class WorkerRole : RoleEntryPoint
    {
        private static readonly ILogger Logger =
            LoggerFactory.GetLogger("Host");

        private readonly CancellationTokenSource _cancellationTokenSource =
            new CancellationTokenSource();

        private readonly ManualResetEvent _runCompleteEvent =
            new ManualResetEvent(false);

        private ProcessingCoordinator _coordinator;

        public override bool OnStart()
        {
            try
            {
                // Register the dependency resolver for initializing handlers and service
                // classes.
                var resolver = CarWorkerHostDependencyResolver.CreateAsync().Result;
                DependencyResolverFactory.Register(
                    resolver
                    );

                // Set up the process defaults for connections to optimize storage performance
                ServicePointManager.DefaultConnectionLimit = int.MaxValue;

                var configuration = DispatcherConfiguration.GetCurrentConfiguration();

                var typesToSearchForHandlers = typeof(UpdateLocationHandler)
                    .Assembly
                    .DefinedTypes;

                _coordinator = ProcessingCoordinator.CreateAsync(
                    RoleEnvironment.CurrentRoleInstance.Id,
                    configuration.EventHubName,
                    configuration.ConsumerGroupName,
                    configuration.EventHubConnectionString,
                    configuration.CheckpointStorageAccount,
                    configuration.MaxBatchSize,
                    configuration.PrefetchCount,
                    configuration.ReceiveTimeout,
                    configuration.MaxConcurrencyPerProcessor,
                    typesToSearchForHandlers,
                    (name, partitionId) =>
                        new CircuitBreaker(
                            name,
                            partitionId,
                            configuration.CircuitBreakerWarningLevel,
                            configuration.CircuitBreakerTripLevel,
                            configuration.CircuitBreakerStallInterval,
                            configuration.CircuitBreakerLogCooldownInterval),
                            new DispatcherInstrumentationManager(instrumentationEnabled: true).CreatePublisher("WaWorkerHost")).Result;

                bool result = base.OnStart();

                return result;
            }
            catch (Exception ex)
            {
                // Hard error on startup, usually configuration or security related
                // Ensure that we log this error, including a direct post to the local
                // event log
                LogHelpers.HandleRoleException(Logger, "OnStart()", ex);
                throw;
            }
        }

        public override void Run()
        {
            try
            {
                Logger.Info("Dispatching processor is running");
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
                Logger.Info("Dispatching processor is stopping");

                _cancellationTokenSource.Cancel();
                _runCompleteEvent.WaitOne();

                base.OnStop();
                Logger.Info("Dispatching processor is stopped");
            }
            catch (Exception ex)
            {
                LogHelpers.HandleRoleException(Logger, "OnStop()", ex);
                throw;
            }
        }
    }
}
