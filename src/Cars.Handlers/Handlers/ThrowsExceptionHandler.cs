namespace Microsoft.Practices.DataPipeline.Cars.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Practices.DataPipeline.Logging;
    using Microsoft.Practices.DataPipeline.Processor;

    [MessageHandler(MessageType = "ThowsExceptionMessage", Version = 1)]
    public class ThrowsExceptionHandler : IMessageHandler
    {
        protected static readonly ILogger Logger = LoggerFactory.GetLogger("ThrowsExceptionHandler");

        public string Name
        {
            get
            {
                return "ThrowsExceptionHandler";
            }
        }

        public TimeSpan Timeout
        {
            get { return TimeSpan.FromSeconds(1); }
        }

        public Task ExecuteAsync(
            ProcessingContext context,
            byte[] payload, 
            IDictionary<string, string> properties)
        {
            Logger.Info("Intentionally throwing exception.");
            throw new JustForTestingException();
        }
    }

    public class JustForTestingException : Exception
    {
    }
}