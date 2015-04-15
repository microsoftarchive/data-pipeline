// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPoisonMessageHandler
    {
        Task PublishAsync(
            FailureMode failureMode,
            ProcessingContext context,
            byte[] payload,
            IDictionary<string, string> properties,
            Exception ex = null);
    }

    public enum FailureMode
    {
        UnknownPayload,
        Error,
        Timeout
    }
}