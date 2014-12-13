namespace Microsoft.Practices.DataPipeline.Cars.Simulator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Practices.DataPipeline.Cars.Simulator.Instrumentation;
    using Microsoft.Practices.DataPipeline.Logging;

    using Microsoft.ServiceBus.Messaging;

    public class SimulationProfile
    {
        private readonly SimulatorConfiguration _simulatorConfiguration;

        // The instrumentation publisher is responsible for updating 
        // performance counters and sending related telemetry events.
        private readonly ISenderInstrumentationPublisher _instrumentationPublisher;

        private readonly string _hostName;

        private static readonly ILogger Logger = LoggerFactory.GetLogger("Simulator");

        private readonly int _carsPerInstance;

        private readonly ISubject<int> _observableTotalCount = new Subject<int>();

        public SimulationProfile(
            string hostName,
            int instanceCount,
            ISenderInstrumentationPublisher instrumentationPublisher,
            SimulatorConfiguration simulatorConfiguration)
        {
            _hostName = hostName;
            _instrumentationPublisher = instrumentationPublisher;
            _simulatorConfiguration = simulatorConfiguration;

            _carsPerInstance = simulatorConfiguration.NumberOfCars / instanceCount;
        }

        public async Task RunEmulationAsync(string scenario, CancellationToken token)
        {
            Logger.SimulationStarted(_hostName, scenario);

            var produceMessagesForScenario = SimulationScenarios.GetScenarioByName(scenario);

            var emulationTasks = new List<Task>();

            var warmup = _simulatorConfiguration.WarmupDuration;
            var warmupPerCar = warmup.Ticks / _carsPerInstance;

            var messagingFactories =
                Enumerable.Range(0, _simulatorConfiguration.SenderCountPerInstance)
                    .Select(i => MessagingFactory.CreateFromConnectionString(_simulatorConfiguration.EventHubConnectionString))
                    .ToArray();

            _observableTotalCount
                .Sum()
                .Subscribe(total => Logger.Info("Final total count for all cars is {0}", total));

            _observableTotalCount
                .Buffer(TimeSpan.FromMinutes(5))
                .Scan(0, (total, next) => total + next.Sum())
                .Subscribe(total => Logger.Info("Current count for all cars is {0}", total));

            try
            {
                for (int i = 0; i < _carsPerInstance; i++)
                {
                    // Use the short form of the host or instance name to generate the vehicle ID
                    var carId = String.Format("{0}-{1}", ConfigurationHelper.InstanceName, i);

                    var messageSender = new MessageSender(
                        messagingFactory: messagingFactories[i % messagingFactories.Length],
                        config: _simulatorConfiguration,
                        serializer: Serializer.ToJsonUTF8,
                        telemetryPublisher: _instrumentationPublisher
                    );

                    var carTask = SimulateCarAsync(
                        deviceId: carId,
                        produceMessagesForScenario: produceMessagesForScenario,
                        sendMessageAsync: messageSender.SendAsync,
                        waitBeforeStarting: TimeSpan.FromTicks(warmupPerCar * i),
                        totalCount: _observableTotalCount,
                        token: token
                    );

                    emulationTasks.Add(carTask);
                }

                await Task.WhenAll(emulationTasks.ToArray());

                _observableTotalCount.OnCompleted();
            }
            finally
            {
                // cannot await on a finally block to do CloseAsync
                foreach (var factory in messagingFactories)
                {
                    factory.Close();
                }
            }

            Logger.SimulationEnded(_hostName);
        }

        private static async Task SimulateCarAsync(
            string deviceId,
            Func<MessagingEntry[]> produceMessagesForScenario,
            Func<string, object, Task<bool>> sendMessageAsync,
            TimeSpan waitBeforeStarting,
            IObserver<int> totalCount,
            CancellationToken token)
        {
            Logger.WarmingUpFor(deviceId, waitBeforeStarting);

            try
            {
                await Task.Delay(waitBeforeStarting, token);
            }
            catch (TaskCanceledException)
            {
                return;
            }

            var messagingEntries = produceMessagesForScenario();
            var car = new Car(deviceId, messagingEntries, sendMessageAsync);

            car.ObservableEventCount
                .Sum()
                .Subscribe(total => Logger.Info("final count for {0} was {1}", deviceId, total));

            car.ObservableEventCount
                .Subscribe(totalCount.OnNext);

            await car.RunSimulationAsync(token);
        }

        public async Task RunOneMessageEmulationAsync(string scenarioName, CancellationToken token)
        {
            Logger.SimulationStarted(_hostName, scenarioName);

            var produceMessagesForScenario = SimulationScenarios.GetScenarioByName(scenarioName);

            var messagingFactory = MessagingFactory
                .CreateFromConnectionString(_simulatorConfiguration.EventHubConnectionString);

            var messageSender = new MessageSender(
                        messagingFactory: messagingFactory,
                        config: _simulatorConfiguration,
                        serializer: Serializer.ToJsonUTF8,
                        telemetryPublisher: _instrumentationPublisher
                    );
            try
            {
                var scenario = produceMessagesForScenario();
                var car = new Car("Single Message Car", scenario, messageSender.SendAsync);
                await car.RunOneMessageSimulationAsync(token);
            }
            finally
            {
                messagingFactory.Close();
            }

            Logger.SimulationEnded(_hostName);
        }
    }
}