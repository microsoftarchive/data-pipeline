namespace Microsoft.Practices.DataPipeline.Dispatcher.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Practices.DataPipeline.Dispatcher;

    using Xunit;

    public class WhenResolvingMessageHandler
    {
        public const string UnknownMessageType = "UnknownType";
        public const string RegisteredMessageType = "TestMessage";
        
        [MessageHandler(MessageType = RegisteredMessageType, Version = 1)]
        public class TestMessageHandler : IMessageHandler
        {
            public string Name
            {
                get
                {
                    return "TestMessageHandler";
                }
            }

            public TimeSpan Timeout
            {
                get { return TimeSpan.FromSeconds(1); }
            }

            public Task ExecuteAsync(
                ProcessingContext context,
                byte[] payload,
                IDictionary<string, string> properties)
            {
                throw new NotImplementedException();
            }
        }
    }
}