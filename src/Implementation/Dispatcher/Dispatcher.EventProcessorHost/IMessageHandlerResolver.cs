namespace Microsoft.Practices.DataPipeline.Dispatcher
{
    using System.Collections.Generic;

    public interface IMessageHandlerResolver
    {
        IMessageHandler GetHandler(IDictionary<string, string> headers, string messageId);
    }
}