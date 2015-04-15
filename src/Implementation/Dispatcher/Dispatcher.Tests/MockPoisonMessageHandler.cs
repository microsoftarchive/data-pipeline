// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Threading;

namespace Microsoft.Practices.DataPipeline.Dispatcher.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Practices.DataPipeline.Dispatcher;

    public class MockPoisonMessageHandler : IPoisonMessageHandler
    {
        private Func<FailureMode, byte[], IDictionary<string, string>, Exception, Task> _onPublishAsync;

        public MockPoisonMessageHandler()
            : this((fm, p, ps, e) => Task.FromResult<object>(null))
        {

        }

        public MockPoisonMessageHandler(
            Func<FailureMode, byte[], IDictionary<string, string>, Exception, Task> onPublishAsync)
        {
            SetHandlerFunc(onPublishAsync);
        }

        public void SetHandlerFunc(Func<FailureMode, byte[], IDictionary<string, string>, Exception, Task> func)
        {
            _onPublishAsync = func;
        }

        public Task PublishAsync(
            FailureMode failureMode,
            ProcessingContext context,
            byte[] payload,
            IDictionary<string, string> properties,
            Exception ex = null)
        {
            _messages.Add(new PoisonMessage()
            {
                Mode = failureMode,
                Payload = payload,
                Properties = properties,
                ExceptionObject = ex
            });
            Console.WriteLine("Writing message to mock poison queue.. count = {0}",
                _messages.Count);
            Thread.Sleep(1);

            return _onPublishAsync(failureMode, payload, properties, ex);
        }

        protected List<PoisonMessage> _messages = new List<PoisonMessage>();

        public IEnumerable<PoisonMessage> Messages
        {
            get { return _messages; }
        }

        public void Clear()
        {
            _messages.Clear();
            Console.WriteLine("Clearing message queue.. count = {0}",
                _messages.Count);
        }
    }

    public class PoisonMessage
    {
        public FailureMode Mode { get; set; }
        public byte[] Payload { get; set; }
        public IDictionary<string, string> Properties { get; set; }
        public Exception ExceptionObject { get; set; }
    }
}