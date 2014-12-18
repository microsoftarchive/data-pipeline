namespace Microsoft.Practices.DataPipeline.Dispatcher.Instrumentation
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
