namespace Microsoft.Practices.DataPipeline.Processor.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Practices.DataPipeline.Processor;
    using Microsoft.Practices.DataPipeline.Tests;

    using Xunit;

    public class GivenACircuitBreaker
    {
        private static readonly TimeSpan stallInterval = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan timeoutInterval = TimeSpan.FromSeconds(20);

        private CircuitBreaker _circuitBreaker;

        public GivenACircuitBreaker()
        {
            _circuitBreaker = new CircuitBreaker(
                "test",
                "partition",
                warningLevel: 10,
                tripLevel: 20,
                stallInterval: stallInterval,
                logCooldownInterval: TimeSpan.FromSeconds(200));
        }

        [Fact]
        [Trait("Running time", "Short")]
        public async Task WhenIncrementsAreBelowTripLevel_ThenDoesNotStallOnCheck()
        {
            var cts = new CancellationTokenSource();

            Parallel.For(0, 15, i => _circuitBreaker.Increment());

            Task checkTask;
            await Task.WhenAny(
                checkTask = _circuitBreaker.CheckBreak(cts.Token),
                Task.Delay(timeoutInterval));

            Assert.True(checkTask.Status == TaskStatus.RanToCompletion);

            cts.Cancel();
            await checkTask;
        }

        [Fact]
        [Trait("Running time", "Short")]
        public async Task WhenIncrementsAndDecrementsAreBelowTripLevel_ThenDoesNotStallOnCheck()
        {
            var cts = new CancellationTokenSource();

            Parallel.For(
                0,
                25,
                i =>
                {
                    _circuitBreaker.Increment();
                    if (i % 2 == 0)
                    {
                        _circuitBreaker.Decrement();
                    }
                });

            Task checkTask;
            await Task.WhenAny(
                checkTask = _circuitBreaker.CheckBreak(cts.Token),
                Task.Delay(timeoutInterval));

            Assert.True(checkTask.Status == TaskStatus.RanToCompletion);

            cts.Cancel();
            await checkTask;
        }

        [Fact]
        [Trait("Running time", "Long")]
        public async Task WhenIncrementsAreAboveTripLevel_ThenStallsOnCheck()
        {
            var cts = new CancellationTokenSource();

            Parallel.For(0, 25, i => _circuitBreaker.Increment());

            Task checkTask;
            await Task.WhenAny(
                checkTask = _circuitBreaker.CheckBreak(cts.Token),
                Task.Delay(timeoutInterval));

            Assert.False(checkTask.Status == TaskStatus.RanToCompletion);

            cts.Cancel();

            await AssertExt.ThrowsAsync<TaskCanceledException>(async () => await checkTask);
        }

        [Fact]
        [Trait("Running time", "Long")]
        public async Task WhenIncrementsAreAboveTripLevel_ThenStallsOnCheckForSeveralStallIntervals()
        {
            var cts = new CancellationTokenSource();

            Parallel.For(0, 25, i => _circuitBreaker.Increment());

            Task checkTask;
            await Task.WhenAny(
                checkTask = _circuitBreaker.CheckBreak(cts.Token),
                Task.Delay(timeoutInterval + timeoutInterval + timeoutInterval));

            Assert.False(checkTask.Status == TaskStatus.RanToCompletion);

            cts.Cancel();

            await AssertExt.ThrowsAsync<TaskCanceledException>(async () => await checkTask);
        }

        [Fact]
        [Trait("Running time", "Long")]
        public async Task WhenLevelIsBroughtBelowTripLevelButAboveWarningLevel_ThenStallsOnCheck()
        {
            var cts = new CancellationTokenSource();

            Parallel.For(0, 25, i => _circuitBreaker.Increment());

            Task checkTask;
            await Task.WhenAny(
                checkTask = _circuitBreaker.CheckBreak(cts.Token),
                Task.Delay(timeoutInterval));

            Assert.False(checkTask.Status == TaskStatus.RanToCompletion);

            Parallel.For(0, 10, i => _circuitBreaker.Decrement());

            await Task.WhenAny(
                checkTask,
                Task.Delay(timeoutInterval));

            cts.Cancel();
            await AssertExt.ThrowsAsync<TaskCanceledException>(async () => await checkTask);
        }

        [Fact]
        [Trait("Running time", "Long")]
        public async Task WhenLevelIsBroughtBelowWarningLevel_ThenReleasesStalling()
        {
            var cts = new CancellationTokenSource();

            Parallel.For(0, 25, i => _circuitBreaker.Increment());

            Task checkTask;
            await Task.WhenAny(
                checkTask = _circuitBreaker.CheckBreak(cts.Token),
                Task.Delay(timeoutInterval));

            Assert.False(checkTask.Status == TaskStatus.RanToCompletion);

            Parallel.For(0, 20, i => _circuitBreaker.Decrement());

            await Task.WhenAny(
                checkTask,
                Task.Delay(timeoutInterval));

            Assert.True(checkTask.Status == TaskStatus.RanToCompletion);
        }

        [Fact]
        [Trait("Running time", "Long")]
        public async Task WhenLevelIsBroughtBelowWarningLevel_NextCheckDoesNotStall()
        {
            var cts = new CancellationTokenSource();

            Parallel.For(0, 25, i => _circuitBreaker.Increment());

            Task checkTask;
            await Task.WhenAny(
                checkTask = _circuitBreaker.CheckBreak(cts.Token),
                Task.Delay(timeoutInterval));

            Assert.False(checkTask.Status == TaskStatus.RanToCompletion);

            Parallel.For(0, 20, i => _circuitBreaker.Decrement());

            await Task.WhenAny(
                checkTask,
                Task.Delay(timeoutInterval));

            Assert.True(checkTask.Status == TaskStatus.RanToCompletion);

            Parallel.For(0, 4, i => _circuitBreaker.Increment());

            await Task.WhenAny(
                checkTask = _circuitBreaker.CheckBreak(cts.Token),
                Task.Delay(timeoutInterval));

            Assert.True(checkTask.Status == TaskStatus.RanToCompletion);
        }
    }
}