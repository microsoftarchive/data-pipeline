// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.Dispatcher.Tests
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Practices.DataPipeline.Dispatcher;
    using Microsoft.Practices.DataPipeline.Dispatcher.Instrumentation;
    using Microsoft.Practices.DataPipeline.Tests.Mocks;
    using Microsoft.ServiceBus.Messaging;
    using Moq;
    using Xunit;

    public class WhenProcessingMessages
    {
        public WhenProcessingMessages()
        {
            var handler = new MockPoisonMessageHandler();
            var mockResolver = MockDependencyResolver
                .CreateFor<IPoisonMessageHandler>(handler);

            DependencyResolverFactory.Register(mockResolver);
            handler.Clear();
        }

        [Fact]
        [Trait("Running time", "Short")]
        public async Task ShouldPublishToPoisonQueueWhenHandlerThrows()
        {
            var context = MockPartitionContext.CreateWithNoopCheckpoint("1");

            var attemptedToResolveHandler = false;

            var events = new[] { new EventData() };

            var resolver = new MockMessageHandlerResolver(
                async (body, headers) =>
                {
                    attemptedToResolveHandler = true;
                    await Task.Yield();
                    throw new Exception("This message was bad.");
                });

            var poisonHandler = (MockPoisonMessageHandler)DependencyResolverFactory
                .GetResolver()
                .GetService(typeof(IPoisonMessageHandler));

            var processor = new EventProcessor(resolver, new MockCircuitBreaker(), 1, "test", Mock.Of<IDispatcherInstrumentationPublisher>());

            await processor.ProcessEventsAsync(context, events);

            Assert.True(attemptedToResolveHandler);
            Assert.True(
                poisonHandler.Messages.Any(), 
                String.Format("Expected poison handler to have messages; count = {0}",
                poisonHandler.Messages.Count()));
        }

        [Fact]
        [Trait("Running time", "Short")]
        public async Task ShouldNotPublishToPoisonQueueWhenMessageIsHandled()
        {
            var handled = false;

            var context = MockPartitionContext.CreateWithNoopCheckpoint("1");

            var events = new[] { new EventData() };

            var resovler = new MockMessageHandlerResolver(
                (body, headers) =>
                {
                    handled = true;
                    return Task.FromResult<object>(null);
                });

            var poisonHandler = (MockPoisonMessageHandler)DependencyResolverFactory
                .GetResolver()
                .GetService(typeof(IPoisonMessageHandler));

            var processor = new EventProcessor(resovler, new MockCircuitBreaker(), 1, "test", Mock.Of<IDispatcherInstrumentationPublisher>());

            await processor.ProcessEventsAsync(context, events);

            Assert.True(handled);
            Assert.False(
                poisonHandler.Messages.Any(), 
                String.Format("Expected poison handler to have no messages; count = {0}",
                poisonHandler.Messages.Count()));
        }

        [Fact]
        [Trait("Running time", "Short")]
        public async Task ShouldProcessMessagesConcurrently()
        {
            const int Concurrency = 4;
            const int MessageCount = 4;
            const int MsPerMessage = 300;

            var context = MockPartitionContext.CreateWithNoopCheckpoint("1");

            var events = Enumerable
                .Range(0, MessageCount)
                .Select(id => new EventData())
                .ToArray();

            var resovler = new MockMessageHandlerResolver(
                async (body, headers) => { await Task.Delay(TimeSpan.FromMilliseconds(MsPerMessage)); });

            var processor = new EventProcessor(resovler, new MockCircuitBreaker(), Concurrency, "test", Mock.Of<IDispatcherInstrumentationPublisher>());

            var sw = Stopwatch.StartNew();
            await processor.ProcessEventsAsync(context, events);
            sw.Stop();

            Assert.True(sw.Elapsed < TimeSpan.FromMilliseconds(MsPerMessage * MessageCount));
            Assert.True(sw.Elapsed >= TimeSpan.FromMilliseconds(MsPerMessage));
        }

        [Fact]
        [Trait("Running time", "Short")]
        public async Task CanProcessMessagesSerially()
        {
            const int Concurrency = 1;
            const int MessageCount = 4;
            const int MsPerMessage = 100;

            var context = MockPartitionContext.CreateWithNoopCheckpoint("1");

            var events = Enumerable
                .Range(0, MessageCount)
                .Select(id => new EventData())
                .ToArray();

            var resovler = new MockMessageHandlerResolver(
                async (body, headers) => { await Task.Delay(TimeSpan.FromMilliseconds(MsPerMessage)); });

            var processor = new EventProcessor(resovler, new MockCircuitBreaker(), Concurrency, "test", Mock.Of<IDispatcherInstrumentationPublisher>());

            var sw = Stopwatch.StartNew();
            await processor.ProcessEventsAsync(context, events);
            sw.Stop();

            Assert.True(sw.Elapsed > TimeSpan.FromMilliseconds(MsPerMessage * MessageCount));
        }
    }
}