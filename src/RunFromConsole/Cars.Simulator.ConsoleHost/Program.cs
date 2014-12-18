namespace Microsoft.Practices.DataPipeline.Cars.Simulator.ConsoleHost
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Practices.DataPipeline.Cars.Dispatcher.Simulator;
    using Microsoft.Practices.DataPipeline.Cars.Dispatcher.Simulator.Instrumentation;
    using Microsoft.Practices.DataPipeline.Tests;

    internal class Program
    {
        private static void Main(string[] args)
        {
            var configuration = SimulatorConfiguration.GetCurrentConfiguration();

            var instrumentationPublisher =
                new SenderInstrumentationManager(instrumentationEnabled: true, installInstrumentation: true)
                    .CreatePublisher("Console");

            var carEmulator = new SimulationProfile("Console", 1, instrumentationPublisher, configuration);

            var options = SimulationScenarios
                .AllScenarios
                .ToDictionary(
                    scenario => "Run " + scenario,
                    scenario => (Func<CancellationToken, Task>)(token => carEmulator.RunEmulationAsync(scenario, token)));

            // Add Single shot
            foreach (var scenario in SimulationScenarios.AllScenarios)
            {
                var name = scenario;
                options.Add(
                    "Send 1 message from " + name,
                    token => carEmulator.RunOneMessageEmulationAsync(name, token)
                    );
            }

            ConsoleHost.WithOptions(options, configuration.ScenarioDuration);
        }
    }
}
