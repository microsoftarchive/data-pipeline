// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Microsoft.Practices.DataPipeline.Dispatcher
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IMessageHandler
    {
        /// <summary>
        /// Name of the message handler (used for logging purposes)
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The timeout value for message execution (after this time period, 
        /// keep processing other messages, but do not cancel or abort the
        /// running task)
        /// </summary>
        TimeSpan Timeout { get; }

        /// <summary>
        /// Execute the handler with the given payload and properties
        /// </summary>        
        Task ExecuteAsync(ProcessingContext context, byte[] payload, IDictionary<string, string> properties);
    }
}