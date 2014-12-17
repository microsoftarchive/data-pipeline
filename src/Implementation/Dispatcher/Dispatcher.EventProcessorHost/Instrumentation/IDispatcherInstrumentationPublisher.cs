namespace Microsoft.Practices.DataPipeline.Processor.Instrumentation
{
    public interface IDispatcherInstrumentationPublisher
    {
        void TaskFaulted();
        void MessageProcessed();
        void TimeoutOccured();
        void TaskStarted();
        void TaskEnded();
    }
}
