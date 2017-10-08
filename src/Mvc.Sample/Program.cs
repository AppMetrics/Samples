using System.IO;
using App.Metrics.AspNetCore;
using App.Metrics.AspNetCore.Health;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace Mvc.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
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
    }
}
