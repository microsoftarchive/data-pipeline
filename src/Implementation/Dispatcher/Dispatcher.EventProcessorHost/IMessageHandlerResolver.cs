// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.Dispatcher
{
    using System.Collections.Generic;

    public interface IMessageHandlerResolver
    {
        IMessageHandler GetHandler(IDictionary<string, string> headers, string messageId);
    }
}