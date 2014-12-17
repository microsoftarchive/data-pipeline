namespace Microsoft.Practices.DataPipeline.Processor
{
    using System.Collections.Generic;

    public interface IMessageHandlerResolver
    {
        IMessageHandler GetHandler(IDictionary<string, string> headers, string messageId);
    }
}