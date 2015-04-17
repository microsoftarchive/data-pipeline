// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.Dispatcher
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface ICircuitBreaker
    {
        Task CheckBreak(CancellationToken cancellationToken);
        void Decrement();
        void Increment();
    }
}
