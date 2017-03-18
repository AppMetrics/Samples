using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Api.ElasticSearch.Sample.ForTesting;
using App.Metrics.Extensions.Middleware.DependencyInjection.Options;
using App.Metrics.Infrastructure;
using App.Metrics.Reporting.Interfaces;
using App.Metrics.Scheduling;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using App.Metrics.Extensions.Reporting.ElasticSearch;
using App.Metrics.Extensions.Reporting.ElasticSearch.Client;

namespace Api.ElasticSearch.Sample
{
    public class Startup
    {
        public static readonly Uri ApiBaseAddress = new Uri("http://localhost:12346/");
        public static readonly string IndexName = "metrics";
        public static readonly Uri ElasticSearchUri = new Uri("http://127.0.0.1:9200");
        public static bool HaveAppRunSampleRequests = true;

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

            if (HaveAppRunSampleRequests)
            {
                RunSampleRequests(lifetime.ApplicationStopping);
            }

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
                .AddJsonSerialization()
                .AddReporting(
                    factory =>
                    {
                        factory.AddElasticSearch(
                            new ElasticSearchReporterSettings
                            {
                                HttpPolicy = new HttpPolicy
                                             {
                                                 FailuresBeforeBackoff = 3,
                                                 BackoffPeriod = TimeSpan.FromSeconds(30),
                                                 Timeout = TimeSpan.FromSeconds(3)
                                             },
                                ElasticSearchSettings = new ElasticSearchSettings(ElasticSearchUri, IndexName),
                                ReportInterval = TimeSpan.FromSeconds(5)
                            });
                    })
                .AddHealthChecks()
                .AddMetricsMiddleware(Configuration.GetSection("AspNetMetrics"));

            services.AddTransient<Func<double, RequestDurationForApdexTesting>>(
                provider => { return apdexTSeconds => new RequestDurationForApdexTesting(apdexTSeconds); });

            services.AddTransient<RequestErrorForErrorTesting>();

            services.AddTransient(
                provider =>
                {
                    var options = provider.GetRequiredService<AspNetMetricsOptions>();
                    return new RequestDurationForApdexTesting(options.ApdexTSeconds);
                });
        }

        private static void RunSampleRequests(CancellationToken token)
        {
            var scheduler = new DefaultTaskScheduler();
            var httpClient = new HttpClient
                             {
                                 BaseAddress = ApiBaseAddress
                             };
            return;
            Task.Run(
                () => scheduler.Interval(
                    TimeSpan.FromMilliseconds(200),
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

            Task.Run(
                () => scheduler.Interval(
                    TimeSpan.FromSeconds(1),
                    TaskCreationOptions.None,
                    async () =>
                    {
                        var satisfied = httpClient.GetAsync("api/error", token);

                        await Task.WhenAll(satisfied);
                    },
                    token),
                token);
        }
    }
}