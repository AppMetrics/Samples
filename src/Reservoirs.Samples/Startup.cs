using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Extensions.Reporting.InfluxDB;
using App.Metrics.Extensions.Reporting.InfluxDB.Client;
using App.Metrics.Filtering;
using App.Metrics.Reporting.Interfaces;
using App.Metrics.Scheduling;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Reservoirs.Samples
{
    public class Startup
    {
        public static readonly Uri InfluxDbUri = new Uri("http://127.0.0.1:8086");
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

        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IApplicationLifetime lifetime)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.RollingFile(Path.Combine($@"C:\logs\{env.ApplicationName}", "log-{Date}.txt"))
                .CreateLogger();

            loggerFactory.AddSerilog(Log.Logger);

            app.UseMetrics();
            app.UseMetricsReporting(lifetime);

            RunRequestsToSample(lifetime.ApplicationStopping);

            app.UseMvc();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddLogging()
                .AddRouting(options => { options.LowercaseUrls = true; });

            services.AddMvc(options => options.AddMetricsResourceFilter());

            var filter = new DefaultMetricsFilter();
            filter.WhereContext(c => c == MetricsRegistry.Context);

            services
                .AddMetrics()
                .AddGlobalFilter(filter)
                .AddJsonSerialization()
                .AddReporting(
                    factory =>
                    {
                        factory.AddInfluxDb(
                            new InfluxDBReporterSettings
                            {
                                InfluxDbSettings = new InfluxDBSettings("appmetricsreservoirs", InfluxDbUri),
                                ReportInterval = TimeSpan.FromSeconds(5)
                            });
                    })
                .AddMetricsMiddleware(options => options.HealthEndpointEnabled = false);
        }

        private static void RunRequestsToSample(CancellationToken token)
        {
            var scheduler = new DefaultTaskScheduler(); 
            var httpClient = new HttpClient
                             {
                                 BaseAddress = ApiBaseAddress
                             };

            Task.Run(() => scheduler.Interval(
                TimeSpan.FromMilliseconds(100),
                TaskCreationOptions.None,
                async () =>
                {
                    var uniform = httpClient.GetStringAsync("api/reservoirs/uniform");
                    var exponentiallyDecaying = httpClient.GetStringAsync("api/reservoirs/exponentially-decaying");
                    var slidingWindow = httpClient.GetStringAsync("api/reservoirs/sliding-window");
                    var hdr = httpClient.GetStringAsync("api/reservoirs/high-dynamic-range");

                    await Task.WhenAll(uniform, exponentiallyDecaying, slidingWindow, hdr);
                },
                token), token);
        }
    }
}