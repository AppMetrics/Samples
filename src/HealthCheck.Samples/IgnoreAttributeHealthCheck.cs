using System;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Health;

namespace HealthCheck.Samples
{
    [Obsolete]
    public class IgnoreAttributeHealthCheck : App.Metrics.Health.HealthCheck
    {
        public IgnoreAttributeHealthCheck() : base("Referencing Assembly - Sample Ignore")
        {
        }

        protected override ValueTask<HealthCheckResult> CheckAsync(CancellationToken token = default)
        {
            return new ValueTask<HealthCheckResult>(HealthCheckResult.Healthy("OK"));
        }
    }
}