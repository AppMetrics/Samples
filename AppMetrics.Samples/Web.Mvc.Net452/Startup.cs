using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Owin;
using Web.Mvc.Net452;
using Web.Mvc.Net452.Infrastructure;

[assembly: OwinStartup(typeof(Startup))]

namespace Web.Mvc.Net452
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var services = new ServiceCollection();

            ConfigureServices(services);

            services.SetDependencyResolver();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();

            services.AddControllersAsServices();

            services
                .AddMetrics(options => { options.DefaultContextLabel = "Mvc.Sample.Net452"; }, Assembly.GetExecutingAssembly().GetName())
                .AddHealthChecks();
        }
    }
}