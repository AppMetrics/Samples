using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Extensions.Configuration;
using App.Metrics.Extensions.DependencyInjection;
using App.Metrics.Filtering;
using App.Metrics.Filters;
using App.Metrics.Health;
using App.Metrics.Reporting;
using App.Metrics.Scheduling;
using Metrics.Samples;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace App.Sample
{
    public class Host
    {
        private static IServiceProvider ServiceProvider { get; set; }

        private static IConfigurationRoot Configuration { get; set; }

        public static void Main()
        {
            var cpuUsage = new CpuUsage();
            cpuUsage.Start();

            Init();      

            var metrics = ServiceProvider.GetRequiredService<IMetricsRoot>();
            var reporter = ServiceProvider.GetRequiredService<IRunMetricsReports>();

            var process = Process.GetCurrentProcess();
            var simpleMetrics = new SampleMetrics(metrics);
            var setCounterSample = new SetCounterSample(metrics);
            var setMeterSample = new SetMeterSample(metrics);
            var userValueHistogramSample = new UserValueHistogramSample(metrics);
            var userValueTimerSample = new UserValueTimerSample(metrics);

            var scheduler = new AppMetricsTaskScheduler(TimeSpan.FromMilliseconds(300), () =>
                {
                    using (metrics.Measure.Apdex.Track(AppMetricsRegistry.ApdexScores.AppApdex))
                    {
                        setCounterSample.RunSomeRequests();
                        setMeterSample.RunSomeRequests();
                        userValueHistogramSample.RunSomeRequests();
                        userValueTimerSample.RunSomeRequests();
                        simpleMetrics.RunSomeRequests();
                    }

                    metrics.Measure.Gauge.SetValue(AppMetricsRegistry.Gauges.Errors, () => 1);
                    metrics.Measure.Gauge.SetValue(AppMetricsRegistry.Gauges.PercentGauge, () => 1);
                    metrics.Measure.Gauge.SetValue(AppMetricsRegistry.Gauges.ApmGauge, () => 1);
                    metrics.Measure.Gauge.SetValue(AppMetricsRegistry.Gauges.ParenthesisGauge, () => 1);
                    metrics.Measure.Gauge.SetValue(AppMetricsRegistry.Gauges.GaugeWithNoValue, () => double.NaN);

                    metrics.Measure.Gauge.SetValue(AppMetricsRegistry.ProcessMetrics.CpuUsageTotal, () =>
                    {
                        cpuUsage.CallCpu();
                        return cpuUsage.CpuUsageTotal;
                    });
                    metrics.Measure.Gauge.SetValue(AppMetricsRegistry.ProcessMetrics.ProcessPagedMemorySizeGauge, () => process.PagedMemorySize64);
                    metrics.Measure.Gauge.SetValue(AppMetricsRegistry.ProcessMetrics.ProcessPeekPagedMemorySizeGauge, () => process.PeakPagedMemorySize64);
                    metrics.Measure.Gauge.SetValue(AppMetricsRegistry.ProcessMetrics.ProcessPeekVirtualMemorySizeGauge,
                        () => process.PeakVirtualMemorySize64);
                    metrics.Measure.Gauge.SetValue(AppMetricsRegistry.ProcessMetrics.ProcessPeekWorkingSetSizeGauge, () => process.WorkingSet64);
                    metrics.Measure.Gauge.SetValue(AppMetricsRegistry.ProcessMetrics.ProcessPrivateMemorySizeGauge, () => process.PrivateMemorySize64);
                    metrics.Measure.Gauge.SetValue(AppMetricsRegistry.ProcessMetrics.ProcessVirtualMemorySizeGauge, () => process.VirtualMemorySize64);
                    metrics.Measure.Gauge.SetValue(AppMetricsRegistry.ProcessMetrics.SystemNonPagedMemoryGauge, () => process.NonpagedSystemMemorySize64);
                    metrics.Measure.Gauge.SetValue(AppMetricsRegistry.ProcessMetrics.SystemPagedMemorySizeGauge, () => process.PagedSystemMemorySize64);

                    return Task.CompletedTask;
                });

            scheduler.Start();

            var reportSchedule = new AppMetricsTaskScheduler(
                TimeSpan.FromSeconds(3),
                async () =>
                {
                    await Task.WhenAll(reporter.RunAllAsync());
                });
            reportSchedule.Start();

            Console.WriteLine("Report Cancelled...");

            Console.ReadKey();
        }

        private static void ConfigureMetrics(IServiceCollection services)
        {
            var influxFilter = new MetricsFilter()
                .WhereTaggedWithKeyValue(new TagKeyValueFilter {{"reporter", "influxdb"}});

            var metrics = new MetricsBuilder()
                .Configuration.Configure(options =>
                {
                    options.GlobalTags.Add("env", "stage");
                })
                .Configuration.ReadFrom(Configuration)
                .OutputEnvInfo.AsPlainText()
                .OutputMetrics.AsPlainText()
                .OutputMetrics.AsJson()
                .Report.ToInfluxDb(options =>
                {
                    options.InfluxDb.BaseUri = new Uri("http://127.0.0.1:8086");
                    options.InfluxDb.Database = "appmetrics";
                    options.FlushInterval = TimeSpan.FromSeconds(5);
                    options.Filter = influxFilter;
                })
                .Report.ToConsole(TimeSpan.FromSeconds(5))
                .Report.ToTextFile(options =>
                {
                    options.FlushInterval = TimeSpan.FromSeconds(30);
                    options.OutputPathAndFileName = @"C:\metrics\sample.txt";
                })
                .BuildAndAddTo(services);            
        }

        private static void Init()
        {
            var services = new ServiceCollection();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole()
                .WriteTo.Seq("http://localhost:5341")
                .CreateLogger();

            ConfigureMetrics(services);

            var health = AppMetricsHealth.CreateDefaultBuilder()
                .HealthChecks.AddCheck("DatabaseConnected",
                    () => new ValueTask<HealthCheckResult>(HealthCheckResult.Healthy("Database Connection OK")))
                .HealthChecks.AddProcessPrivateMemorySizeCheck("Private Memory Size", 200)
                .HealthChecks.AddProcessVirtualMemorySizeCheck("Virtual Memory Size", 200)
                .HealthChecks.AddProcessPhysicalMemoryCheck("Working Set", 200)
                .HealthChecks.AddCheck("DiskSpace", () =>
                {
                    var freeDiskSpace = GetFreeDiskSpace();

                    return new ValueTask<HealthCheckResult>(freeDiskSpace <= 512
                        ? HealthCheckResult.Unhealthy("Not enough disk space: {0}", freeDiskSpace)
                        : HealthCheckResult.Unhealthy("Disk space ok: {0}", freeDiskSpace));
                })
                .BuildAndAddTo(services);

            ServiceProvider = services.BuildServiceProvider();

            int GetFreeDiskSpace()
            {
                return 1024;
            }
        }
    }
}