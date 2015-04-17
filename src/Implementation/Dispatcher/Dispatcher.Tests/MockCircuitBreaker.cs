// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.Dispatcher.Tests
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Practices.DataPipeline.Dispatcher;

    class MockCircuitBreaker : ICircuitBreaker
    {
        public Task CheckBreak(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public void Decrement()
        {
        }

        public void Increment()
        {
        }
    }
}
