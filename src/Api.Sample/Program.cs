using System.IO;
using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.AspNetCore.Health;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;

namespace Api.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigureLogging();

            var host = new WebHostBuilder()
                .ConfigureMetricsWithDefaults(
                    builder =>
                    {
                        builder.Report.ToInfluxDb("http://127.0.0.1:8086", "appmetricsapi");
                    })
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseHealth()
                .UseMetrics()
                .UseSerilog()
                .Build();

            host.Run();
        }

        private static void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
                .WriteTo.LiterateConsole()
                .WriteTo.Seq("http://localhost:5341")
                .CreateLogger();
        }
    }
}
