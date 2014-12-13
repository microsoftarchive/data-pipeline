namespace Microsoft.Practices.DataPipeline.Processor.Tests
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Practices.DataPipeline.Processor;

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
