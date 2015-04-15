// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.ColdStorage
{
    using System;
    using System.Threading;
    using Microsoft.ServiceBus.Messaging;

    using Microsoft.Practices.DataPipeline.ColdStorage.BlobWriter;
    using Microsoft.Practices.DataPipeline.Logging;
    using Microsoft.Practices.DataPipeline.ColdStorage.Instrumentation;


    public class ColdStorageEventProcessorFactory : IEventProcessorFactory
    {
        private static readonly ILogger Logger = 
            LoggerFactory.GetLogger("ColdStorage.Processor");

        private readonly Func<string, IBlobWriter> _blobWriterFactory = null;
        private readonly IColdStorageInstrumentationPublisher _instrumentationPublisher;
        private readonly CancellationToken _token = CancellationToken.None;
        private readonly int _warningLevel;
        private readonly int _tripLevel;
        private readonly TimeSpan _stallInterval;
        private readonly TimeSpan _logCooldownInterval;
        private readonly string _eventHubName;

        public ColdStorageEventProcessorFactory(
            Func<string, IBlobWriter> blobWriterFactory,
            IColdStorageInstrumentationPublisher instrumentationPublisher,
            CancellationToken token,
            int warningLevel,
            int tripLevel,
            TimeSpan stallInterval,
            TimeSpan logCooldownInterval,
            string eventHubName)
        {

            Guard.ArgumentNotNull(blobWriterFactory, "blobWriterFactory");
            Guard.ArgumentNotNull(instrumentationPublisher, "instrumentationPublisher");

            _token = token;
            _blobWriterFactory = blobWriterFactory;
            _instrumentationPublisher = instrumentationPublisher;
            _warningLevel = warningLevel;
            _tripLevel = tripLevel;
            _stallInterval = stallInterval;
            _logCooldownInterval = logCooldownInterval;
            _eventHubName = eventHubName;
        }

        public IEventProcessor CreateEventProcessor(PartitionContext context)
        {
            var processor = new ColdStorageProcessor(
                _blobWriterFactory,
                _instrumentationPublisher,
                _token,
                _warningLevel,
                _tripLevel,
                _stallInterval,
                _logCooldownInterval, 
                _eventHubName
            );
            return processor;
        }
    }
}
