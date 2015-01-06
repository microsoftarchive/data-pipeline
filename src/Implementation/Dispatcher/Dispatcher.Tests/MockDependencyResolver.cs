namespace Microsoft.Practices.DataPipeline.Dispatcher.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http.Dependencies;

    internal class MockDependencyResolver : IDependencyResolver
    {
        private readonly Dictionary<Type,object> _registrations = new Dictionary<Type, object>();
 
        private MockDependencyResolver()
        {
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }

        public object GetService(Type serviceType)
        {
            return _registrations.ContainsKey(serviceType) 
                ? _registrations[serviceType] 
                : Activator.CreateInstance(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return new[] { GetService(serviceType) };
        }

        public void Dispose()
        {
        }

        public MockDependencyResolver Register<T>(T service)
        {
            _registrations[typeof(T)] = service;
            return this;
        }

        public static MockDependencyResolver CreateFor<T>(T service)
        {
            var resolver = new MockDependencyResolver();
            resolver.Register(service);
            return resolver;
        }

        public static MockDependencyResolver Create()
        {
            return new MockDependencyResolver();
        }
    }
}