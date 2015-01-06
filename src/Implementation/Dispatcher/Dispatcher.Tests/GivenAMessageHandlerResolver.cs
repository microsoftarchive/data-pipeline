namespace Microsoft.Practices.DataPipeline.Dispatcher.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Practices.DataPipeline.Cars.Messages;
    using Microsoft.Practices.DataPipeline.Dispatcher;

    using Xunit;

    public class GivenAMessageHandlerResolver
    {
        public const string UnknownMessageType = "UnknownType";
        public const string RegisteredMessageType = "TestMessage";

        [Trait("Running time", "Short")]
        [Fact]
        public void WhenResolving_ShouldUseTypeFromMessageHeaders()
        {
            var dependencies = MockDependencyResolver
                .Create()
                .Register<IPoisonMessageHandler>(null);

            var resolver = new MessageHandlerResolver(new[] { typeof(TestMessageHandler) }, dependencies);
            var headers = new Dictionary<string, string>
                              {
                                  { EventDataPropertyKeys.MessageType, RegisteredMessageType },
                                  { EventDataPropertyKeys.MessageTypeVersion, "1" }
                              };
            var handler = resolver.GetHandler(headers, "test-message-0");

            Assert.NotNull(handler);
            Assert.IsType<TestMessageHandler>(handler);
            Assert.Equal("TestMessageHandler", handler.Name);
        }


        [Trait("Running time", "Short")]
        [Fact]
        public void WhenResolvingUnknownMessageTypes_ShouldReturnDefaultHandler()
        {
            var dependencies = MockDependencyResolver
                .Create()
                .Register<IPoisonMessageHandler>(null);

            var resolver = new MessageHandlerResolver(new[] { typeof(TestMessageHandler) }, dependencies);
            var headers = new Dictionary<string, string> { { EventDataPropertyKeys.MessageType, UnknownMessageType } };
            var handler = resolver.GetHandler(headers, "test-message-0");

            Assert.IsType<UnknownTypeMessageHandler>(handler);
        }
        
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