using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Health;

namespace Mvc.Sample.HealthChecks
{
    public class SampleHealthCheck : App.Metrics.Health.HealthCheck
    {
        public SampleHealthCheck() : base("Sample Healthy") { }

        protected override ValueTask<HealthCheckResult> CheckAsync(CancellationToken token = default)
        {
            return new ValueTask<HealthCheckResult>(HealthCheckResult.Healthy("OK"));
        }
    }
}