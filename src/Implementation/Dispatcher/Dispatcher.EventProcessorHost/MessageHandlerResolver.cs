namespace Microsoft.Practices.DataPipeline.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    using Logging;

    using System.Web.Http.Dependencies;

    using Microsoft.Practices.DataPipeline.Cars.Messages;

    public class MessageHandlerResolver : IMessageHandlerResolver
    {
        private static readonly ILogger Logger =
            LoggerFactory.GetLogger("Dispatcher.Resolver");

        private IReadOnlyDictionary<MessageRegistration, IMessageHandler> _dispatchTable;
        private readonly IMessageHandler _defaultHandler;
        private readonly IDependencyResolver _resolver;

        public MessageHandlerResolver(IEnumerable<Type> typesToSearch)
        {
            // Use the dependency resolver to create mapping types
            _resolver = DependencyResolverFactory.GetResolver();

            // Initialize the dispatch table given a bounded set of 
            // types.  Message handlers are identified by implementing
            // IMessageHandler, with a [MessageHandler] attribute 
            this.InitializeDispatchTable(typesToSearch);

            // Create a default handler for unknown types
            _defaultHandler = new UnknownTypeMessageHandler();
        }

        public IMessageHandler GetHandler(IDictionary<string, string> headers, string messageId)
        {
            // Attempt to determine the message type by checking these
            // promoted properties.
            Logger.Debug("Attempting to find handler for message id {0}", messageId);

            var registration = ExtractFrom(headers);
            //Logger.Debug("Extracted handler registration from headers: {0}, for message id {1}", registration, messageId);

            if (this._dispatchTable.ContainsKey(registration))
            {
                //Logger.Debug("Found handler for registration: {0}, for message id {1}", registration, messageId);
                return this._dispatchTable[registration];
            }

            // Return the default handler
            Logger.Warning("No handler was found for message id {0}", messageId);
            return _defaultHandler;
        }

        private static MessageRegistration ExtractFrom(IDictionary<string, string> properties)
        {
            string type;
            string versionString;
            int version = 0;

            properties.TryGetValue(EventDataPropertyKeys.MessageType, out type);
            if (properties.TryGetValue(EventDataPropertyKeys.MessageTypeVersion, out versionString))
            {
                version = int.Parse(versionString);
            }

            return new MessageRegistration { Type = type, Version = version };
        }

        private void InitializeDispatchTable(IEnumerable<Type> types)
        {
            // Create a temporary holding dictionary for the type->instance map
            var handlers = new Dictionary<MessageRegistration, IMessageHandler>();

            var filteredTypes = types.Where(t => typeof(IMessageHandler).IsAssignableFrom(t));

            // Iterate through the types and generate a type->instance of type
            // map
            foreach (var type in filteredTypes)
            {
                // Each handler may be registered to multiple types
                var registrations = type.GetCustomAttributes(
                    typeof(MessageHandlerAttribute), true);

                foreach (MessageHandlerAttribute registration in registrations)
                {
                    // Create an instance of the type and registration key
                    var handler = _resolver.GetService(type) as IMessageHandler;
                    if (handler == null)
                    {
                        Logger.Error(
                            "Requested type {0} does not return a valid IMessageHandler from the dependency resolver",
                            type.AssemblyQualifiedName);
                        continue;
                    }

                    var key = new MessageRegistration
                    {
                        Type = registration.MessageType,
                        Version = registration.Version
                    };

                    Logger.Debug(
                        "Registering type {0} to handle message type {1}.",
                        type.AssemblyQualifiedName,
                        key);

                    // Add to the set of handlers
                    handlers.Add(key, handler);
                }
            }

            // Convert the temporary map into an immutable dictionary for
            // performance in concurrent applications
            Logger.Debug("Registering {0} handlers", handlers.Keys.Count);
            this._dispatchTable = handlers.ToImmutableDictionary();
        }
    }
}
