using System;
using System.Net.Http;
using System.Threading.Tasks;
using App.Metrics.Scheduling;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Reservoirs.Samples
{
    public class Startup
    {
        public static readonly Uri ApiBaseAddress = new Uri("http://localhost:50202/");

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        public void Configure(IApplicationBuilder app)
        {
            RunRequestsToSample();

            app.UseMvc();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting(options => { options.LowercaseUrls = true; });
            services.AddMvc(options => options.AddMetricsResourceFilter());
        }

        private static void RunRequestsToSample()
        {
            var httpClient = new HttpClient
            {
                BaseAddress = ApiBaseAddress
            };

            var requestSamplesScheduler = new AppMetricsTaskScheduler(TimeSpan.FromMilliseconds(100), async () =>
            {
                var uniform = httpClient.GetStringAsync("api/reservoirs/uniform");
                var exponentiallyDecaying = httpClient.GetStringAsync("api/reservoirs/exponentially-decaying");
                var slidingWindow = httpClient.GetStringAsync("api/reservoirs/sliding-window");

                await Task.WhenAll(uniform, exponentiallyDecaying, slidingWindow);
            });

            requestSamplesScheduler.Start();
        }
    }
}