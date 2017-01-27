using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Health;

namespace Mvc.Sample.HealthChecks
{
    public class SampleHealthCheck : App.Metrics.Health.HealthCheck
    {
        public SampleHealthCheck() : base("Sample Healthy") { }

        protected override Task<HealthCheckResult> CheckAsync(CancellationToken token = default(CancellationToken))
        {
            return Task.FromResult(HealthCheckResult.Healthy("OK"));
        }
    }
}