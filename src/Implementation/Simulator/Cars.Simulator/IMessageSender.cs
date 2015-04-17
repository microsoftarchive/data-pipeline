// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.Cars.Dispatcher.Simulator
{
    using System.Threading.Tasks;

    public interface IMessageSender
    {
        Task<bool> SendAsync(string partitionKey, object msg);
    }
}