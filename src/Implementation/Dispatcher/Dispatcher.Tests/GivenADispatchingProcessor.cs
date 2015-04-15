// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.Dispatcher.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Practices.DataPipeline.Logging;
    using Microsoft.Practices.DataPipeline.Dispatcher;
    using Microsoft.Practices.DataPipeline.Dispatcher.Instrumentation;
    using Microsoft.Practices.DataPipeline.Tests;
    using Microsoft.Practices.DataPipeline.Tests.Mocks;
    using Microsoft.ServiceBus.Messaging;
    using Microsoft.WindowsAzure.Storage;
    using Moq;
    using Xunit;

    public class GivenADispatchingProcessor : IDisposable
    {
        private const string EventHubName = "eventhubName";
        private const string PartitionId = "0";
        private const int MaxConcurrency = 1;

        private readonly AppDomain testDomain;

        public GivenADispatchingProcessor()
        {
            var name = "test-domain" + Guid.NewGuid().ToString();
            testDomain = AppDomain.CreateDomain(name, AppDomain.CurrentDomain.Evidence, AppDomain.CurrentDomain.SetupInformation);
        }

        [Fact]
        [Trait("Category", "Unit")]
        [Trait("Running time", "Short")]
        public void ThrowsWhenCheckpointFailsWithOtherExceptionTypes()
        {
            testDomain.DoCallBack(() =>
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
                AssertExt.ThrowsAsync<InvalidOperationException>(() => processor.ProcessEventsAsync(context, events)).Wait();
                mockLogger.Verify(l => l.Warning(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()), Times.Never);
            });
        }

        [Fact]
        [Trait("Category", "Unit")]
        [Trait("Running time", "Short")]
        public void DoesNotThrowWhenCheckpointFailsWithStorageExceptionTypes()
        {
            testDomain.DoCallBack(() =>
            {
                // Arrange
                var mockLogger = new Mock<ILogger>();
                LoggerFactory.Register(Mock.Of<ILogFactory>(f => f.Create(It.IsAny<string>()) == mockLogger.Object));
                var mockCircuitBreaker = new MockCircuitBreaker();
                var mockResolver = new MockMessageHandlerResolver();

                var processor = new EventProcessor(mockResolver, mockCircuitBreaker, MaxConcurrency, EventHubName, Mock.Of<IDispatcherInstrumentationPublisher>());
                Func<Task> faultedCheckpoint = () =>
                {
                    throw new StorageException();
                };

                var context = MockPartitionContext.Create(PartitionId, faultedCheckpoint);
                var events = new[] { new EventData() };

                // Act & Assert
                processor.ProcessEventsAsync(context, events).Wait();
                mockLogger.Verify(l => l.Warning(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
            });
        }

        [Fact]
        [Trait("Category", "Unit")]
        [Trait("Running time", "Short")]
        public void DoesNotThrowWhenCheckpointFailsWithLeaseLostExceptionTypes()
        {
            testDomain.DoCallBack(() =>
            {
                // Arrange
                var mockLogger = new Mock<ILogger>();
                LoggerFactory.Register(Mock.Of<ILogFactory>(f => f.Create(It.IsAny<string>()) == mockLogger.Object));
                var mockCircuitBreaker = new MockCircuitBreaker();
                var mockResolver = new MockMessageHandlerResolver();

                var processor = new EventProcessor(mockResolver, mockCircuitBreaker, MaxConcurrency, EventHubName, Mock.Of<IDispatcherInstrumentationPublisher>());
                Func<Task> faultedCheckpoint = () =>
                {
                    throw new LeaseLostException();
                };

                var context = MockPartitionContext.Create(PartitionId, faultedCheckpoint);
                var events = new[] { new EventData() };

                // Act & Assert
                processor.ProcessEventsAsync(context, events).Wait();
                mockLogger.Verify(l => l.Warning(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
            });
        }

        public void Dispose()
        {
            if (testDomain != null)
            {
                AppDomain.Unload(testDomain);
            }
        }
    }
}
