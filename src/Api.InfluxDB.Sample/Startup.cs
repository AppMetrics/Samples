using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Api.InfluxDB.Sample.ForTesting;
using App.Metrics.Extensions.Middleware.DependencyInjection.Options;
using App.Metrics.Extensions.Reporting.InfluxDB;
using App.Metrics.Extensions.Reporting.InfluxDB.Client;
using App.Metrics.Infrastructure;
using App.Metrics.Reporting.Interfaces;
using App.Metrics.Scheduling;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Api.InfluxDB.Sample
{
    public class Startup
    {
        public static readonly Uri ApiBaseAddress = new Uri("http://localhost:12345/");
        public static readonly string InfluxDbDatabase = "appmetricsinfluxsample";
        public static readonly Uri InfluxDbUri = new Uri("http://127.0.0.1:8086");

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
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

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

            services
                .AddMetrics(Configuration.GetSection("AppMetrics"), options => options.GlobalTags.Add("app", "sample app"))
                .AddClockType<TestClock>()
                .AddJsonSerialization()
                .AddReporting(
                    factory =>
                    {
                        factory.AddInfluxDb(
                            new InfluxDBReporterSettings
                            {
                                HttpPolicy = new HttpPolicy
                                             {
                                                 FailuresBeforeBackoff = 3,
                                                 BackoffPeriod = TimeSpan.FromSeconds(30),
                                                 Timeout = TimeSpan.FromSeconds(3)
                                             },
                                InfluxDbSettings = new InfluxDBSettings(InfluxDbDatabase, InfluxDbUri),
                                ReportInterval = TimeSpan.FromSeconds(5)
                            });
                    })
                .AddHealthChecks()
                .AddMetricsMiddleware(Configuration.GetSection("AspNetMetrics"));

            services.AddTransient<Func<double, RequestDurationForApdexTesting>>(
                provider => { return apdexTSeconds => new RequestDurationForApdexTesting(apdexTSeconds); });

            services.AddTransient(
                provider =>
                {
                    var options = provider.GetRequiredService<AspNetMetricsOptions>();
                    return new RequestDurationForApdexTesting(options.ApdexTSeconds);
                });
        }

        private static void RunRequestsToSample(CancellationToken token)
        {
            var scheduler = new DefaultTaskScheduler();
            var httpClient = new HttpClient
                             {
                                 BaseAddress = ApiBaseAddress
                             };

            Task.Run(
                () => scheduler.Interval(
                    TimeSpan.FromMilliseconds(100),
                    TaskCreationOptions.None,
                    async () =>
                    {
                        var satisfied = httpClient.GetAsync("api/apdexsatisfied", token);
                        var tolerating = httpClient.GetAsync("api/apdextolerating", token);
                        var frustrating = httpClient.GetAsync("api/apdexfrustrating", token);

                        await Task.WhenAll(satisfied, tolerating, frustrating);
                    },
                    token),
                token);
        }
    }
}