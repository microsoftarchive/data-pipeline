// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.Cars.Dispatcher.Simulator
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reactive.Subjects;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;

    public class Car
    {
        private static readonly TimeSpan LoopFrequency = TimeSpan.FromSeconds(0.33);

        private static readonly ILogger Logger = LoggerFactory.GetLogger("Simulator");

        private readonly string _deviceId;

        private readonly IEnumerable<MessagingEntry> _messagingList;

        private readonly Func<string, object, Task<bool>> _sendMessageAsync;

        public ISubject<int> ObservableEventCount { get; private set; }

        public Car(
            string deviceId,
            IEnumerable<MessagingEntry> messagingList,
            Func<string, object, Task<bool>> sendMessageAsync)
        {
            _deviceId = deviceId;
            _sendMessageAsync = sendMessageAsync;
            _messagingList = messagingList;

            ObservableEventCount = new Subject<int>();
        }

        public async Task RunSimulationAsync(CancellationToken token)
        {
            var stopwatch = Stopwatch.StartNew();

            Logger.CarStarting(_deviceId);

            try
            {
                while (!token.IsCancellationRequested)
                {
                    var elaspedTime = stopwatch.Elapsed;
                    stopwatch.Restart();

                    foreach (var entry in _messagingList)
                    {
                        entry.UpdateElapsedTime(elaspedTime);
                        if (!entry.ShouldSendMessage())
                        {
                            continue;
                        }
                        entry.ResetElapsedTime();

                        var msg = entry.CreateNewMessage();
                        var partitionKey = _deviceId.ToString(CultureInfo.InvariantCulture);
                        var wasEventSent = await _sendMessageAsync(partitionKey, msg);

                        if (wasEventSent)
                        {
                            ObservableEventCount.OnNext(1);
                        }
                        else
                        {
                            // If the event was not sent, it is likely that Event Hub
                            // is throttling our requests. So we will cause the simulation
                            // for this particular car to delay and reduce the load.
                            // Note that in some cases you will want resend the event,
                            // however we are merely pausing before trying to send
                            // the next one.
                            try
                            {
                                await Task.Delay(TimeSpan.FromSeconds(10), token);
                            }
                            catch (TaskCanceledException) { /* cancelling Task.Delay will throw */ }
                        }
                    }

                    try
                    {
                        await Task.Delay(LoopFrequency, token);
                    }
                    catch (TaskCanceledException) { /* cancelling Task.Delay will throw */ }
                }
            }
            catch (Exception e)
            {
                ObservableEventCount.OnError(e);
                Logger.CarUnexpectedFailure(e, _deviceId);
                return;
            }

            ObservableEventCount.OnCompleted();

            Logger.CarStopping(_deviceId);
        }

        public async Task RunOneMessageSimulationAsync(CancellationToken token)
        {
            Logger.CarStarting(_deviceId);

            try
            {
                foreach (var entry in _messagingList)
                {
                    var msg = entry.CreateNewMessage();
                    var partitionKey = _deviceId.ToString(CultureInfo.InvariantCulture);
                    await _sendMessageAsync(partitionKey, msg);
                }
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception e)
            {
                Logger.CarUnexpectedFailure(e, _deviceId);
            }

            Logger.CarStopping(_deviceId);
        }
    }
}