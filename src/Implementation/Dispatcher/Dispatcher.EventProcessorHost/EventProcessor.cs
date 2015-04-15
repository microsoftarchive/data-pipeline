// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;

    using Microsoft.Practices.DataPipeline.Dispatcher.Instrumentation;
    using Microsoft.Practices.DataPipeline.Logging;
    using Microsoft.ServiceBus.Messaging;
    using Microsoft.WindowsAzure.Storage;

    public class EventProcessor : IEventProcessor
    {
        private static readonly ILogger Logger =
            LoggerFactory.GetLogger("Dispatcher.Processor");

        private static readonly byte[] EmptyContent = new byte[0];

        /// <summary>
        /// The message type resolver used to map incoming payloads to 
        /// processing functions
        /// </summary>
        private readonly IMessageHandlerResolver _handlerResolver;

        /// <summary>
        /// Execution options for tuning the degree of parallel 
        /// execution in message processing
        /// </summary>
        private readonly ExecutionDataflowBlockOptions _options;

        /// <summary>
        /// A circuit breaker to prevent a system overload when
        /// too much work is pending
        /// </summary>
        private readonly ICircuitBreaker _circuitBreaker;

        /// <summary>
        /// The name of the event hub
        /// </summary>
        private readonly string _eventHubName;


        private readonly IDispatcherInstrumentationPublisher _instrumentationPublisher = null;

        /// <summary>
        /// Instantiate a message processor, using the provided handlers
        /// and timeout/concurrency configuration
        /// </summary>
        public EventProcessor(IMessageHandlerResolver handlerResolver,
            ICircuitBreaker circuitBreaker,
            int maxConcurrency,
            string eventHubName,
            IDispatcherInstrumentationPublisher instrumentationPublisher)
        {
            Logger.Info(
                "Initializing event processor with concurrency {0}",
                maxConcurrency);

            _handlerResolver = handlerResolver;
            _circuitBreaker = circuitBreaker;
            _eventHubName = eventHubName;
            _instrumentationPublisher = instrumentationPublisher;

            // Set up the execution options for bounded concurrency
            // when using the TPL data flow action block
            _options = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = maxConcurrency
            };
        }

        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            if (reason == CloseReason.LeaseLost)
            {
                Logger.LeaseLost(_eventHubName, context.Lease.PartitionId);
            }
            if (reason == CloseReason.Shutdown)
            {
                Logger.ShutDownInitiated(_eventHubName, context.Lease.PartitionId);

            }

            return Task.FromResult<object>(null);
        }

        public Task OpenAsync(PartitionContext context)
        {
            Logger.LeaseObtained(_eventHubName, context.Lease.PartitionId);

            return Task.FromResult<object>(null);
        }

        public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            // Workaround for event hub sending null on timeout
            messages = messages ?? Enumerable.Empty<EventData>();

            return ProcessEventsAsync(context, messages, CancellationToken.None);
        }

        public async Task ProcessEventsAsync(
            PartitionContext context,
            IEnumerable<EventData> messages,
            CancellationToken cancellationToken)
        {
            await _circuitBreaker.CheckBreak(cancellationToken).ConfigureAwait(false);

            var batchStopwatch = Stopwatch.StartNew();
            var batchEventCount = 0;

            // Instantiate an action block for parallel processing of 
            // messages, using an asynchronous lambda 
            var workerBlock = new ActionBlock<EventData>(async msg =>
                await ProcessAsync(msg, context.Lease.PartitionId).ConfigureAwait(false),
                _options);

            // Post each message into the worker block, which will 
            // execute them with bounded concurrency
            foreach (var m in messages)
            {
                batchEventCount++;
                workerBlock.Post(m);
            }

            // Signal the worker block to accept no additional work
            workerBlock.Complete();

            // Wait for all messages to be processed.  Note that any message 
            // which has execeeded its timeout value will be dumped to the 
            // poison message queue, and processing will continue
            await workerBlock.Completion.ConfigureAwait(false);

            // Update the checkpoint for this partition and continue processing
            try
            {
                await context.CheckpointAsync().ConfigureAwait(false);
                Logger.BatchCompleted(
                    _eventHubName,
                    context.Lease.PartitionId,
                    batchEventCount,
                    batchStopwatch.Elapsed);
            }
            catch (Exception ex)
            {
                if (!(ex is StorageException || ex is LeaseLostException))
                {
                    throw;
                }

                Logger.UnableToCheckpoint(
                    ex,
                    _eventHubName,
                    context.Lease.PartitionId,
                    batchEventCount,
                    batchStopwatch.Elapsed);
            }
        }

        /// <summary>
        /// Process the message contained in the event data object, 
        /// handling timeouts, errors and telemetry
        /// </summary>
        private async Task ProcessAsync(EventData msg, string partitionId)
        {
            var context = new ProcessingContext(_eventHubName, partitionId, msg.Offset);

            Exception ex = null;
            string handlerName = "[handler not yet resolved]";
            byte[] content = EmptyContent;
            Dictionary<string, string> props = null;

            try
            {
                // The message stream can only be read once, extract the body up front
                content = msg.GetBytes();

                // if only some properties are used in the happy path but all are needed in some cases.
                props = msg.Properties.ToDictionary(
                    k => k.Key,
                    v => v.Value.ToString());

                // Promote standard EventHub properties into the property collection (for logging)
                var msgId = String.Concat(_eventHubName, "/", msg.PartitionKey, "/", msg.Offset);
                props.Add(Constants.ID, msgId);
                props.Add(Constants.MSG_OFFSET, msg.Offset);
                props.Add(Constants.PARTITION, msg.PartitionKey);

                // Look up the message type handler, using the payload and 
                // associated properties
                var handler = _handlerResolver.GetHandler(props, msgId);
                handlerName = handler.Name;

                // Create a task to track timeout
                var sw = Stopwatch.StartNew();
                var timeoutTask = Task.Delay(handler.Timeout);

                // Invoke the handler, using the continuation token to handle exceptions and
                // elapsed time logging.  Note: if this method throws synchronously (a misbehaving
                // handler) then we handle it in the catch statement below

                var handlerTask = handler
                    .ExecuteAsync(context, content, props)
                    .ContinueWith(async t => await HandleCompletionAsync(t, sw, handlerName, content, props, context).ConfigureAwait(false));

                _instrumentationPublisher.TaskStarted();
                _circuitBreaker.Increment();

                // Wait for either the timeout task or the handler to complete
                await Task.WhenAny(handlerTask, timeoutTask).ConfigureAwait(false);

                // If the message did not complete before the timeout, flag for further review
                if (handlerTask.Status != TaskStatus.RanToCompletion)
                {
                    props.Add("timeoutDuration", handler.Timeout.ToString());

                    _instrumentationPublisher.TimeoutOccured();
                    await Logger.LogHandlerTimeout(handler, context, content, props);
                }
            }
            catch (Exception ex0)
            {
                ex = ex0;
            }

            // Misbehaving handler threw a synchronous exception
            if (ex != null)
            {
                // We can't await in a catch block
                await Logger.HandlerThrewException(
                    handlerName,
                    context,
                    content,
                    props ?? new Dictionary<string, string>(),
                    ex);
            }
        }

        /// <summary>
        /// Handle completion of the preceding task, including error logging,
        /// and timeout logging (poison publishing in case of a timeout message
        /// is handled in the main message flow).
        /// </summary>        
        private async Task HandleCompletionAsync(
            Task t,
            Stopwatch sw,
            string handlerName,
            byte[] content,
            IDictionary<string, string> props,
            ProcessingContext context)
        {
            sw.Stop();
            _instrumentationPublisher.TaskEnded();
            _instrumentationPublisher.MessageProcessed();
            _circuitBreaker.Decrement();

            // If the preceding task failed with an exception, flag the exception and
            // log the message body/properties 
            if (t.IsFaulted)
            {
                _instrumentationPublisher.TaskFaulted();
                await HandleExceptionAsync(handlerName, context, sw.Elapsed, content, props, t.Exception).ConfigureAwait(false);
            }
            // Otherwise log the execution time (if the message is in a timeout condition, 
            // it will be logged in the primary flow as a warning)
            else
            {
                Logger.TraceApi(
                    handlerName,
                    sw.Elapsed,
                    "{0}/{1}/OK",
                    context.PartitionId,
                    context.EventDataOffset);
            }
        }

        private static async Task HandleExceptionAsync(
            string handlerName,
            ProcessingContext context,
            TimeSpan elapsed,
            byte[] content,
            IDictionary<string, string> props,
            Exception exception)
        {
            await Logger.HandlerThrewException(handlerName, context, content, props, exception);

            Logger.TraceApi(
                handlerName,
                elapsed,
                "{0}/{1}/ERR",
                context.PartitionId,
                context.EventDataOffset);
        }
    }
}