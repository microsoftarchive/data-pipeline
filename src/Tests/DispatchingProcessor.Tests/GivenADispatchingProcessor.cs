namespace Microsoft.Practices.DataPipeline.Processor.Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Practices.DataPipeline.Logging;
    using Microsoft.Practices.DataPipeline.Processor;
    using Microsoft.Practices.DataPipeline.Processor.Instrumentation;
    using Microsoft.Practices.DataPipeline.Tests;
    using Microsoft.Practices.DataPipeline.Tests.Mocks;
    using Microsoft.ServiceBus.Messaging;
    using Microsoft.WindowsAzure.Storage;
    using Moq;
    using Xunit;

    public class GivenADispatchingProcessor
    {
        private const string EventHubName = "eventhubName";
        private const string PartitionId = "0";
        private const int MaxConcurrency = 1;

        [Fact]
        [Trait("Kind", "Unit")]
        [Trait("Running time", "Short")]
        public async Task ThrowsWhenCheckpointFailsWithOtherExceptionTypes()
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();
            LoggerFactory.Register(Mock.Of<ILogFactory>(f => f.Create(It.IsAny<string>()) == mockLogger.Object));
            var mockCircuitBreaker = new MockCircuitBreaker();
            var mockResolver = new MockMessageHandlerResolver();

            var processor = new EventProcessor(mockResolver, mockCircuitBreaker, MaxConcurrency, EventHubName, Mock.Of<IDispatcherInstrumentationPublisher>());
            Func<Task> faultedCheckpoint = () =>
            {
                throw new InvalidOperationException();
            };

            var context = MockPartitionContext.Create(PartitionId, faultedCheckpoint);
            var events = new[] { new EventData() };

            // Act & Assert
            await AssertExt.ThrowsAsync<InvalidOperationException>(() => processor.ProcessEventsAsync(context, events));
            mockLogger.Verify(l => l.Warning(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()), Times.Never);
        }
    }
}
