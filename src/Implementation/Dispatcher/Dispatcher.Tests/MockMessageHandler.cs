// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.Dispatcher.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Practices.DataPipeline.Dispatcher;

    public class MockMessageHandler : IMessageHandler
    {
        private readonly Func<byte[], IDictionary<string, string>, Task> onExecute;

        public MockMessageHandler()
            : this((bytes, dictionary) => Task.FromResult<object>(null))
        {
        }

        public MockMessageHandler(Func<byte[], IDictionary<string, string>, Task> onExecute)
        {
            this.onExecute = onExecute;
        }

        public string Name
        {
            get { return "MockMessageHandler";  }
        }

        public Task ExecuteAsync(
            ProcessingContext context,
            byte[] payload,
            IDictionary<string, string> properties)
        {
            return this.onExecute(payload, properties);
        }

        public TimeSpan Timeout
        {
            get { return TimeSpan.FromSeconds(1); }
        }
    }
}