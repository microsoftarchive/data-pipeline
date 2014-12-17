namespace Microsoft.Practices.DataPipeline.Cars.Dispatcher.Simulator
{
    using System;

    public class MessagingEntry
    {
        private readonly double _frequency;

        private readonly double _jitter;

        private readonly Func<Random, object> _messageFactory;

        private double _frequencyWithJitter;

        private double _totalElapsedMilliseconds;

        private readonly Random _random;

        public MessagingEntry(Func<Random, object> messageFactory, TimeSpan frequency, double percentToJitter = 0f)
        {
            _messageFactory = messageFactory;
            _frequency = frequency.TotalMilliseconds;
            _jitter = percentToJitter;
            _random = new Random();

            ResetElapsedTime();
        }

        public TimeSpan ElapsedTime
        {
            get
            {
                return TimeSpan.FromMilliseconds(_totalElapsedMilliseconds);
            }
        }

        public object CreateNewMessage()
        {
            return _messageFactory(_random);
        }

        public void ResetElapsedTime()
        {
            _totalElapsedMilliseconds = 0;

            var nextJitter = (_random.NextDouble() * _jitter);
            _frequencyWithJitter = _frequency + (nextJitter * _frequency);
        }

        public bool ShouldSendMessage()
        {
            return _totalElapsedMilliseconds >= _frequencyWithJitter;
        }

        public void UpdateElapsedTime(TimeSpan elapsed)
        {
            _totalElapsedMilliseconds += elapsed.TotalMilliseconds;
        }
    }
}