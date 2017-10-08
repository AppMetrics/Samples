using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Health;

namespace HealthCheck.Samples
{
    public class HealthCheckDegraded : App.Metrics.Health.HealthCheck
    {
        public HealthCheckDegraded() : base("Referencing Assembly - Sample Degraded") { }

        protected override ValueTask<HealthCheckResult> CheckAsync(CancellationToken token = default)
        {
            return new ValueTask<HealthCheckResult>(HealthCheckResult.Degraded());
        }
    }
}