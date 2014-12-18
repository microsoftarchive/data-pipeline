namespace Microsoft.Practices.DataPipeline.Tests.Helpers
{
    using System;
    using System.Threading.Tasks;

    public static class TaskHelpers
    {
        public static Task CreateFaultedTask<TException>()
            where TException : Exception, new()
        {
            return CreateFaultedTask<object, TException>();
        }

        public static Task CreateFaultedTask<T, TException>()
            where TException : Exception, new()
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetException(new TException());
            return tcs.Task;
        }

        public static Task CreateCompletedTask()
        {
            return CreateCompletedTask(false);
        }

        public static Task<T> CreateCompletedTask<T>(T result)
        {
            return Task.FromResult(result);
        }
    }
}