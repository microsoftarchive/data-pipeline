namespace Microsoft.Practices.DataPipeline.Cars.Simulator.Instrumentation
{
    using System;

    public interface ISenderInstrumentationPublisher
    {
        void MessageSendRequested();
        void MessageSendCompleted(long length, TimeSpan elapsed);
    }
}