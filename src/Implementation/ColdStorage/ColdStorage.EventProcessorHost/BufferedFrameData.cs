namespace Microsoft.Practices.DataPipeline.ColdStorage
{
    using Microsoft.Practices.DataPipeline.ColdStorage.BlobWriter;
    using Microsoft.Practices.DataPipeline.ColdStorage.Common;
    using Microsoft.ServiceBus.Messaging;

    public class BufferedFrameData : BlockData
    {
        private readonly EventData _lastEventDataInFrame;

        public BufferedFrameData(byte[] frame, int actualFrameLength, EventData lastEventDataInFrame)
            : base(frame, actualFrameLength)
        {
            Guard.ArgumentNotNull(lastEventDataInFrame, "lastEventDataInFrame");

            _lastEventDataInFrame = lastEventDataInFrame;
        }

        public EventData LastEventDataInFrame
        {
            get { return _lastEventDataInFrame; }
        }
    }
}