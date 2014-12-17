using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dependencies;
using Microsoft.Practices.DataPipeline.Dispatcher;
using Microsoft.Practices.DataPipeline.Dispatcher.Tests;

namespace Microsoft.Practices.DataPipeline.Dispatcher.Tests
{
    class MockDependencyResolver : IDependencyResolver
    {
        private readonly IPoisonMessageHandler _handler;

        public MockDependencyResolver(IPoisonMessageHandler handler)
        {
            _handler = handler;
        }
        
        public IDependencyScope BeginScope()
        {
            return this;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof (IPoisonMessageHandler))
                return _handler;
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
    }
}
