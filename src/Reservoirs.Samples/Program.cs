using System.IO;
using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.Filtering;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;

namespace Reservoirs.Samples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigureLogging();

            var filter = new MetricsFilter();
            filter.WhereContext(c => c == MetricsRegistry.Context);

            var host = new WebHostBuilder()
                .ConfigureMetricsWithDefaults(builder =>
                {
                    builder.Filter.With(filter);
                    builder.Report.ToInfluxDb("http://127.0.0.1:8086", "appmetricsreservoirs");
                })
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseMetrics()
                .Build();

            host.Run();
        }

        private static void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
                .WriteTo.Seq("http://localhost:5341")
                .CreateLogger();
        }
    }
}
