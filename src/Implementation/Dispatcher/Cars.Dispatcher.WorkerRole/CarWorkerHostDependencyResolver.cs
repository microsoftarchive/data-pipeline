// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.Cars.Dispatcher
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Http.Dependencies;

    using Microsoft.Practices.DataPipeline;
    using Microsoft.Practices.DataPipeline.Cars.Handlers;
    using Microsoft.Practices.DataPipeline.Dispatcher;
    using Microsoft.WindowsAzure.Storage;

    public sealed class CarWorkerHostDependencyResolver : IDependencyResolver
    {
        public async static Task<CarWorkerHostDependencyResolver> CreateAsync()
        {
            var resolver = new CarWorkerHostDependencyResolver();
            await resolver.InitializeAsync();
            return resolver;
        }

        private CarWorkerHostDependencyResolver()
        { }

        private async Task InitializeAsync()
        {
            var poisonMessageStorageAccount = ConfigurationHelper.GetConfigValue<CloudStorageAccount>("Dispatcher.PoisonMessageStorageAccount");            
            var poisonMessageContainer = ConfigurationHelper.GetConfigValue<string>("Dispatcher.PoisonMessageContainer");
            var badMessageHandler = await AzureBlobPoisonMessageHandler.CreateAsync(
                poisonMessageStorageAccount, poisonMessageContainer
            );
            _poisonHandler = badMessageHandler;
        }

        private IPoisonMessageHandler _poisonHandler;


        public IDependencyScope BeginScope()
        {
            return this;
        }

        public object GetService(Type serviceType)
        {
            if (HandlerMap.ContainsKey(serviceType))
                return (object) HandlerMap[serviceType]();
            else if (serviceType == typeof(IPoisonMessageHandler))
                return _poisonHandler;
            else
                return Activator.CreateInstance(serviceType);            
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return new Object[] { GetService(serviceType) };            
        }

        public void Dispose()
        {
            
        }

        private ConcurrentDictionary<Type, object> _instanceMap = 
            new ConcurrentDictionary<Type, object>(); 

        private static readonly Dictionary<Type, Func<IMessageHandler>> HandlerMap =
            new Dictionary<Type, Func<IMessageHandler>>
            {
                {
                    typeof(LongRunningHandler), 
                    () => new LongRunningHandler(
                        ConfigurationHelper.GetConfigValue<TimeSpan>("Handler.LongRunningTaskDuration")
                    )
                }
            };
    }
}
