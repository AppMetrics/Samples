using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Health;

namespace Api.Sample.HealthChecks
{
    public class SampleDegradedHealthCheck : App.Metrics.Health.HealthCheck
    {
        public SampleDegradedHealthCheck() : base("Sample Degraded")
        {
        }

        protected override ValueTask<HealthCheckResult> CheckAsync(CancellationToken token = default(CancellationToken))
        {
            return new ValueTask<HealthCheckResult>(HealthCheckResult.Degraded("Degraded"));
        }
    }
}