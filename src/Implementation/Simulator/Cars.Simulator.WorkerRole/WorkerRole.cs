namespace Microsoft.Practices.DataPipeline.Cars.Simulator
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Threading;

    using Microsoft.Practices.DataPipeline.Cars.Dispatcher.Simulator;
    using Microsoft.Practices.DataPipeline.Cars.Dispatcher.Simulator.Instrumentation;
    using Microsoft.Practices.DataPipeline.Logging;
    using Microsoft.WindowsAzure.ServiceRuntime;

    public class WorkerRole : RoleEntryPoint
    {
        private static readonly ILogger Logger =
            LoggerFactory.GetLogger("Simulator");

        private readonly CancellationTokenSource _cancellationTokenSource =
            new CancellationTokenSource();

        private readonly ManualResetEvent _runCompleteEvent =
            new ManualResetEvent(false);

        private readonly ManualResetEvent _spinningCompleteEvent =
            new ManualResetEvent(false);

        private SimulationProfile _simulationProfile;

        private string _scenario;

        private bool _isSpinningAfterCompletingScenario = false;

        public override bool OnStart()
        {
            try
            {
                ServicePointManager.DefaultConnectionLimit = int.MaxValue;

                // We obtain the number of instance running this role and pass it along
                // to the simulation profile. Note that we are not supporting runtime
                // changing in the number of instances. Adding new instances at runtime will
                // increase the number of cars beyond the number specified in the configuration.
                var instanceCount = RoleEnvironment.CurrentRoleInstance.Role.Instances.Count;

                // Obtain the simulation configuration and generate a simulation profile.  Set up
                // the cancellation token to terminate the simulation after the configured duration 
                var configuration = SimulatorConfiguration.GetCurrentConfiguration();

                _scenario = string.IsNullOrEmpty(configuration.Scenario)
                    ? SimulationScenarios.DefaultScenario()
                    : configuration.Scenario;

                var instrumentationPublisher =
                    new SenderInstrumentationManager(instrumentationEnabled: true, installInstrumentation: false)
                        .CreatePublisher("WaWorkerHost");

                var hostName = ConfigurationHelper.SourceName;

                _simulationProfile = new SimulationProfile(
                    hostName,
                    instanceCount,
                    instrumentationPublisher,
                    configuration);

                _cancellationTokenSource.CancelAfter(configuration.ScenarioDuration);

                Logger.WorkerRoleStartedWith(configuration);
            }
            catch (Exception ex)
            {
                // Hard error on startup, usually configuration or security related
                // Ensure that we log this error, including a direct post to the local
                // event log
                LogHelpers.HandleRoleException(Logger, "OnStart()", ex);
                throw;
            }

            return base.OnStart();
        }

        public override void Run()
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                Logger.WorkerRoleRunning();

                // Run the scenario for the specified time
                _simulationProfile
                    .RunEmulationAsync(_scenario, _cancellationTokenSource.Token)
                    .Wait();

                stopwatch.Stop();
                Logger.TotalSimulationTook(stopwatch.Elapsed);

                // After the scenario completes, we do not want
                // it to restart automatically. We block the thread
                // to keep from exiting Run(). Normally, blocking a
                // thread is a very bad thing to do.
                if (_cancellationTokenSource.IsCancellationRequested)
                {
                    Logger.SpinningAfterScenario();
                    _isSpinningAfterCompletingScenario = true;
                    _spinningCompleteEvent.WaitOne();
                }
            }
            catch (Exception e)
            {
                LogHelpers.HandleRoleException(Logger, "Run()", e);
            }
            finally
            {
                _runCompleteEvent.Set();
            }
        }

        public override void OnStop()
        {
            Logger.WorkerRoleStopping();

            // In case the scenario finished, and we were blocking 
            // to prevent the worker from restarting.
            if (_isSpinningAfterCompletingScenario)
            {
                _spinningCompleteEvent.Set();
            }
            else
            {
                // We call Cancel() in case the scenario is still running
                _cancellationTokenSource.Cancel();
                // Then we block in order for the scenario to finish running,
                // We expect it to set the reset event after it's all done.
                _runCompleteEvent.WaitOne();
            }

            base.OnStop();

            Logger.WorkerRoleStopped();
        }
    }
}