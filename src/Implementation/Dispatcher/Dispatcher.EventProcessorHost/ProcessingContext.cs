namespace Microsoft.Practices.DataPipeline.Dispatcher
{
    // This class exists simply to capture and consolidate
    // the context that a given event is being processed in.
    // The information captured here will help to correlate
    // log entries.
    public class ProcessingContext
    {
        public string EventHubName { get; private set; }

        public string PartitionId { get; private set; }

        public string EventDataOffset { get; private set; }

        public ProcessingContext(string eventHubName, string partitionId, string eventDataOffset)
        {
            EventHubName = eventHubName;
            PartitionId = partitionId;
            EventDataOffset = eventDataOffset;
        }

        public override string ToString()
        {
            return string.Concat(EventHubName, "/", PartitionId, "/", EventDataOffset);
        }
    }
}