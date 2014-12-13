namespace Microsoft.Practices.DataPipeline.ColdStorage.Instrumentation
{
    using System;

    public interface IColdStorageInstrumentationPublisher
    {
        void WriteAttempted();
        void WriteSucceeded(int numBlocks, TimeSpan elapsed);
        void WriteFailed();
        void ConcurrencyFailed();
        void FrameCached();
        void FrameCacheFlushed(int numFrames);
        void LeaseObtained();
        void LeaseLost();
        void EventsProcessed(int eventCount);
    }
}