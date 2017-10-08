using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Health;

namespace HealthCheck.Samples
{
    public class HealthCheckUnHealthy : App.Metrics.Health.HealthCheck
    {
        public HealthCheckUnHealthy() : base("Referencing Assembly - Sample UnHealthy")
        {
        }

        protected override ValueTask<HealthCheckResult> CheckAsync(CancellationToken token = default)
        {
            return new ValueTask<HealthCheckResult>(HealthCheckResult.Unhealthy("OOPS"));
        }
    }
}