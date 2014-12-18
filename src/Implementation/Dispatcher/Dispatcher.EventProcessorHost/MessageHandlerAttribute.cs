namespace Microsoft.Practices.DataPipeline.Dispatcher
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class MessageHandlerAttribute : Attribute
    {
        public string MessageType { get; set; }

        public int Version { get; set; }
    }
}