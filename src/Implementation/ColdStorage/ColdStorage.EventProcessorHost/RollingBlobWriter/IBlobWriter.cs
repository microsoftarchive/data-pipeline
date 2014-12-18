namespace Microsoft.Practices.DataPipeline.ColdStorage.BlobWriter
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IBlobWriter
    {
        Task<bool> WriteAsync(IReadOnlyCollection<BlockData> blockDataList, CancellationToken cancellationToken);
    }
}