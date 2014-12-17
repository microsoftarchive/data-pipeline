using System.Collections.Concurrent;
using Microsoft.Practices.DataPipeline;
using Microsoft.Practices.DataPipeline.Cars.Handlers;
using Microsoft.Practices.DataPipeline.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dependencies;
using Microsoft.WindowsAzure.Storage;

namespace Cars.WorkerHost
{
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
