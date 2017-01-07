using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Scheduling;
using Console.Net452;

// ReSharper disable CheckNamespace
namespace System
// ReSharper restore CheckNamespace
{
    public static class IServiceProviderExtensions
    {
        public static void ScheduleMetricReporting(this IServiceProvider provider, CancellationTokenSource cancellationTokenSource)
        {
            var application = new Application(provider);
            var scheduler = new DefaultTaskScheduler();

            Task.Factory.StartNew(() =>
            {
                scheduler.Interval(
                    TimeSpan.FromMilliseconds(300), TaskCreationOptions.None, () =>
                    {
                        // Run Metrics
                    }, cancellationTokenSource.Token);
            });

            Task.Factory.StartNew(() =>
            {
                application.Reporter.RunReports(application.Metrics, cancellationTokenSource.Token);
            });
               
        }
    }
}