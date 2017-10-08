using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Health;

namespace Api.Sample.HealthChecks
{
    public class SampleHealthCheckUnHealthy : App.Metrics.Health.HealthCheck
    {
        public SampleHealthCheckUnHealthy() : base("Sample UnHealthy") { }

        protected override ValueTask<HealthCheckResult> CheckAsync(CancellationToken token = default(CancellationToken))
        {
            return new ValueTask<HealthCheckResult>(HealthCheckResult.Unhealthy("OOPS"));
        }
    }
}