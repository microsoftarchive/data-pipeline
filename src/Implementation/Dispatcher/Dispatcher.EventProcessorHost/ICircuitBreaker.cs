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
