namespace Microsoft.Practices.DataPipeline.Cars.Simulator
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using Messages;
    using Instrumentation;
    using Logging;
    using ServiceBus.Messaging;

    public class MessageSender : IMessageSender
    {
        private static readonly ILogger Logger = LoggerFactory.GetLogger("Simulator");

        private readonly EventHubClient _eventHubClient;

        private readonly ISenderInstrumentationPublisher _instrumentationTelemetryPublisher;

        private readonly Func<object, byte[]> _serializer;

        public MessageSender(
            MessagingFactory messagingFactory,
            SimulatorConfiguration config,
            Func<object, byte[]> serializer,
            ISenderInstrumentationPublisher telemetryPublisher)
        {
            this._serializer = serializer;
            this._instrumentationTelemetryPublisher = telemetryPublisher;

            this._eventHubClient = messagingFactory.CreateEventHubClient(config.EventHubPath);
        }

        public static Tuple<string, int> DetermineTypeFromMessage(object msg)
        {
            // For the purposes of this simulation, we are defaulting
            // all type version numbers to 1.
            var type = msg.GetType().Name;
            return new Tuple<string, int>(type, 1);
        }

        public async Task<bool> SendAsync(string partitionKey, object msg)
        {
            try
            {
                var bytes = this._serializer(msg);

                using (var eventData = new EventData(bytes) { PartitionKey = partitionKey })
                {
                    var registration = DetermineTypeFromMessage(msg);
                    eventData.Properties[EventDataPropertyKeys.MessageType] = registration.Item1;
                    eventData.Properties[EventDataPropertyKeys.MessageTypeVersion] = registration.Item2;
                    eventData.Properties[EventDataPropertyKeys.DeviceId] = partitionKey;

                    var stopwatch = Stopwatch.StartNew();

                    this._instrumentationTelemetryPublisher.MessageSendRequested();

                    await this._eventHubClient.SendAsync(eventData);
                    stopwatch.Stop();

                    this._instrumentationTelemetryPublisher.MessageSendCompleted(
                        bytes.Length,
                        stopwatch.Elapsed);

                    Logger.TraceApi(
                        "EventHubClient.SendAsync",
                        stopwatch.Elapsed,
                        "OK/{0}",
                        partitionKey);

                    return true;
                }
            }
            catch (ServerBusyException e)
            {
                Logger.ServiceThrottled(e, partitionKey);
            }
            catch (Exception e)
            {
                Logger.UnableToSend(e, partitionKey, msg);
            }

            return false;
        }
    }
}