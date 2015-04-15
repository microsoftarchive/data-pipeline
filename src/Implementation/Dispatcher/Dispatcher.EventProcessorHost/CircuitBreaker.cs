// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.Dispatcher
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Practices.DataPipeline.Logging;

    public class CircuitBreaker : ICircuitBreaker
    {
        private static readonly ILogger Logger = LoggerFactory.GetLogger("Dispatcher.Processor");

        private readonly string _processorName;
        private readonly string _partitionId;
        private readonly int _warningLevel;
        private readonly int _tripLevel;
        private readonly TimeSpan _stallInterval;
        private readonly TimeSpan _logCooldownInterval;
        private int _currentLevel;
        private DateTime? _nextWarningLogTime;

        public CircuitBreaker(string processorName, string partitionId, int warningLevel, int tripLevel, TimeSpan stallInterval, TimeSpan logCooldownInterval)
        {
            _processorName = processorName;
            _partitionId = partitionId;
            _warningLevel = warningLevel;
            _tripLevel = tripLevel;
            _stallInterval = stallInterval;
            _logCooldownInterval = logCooldownInterval;

            Logger.CircuitBreakerInitialized(_processorName, _partitionId, _warningLevel, _tripLevel, _stallInterval, _logCooldownInterval);
        }

        public void Increment()
        {
            Interlocked.Increment(ref _currentLevel);
        }

        public void Decrement()
        {
            var currentCount = Interlocked.Decrement(ref _currentLevel);
        }

        public async Task CheckBreak(CancellationToken cancellationToken)
        {
            var currentLevel = Volatile.Read(ref _currentLevel);
            if (currentLevel < _warningLevel)
            {
                // Circuit is closed

                Logger.CircuitBreakerClosed(_processorName, _partitionId, currentLevel);
                _nextWarningLogTime = null;

                return;
            }
            else if (currentLevel < _tripLevel)
            {
                // Circuit is closed, but log a warning if appropriate

                if (_nextWarningLogTime == null || _nextWarningLogTime.Value <= DateTime.UtcNow)
                {
                    Logger.CircuitBreakerWarning(_processorName, _partitionId, _warningLevel, currentLevel);
                    _nextWarningLogTime = DateTime.UtcNow.Add(_logCooldownInterval);
                }

                return;
            }

            // Circuit is open. Stall until conditions are restored, logging an update when appropriate

            Logger.CircuitBreakerTripped(_processorName, _partitionId, _tripLevel, currentLevel);
            var nextErrorLogTime = DateTime.UtcNow.Add(_logCooldownInterval);
            while (true)
            {
                Logger.CircuitBreakerStalling(_processorName, _partitionId, _tripLevel, currentLevel, _stallInterval);
                await Task.Delay(_stallInterval, cancellationToken);

                currentLevel = Volatile.Read(ref _currentLevel);

                if (currentLevel < _warningLevel)
                {
                    Logger.CircuitBreakerRestored(_processorName, _partitionId, _warningLevel, currentLevel);
                    _nextWarningLogTime = null;
                    return;
                }

                var now = DateTime.UtcNow;
                if (nextErrorLogTime <= now)
                {
                    Logger.CircuitBreakerTripped(_processorName, _partitionId, _tripLevel, currentLevel);
                    nextErrorLogTime = now.Add(_logCooldownInterval);
                }
            }
        }
    }
}
