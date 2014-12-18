namespace Microsoft.Practices.DataPipeline.Cars.Dispatcher.Simulator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Microsoft.Practices.DataPipeline.Cars.Messages;
    using Microsoft.Practices.DataPipeline.Logging;

    using MessageGenerator = System.Func<MessagingEntry[]>;

    public static class SimulationScenarios
    {
        private static readonly ILogger Logger = LoggerFactory.GetLogger("Simulator");

        private static readonly Dictionary<string, MessageGenerator> ScenarioMap;

        static SimulationScenarios()
        {
            ScenarioMap =
                typeof(SimulationScenarios).GetTypeInfo()
                    .DeclaredMethods
                    .Where(x => x.ReturnType == typeof(MessagingEntry[]))
                    .ToDictionary(
                        x => x.Name,
                        x => (MessageGenerator)x.CreateDelegate(typeof(MessageGenerator)));
        }

        public static MessagingEntry[] NoErrorsExpected()
        {
            return new[]
                       {
                           new MessagingEntry(MessageFactory.LocationFactory, TimeSpan.FromSeconds(1), 0.5),
                           new MessagingEntry(MessageFactory.EngineNotificationFactory, TimeSpan.FromSeconds(300), 0.1) 
                       };
        }

        public static MessagingEntry[] MessagesWithNoHandler()
        {
            return new[]
                       {
                           new MessagingEntry(MessageFactory.LocationFactory, TimeSpan.FromSeconds(1), 0.5),
                           new MessagingEntry(
                               _ => new NoHandlerMessage(), 
                               TimeSpan.FromSeconds(60))
                       };
        }

        public static MessagingEntry[] MalformedMessages()
        {
            return new[]
                       {
                           new MessagingEntry(MessageFactory.LocationFactory, TimeSpan.FromSeconds(1), 0.5),
                           new MessagingEntry(
                               _ => new ThrowsExceptionMessage(),
                               TimeSpan.FromSeconds(60))
                       };
        }

        public static MessagingEntry[] LongRunningMessages()
        {
            return new[]
                       {
                           new MessagingEntry(MessageFactory.LocationFactory, TimeSpan.FromSeconds(1), 0.5),
                           new MessagingEntry(
                               _ => new LongRunningMessage(),
                               TimeSpan.FromSeconds(10))
                       };
        }

        public static IReadOnlyList<string> AllScenarios
        {
            get { return ScenarioMap.Keys.ToList(); }
        }

        public static MessageGenerator GetScenarioByName(string scenario)
        {
            MessageGenerator generator;
            if (!ScenarioMap.TryGetValue(scenario, out generator))
            {
                var ex = new KeyNotFoundException("The specified scenario, " + scenario + ", was not recognized.");
                Logger.UnknownScenario(scenario, ex);
                throw ex;
            }

            return generator;
        }

        public static string DefaultScenario()
        {
            MessageGenerator func = NoErrorsExpected;
            return func.Method.Name;
        }
    }
}