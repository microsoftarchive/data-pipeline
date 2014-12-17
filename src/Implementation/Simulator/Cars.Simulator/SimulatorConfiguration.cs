namespace Microsoft.Practices.DataPipeline.Cars.Dispatcher.Simulator
{
    using System;

    public class SimulatorConfiguration
    {
        public string Scenario { get; set; }

        public int NumberOfCars { get; set; }

        public string EventHubConnectionString { get; set; }

        public string EventHubPath { get; set; }

        public TimeSpan ScenarioDuration { get; set; }

        public int SenderCountPerInstance { get; set; }

        public TimeSpan WarmupDuration { get; set; }

        public static SimulatorConfiguration GetCurrentConfiguration()
        {
            return new SimulatorConfiguration
            {
                EventHubConnectionString = ConfigurationHelper.GetConfigValue<string>("Simulator.EventHubConnectionString"),
                EventHubPath = ConfigurationHelper.GetConfigValue<string>("Simulator.EventHubPath"),
                NumberOfCars = ConfigurationHelper.GetConfigValue<int>("Simulator.NumberOfCars"),
                ScenarioDuration = ConfigurationHelper.GetConfigValue("Simulator.ScenarioDuration", TimeSpan.FromMinutes(10)),
                SenderCountPerInstance = ConfigurationHelper.GetConfigValue("Simulator.SenderCountPerInstance", 5),
                WarmupDuration = ConfigurationHelper.GetConfigValue("Simulator.WarmupDuration", TimeSpan.FromSeconds(30)),
                Scenario = ConfigurationHelper.GetConfigValue<string>("Simulator.Scenario", String.Empty)
            };
        }

        public override string ToString()
        {
            return String.Format(
                "Simulation SimulatorConfiguration; car count = {0} event hub name = {1}, duration = {2}",
                NumberOfCars,
                EventHubPath,
                ScenarioDuration);
        }
    }
}