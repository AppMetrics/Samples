using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Health;

namespace Mvc.Sample.HealthChecks
{
    public class SampleHealthCheckUnHealthy : App.Metrics.Health.HealthCheck
    {
        public SampleHealthCheckUnHealthy() : base("Sample UnHealthy") { }

        protected override ValueTask<HealthCheckResult> CheckAsync(CancellationToken token = default)
        {
            return new ValueTask<HealthCheckResult>(HealthCheckResult.Unhealthy("OOPS"));
        }
    }
}