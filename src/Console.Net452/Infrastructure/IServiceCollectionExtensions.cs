using System;
using System.IO;
using System.Reflection;
using App.Metrics;
using App.Metrics.Extensions.Reporting.Console;
using App.Metrics.Extensions.Reporting.InfluxDB;
using App.Metrics.Extensions.Reporting.InfluxDB.Client;
using App.Metrics.Extensions.Reporting.TextFile;
using App.Metrics.Reporting.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Serilog;

// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection
    // ReSharper restore CheckNamespace
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureMetrics(this IServiceCollection services)
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName();
            services
                .AddMetrics(options =>
                {
                    options.ReportingEnabled = true;
                    options.GlobalTags.Add("env", "stage");
                }, assemblyName)
                .AddHealthChecks(factory =>
                {
                    factory.RegisterProcessPrivateMemorySizeHealthCheck("Private Memory Size", 200);
                    factory.RegisterProcessVirtualMemorySizeHealthCheck("Virtual Memory Size", 200);
                    factory.RegisterProcessPhysicalMemoryHealthCheck("Working Set", 200);
                })
                .AddReporting(factory =>
                {
                    factory.AddConsole(new ConsoleReporterSettings
                    {
                        ReportInterval = TimeSpan.FromSeconds(5),
                    });

                    factory.AddTextFile(new TextFileReporterSettings
                    {
                        ReportInterval = TimeSpan.FromSeconds(30),
                        FileName = $@"C:\metrics\{assemblyName.Name}.txt"
                    });

                    var influxFilter = new DefaultMetricsFilter()
                        .WhereMetricTaggedWithKeyValue(new TagKeyValueFilter { { "reporter", "influxdb" } })
                        .WithHealthChecks(true)
                        .WithEnvironmentInfo(true);

                    factory.AddInfluxDb(new InfluxDBReporterSettings
                    {
                        HttpPolicy = new HttpPolicy
                        {
                            FailuresBeforeBackoff = 3,
                            BackoffPeriod = TimeSpan.FromSeconds(10),
                            Timeout = TimeSpan.FromSeconds(3)
                        },
                        InfluxDbSettings = new InfluxDBSettings("appmetricsnet452", new Uri("http://127.0.0.1:8086")),
                        ReportInterval = TimeSpan.FromSeconds(5)
                    }, influxFilter);
                });

            return services;
        }

        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            var env = PlatformServices.Default.Application;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.RollingFile(Path.Combine($@"C:\logs\{env.ApplicationName}", "log-{Date}.txt"))
                .CreateLogger();

            var loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole((l, s) => s == LogLevel.Trace);
            loggerFactory.AddSerilog(Log.Logger);

            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddLogging();

            return services;
        }
    }
}