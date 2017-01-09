using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using Microsoft.Extensions.DependencyInjection;

namespace Web.Api.Net452.Infrastructure
{
    public class DefaultIDependencyScope : IDependencyScope
    {
        private readonly IServiceScope _serviceScope;

        public DefaultIDependencyScope(IServiceScope serviceScope)
        {
            _serviceScope = serviceScope;
        }

        public void Dispose()
        {
            _serviceScope.Dispose();
        }

        public object GetService(Type serviceType)
        {
            return _serviceScope.ServiceProvider.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _serviceScope.ServiceProvider.GetServices(serviceType);
        }
    }

    public class DefaultDependencyResolver : IDependencyResolver
    {
        protected IServiceProvider ServiceProvider;


        public DefaultDependencyResolver(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;            
        }

        public IDependencyScope BeginScope()
        {
            return new DefaultIDependencyScope(ServiceProvider.CreateScope());
        }

        public void Dispose()
        {
        }

        public object GetService(Type serviceType)
        {
            return ServiceProvider.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return ServiceProvider.GetServices(serviceType);
        }
    }
}