namespace Microsoft.Practices.DataPipeline.Tests.Mocks
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.ServiceBus.Messaging;

    public class MockCheckpointManager : ICheckpointManager
    {
        private readonly Func<Task> _checkpointAsync;

        public MockCheckpointManager(Func<Task> checkpointAsync)
        {
            _checkpointAsync = checkpointAsync;
        }

        public Task CheckpointAsync(Lease lease, string offset, long sequenceNumber)
        {
            return _checkpointAsync();
        }
    }
}