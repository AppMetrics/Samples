using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Health;

namespace HealthCheck.Samples
{
    public class SuccessHealthCheck : App.Metrics.Health.HealthCheck
    {
        public SuccessHealthCheck() : base("Referencing Assembly - Sample Healthy")
        {
        }

        protected override ValueTask<HealthCheckResult> CheckAsync(CancellationToken token = default)
        {
            return new ValueTask<HealthCheckResult>(HealthCheckResult.Healthy("OK"));
        }
    }
}