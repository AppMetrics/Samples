using System;
using System.Threading;
using App.Metrics;
using App.Metrics.Reporting.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Console.Net452
{
    public class Program
    {
        public static void Main()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.ConfigureServices();
            serviceCollection.ConfigureMetrics();

            var provider = serviceCollection.BuildServiceProvider();

            provider.ScheduleMetricReporting(cancellationTokenSource);

            System.Console.WriteLine("Running Reports...");

            System.Console.ReadKey();
        }
    }

    public class Application
    {
        public Application(IServiceProvider provider)
        {
            Metrics = provider.GetRequiredService<IMetrics>();

            var reporterFactory = provider.GetRequiredService<IReportFactory>();
            Reporter = reporterFactory.CreateReporter();

            Token = new CancellationToken();
        }

        public IMetrics Metrics { get; set; }

        public IReporter Reporter { get; set; }

        public CancellationToken Token { get; set; }
    }
}