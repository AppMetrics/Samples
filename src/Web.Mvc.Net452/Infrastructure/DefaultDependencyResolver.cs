using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Web.Mvc.Net452.Infrastructure
{
    public class DefaultDependencyResolver : IDependencyResolver
    {
        protected IServiceProvider ServiceProvider;

        public DefaultDependencyResolver(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
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