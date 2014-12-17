namespace Microsoft.Practices.DataPipeline.Cars.Dispatcher.Simulator.Instrumentation
{
    using System;

    public interface ISenderInstrumentationPublisher
    {
        void MessageSendRequested();
        void MessageSendCompleted(long length, TimeSpan elapsed);
    }
}