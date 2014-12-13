namespace Microsoft.Practices.DataPipeline.Processor
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
