namespace Microsoft.Practices.DataPipeline.ColdStorage.BlobWriter
{
    public class BlockData
    {
        private byte[] _frame = null;

        private int _actualFrameLength = 0;

        public BlockData(byte[] frame, int frameLength)
        {
            Guard.ArgumentNotNull(frame, "frame");
            Guard.ArgumentGreaterOrEqualThan(0, frameLength, "frameLength");
            Guard.ArgumentLowerOrEqualThan(frame.Length, frameLength, "frameLength");

            _frame = frame;
            _actualFrameLength = frameLength;
        }

        public byte[] Frame
        {
            get { return _frame; }
        }

        public int FrameLength
        {
            get { return _actualFrameLength; }
        }
    }
}