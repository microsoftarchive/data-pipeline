namespace Microsoft.Practices.DataPipeline.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPoisonMessageHandler
    {
        Task PublishAsync(
            FailureMode failureMode,
            ProcessingContext context,
            byte[] payload,
            IDictionary<string, string> properties,
            Exception ex = null);
    }

    public enum FailureMode
    {
        UnknownPayload,
        Error,
        Timeout
    }
}