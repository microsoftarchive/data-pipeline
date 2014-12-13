namespace Microsoft.Practices.DataPipeline.Processor.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class MockMessageHandlerResolver : IMessageHandlerResolver
    {
        private readonly Func<IMessageHandler> _onGetHandler;

        public MockMessageHandlerResolver()
            : this(() => new MockMessageHandler())
        {
        }

        public MockMessageHandlerResolver(Func<byte[], IDictionary<string, string>, Task> onExecute)
            : this(() => new MockMessageHandler(onExecute))
        {
        }

        public MockMessageHandlerResolver(Func<IMessageHandler> onGetHandler)
        {
            _onGetHandler = onGetHandler;
        }

        public IMessageHandler GetHandler(IDictionary<string, string> headers, string messageId)
        {
            return _onGetHandler();
        }
    }
}