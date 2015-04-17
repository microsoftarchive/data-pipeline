// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.Cars.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Practices.DataPipeline.Dispatcher;
    using Microsoft.Practices.DataPipeline.Logging;

    [MessageHandler(MessageType = "UpdateEngineNotificationMessage", Version = 1)]
    public class UpdateEngineNotificationHandler : IMessageHandler
    {
        protected static readonly ILogger Logger = LoggerFactory.GetLogger("UpdateEngineNotificationHandler");

        public string Name
        {
            get { return "UpdateEngineNotification"; }
        }

        public async Task ExecuteAsync(
            ProcessingContext context, 
            byte[] payload,
            IDictionary<string, string> properties)
        {
            // This handler is intentionally imcomplete.
            // In an actual handler, we would use the provided data
            // to retrieve and update the state of a particular
            // vehicle.
            await Task.Delay(TimeSpan.FromMilliseconds(50));
        }

        public TimeSpan Timeout
        {
            get { return TimeSpan.FromMilliseconds(100); }
        }
    }
}