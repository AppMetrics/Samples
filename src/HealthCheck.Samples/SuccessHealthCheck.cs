using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Core;
using App.Metrics.Health;

namespace HealthCheck.Samples
{
    public class SuccessHealthCheck : App.Metrics.Health.HealthCheck
    {
        public SuccessHealthCheck() : base("Referencing Assembly - Sample Healthy")
        {
        }

        protected override Task<HealthCheckResult> CheckAsync(CancellationToken token = default(CancellationToken))
        {
            return Task.FromResult(HealthCheckResult.Healthy("OK"));
        }
    }
}