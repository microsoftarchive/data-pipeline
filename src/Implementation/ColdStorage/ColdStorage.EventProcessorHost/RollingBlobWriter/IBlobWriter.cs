// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
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