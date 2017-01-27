using System;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using Microsoft.AspNetCore.Mvc;

namespace Reservoirs.Samples.Controllers
{
    [Route("api/[controller]")]
    public class ReservoirsController : Controller
    {
        private static readonly Random Rnd = new Random();

        private readonly IMetrics _metrics;

        public ReservoirsController(IMetrics metrics)
        {
            if (metrics == null)
            {
                throw new ArgumentNullException(nameof(metrics));
            }

            _metrics = metrics;
        }

        [HttpGet("exponentially-decaying")]
        public async Task<string> ExponentiallyDecaying()
        {
            using (_metrics.Measure.Timer.Time(MetricsRegistry.TimerUsingExponentialForwardDecayingReservoir))
            {
                await Delay();
            }

            return "OK";
        }

        [HttpGet("sliding-window")]
        public async Task<string> SlidingWindow()
        {
            using (_metrics.Measure.Timer.Time(MetricsRegistry.TimerUsingSlidingWindowReservoir))
            {
                await Delay();
            }

            return "OK";
        }

        [HttpGet("high-dynamic-range")]
        public async Task<string> HdrHistogram()
        {
            using (_metrics.Measure.Timer.Time(MetricsRegistry.TimerUsingHdrHistogramReservoir))
            {
                await Delay();
            }

            return "OK";
        }

        [HttpGet("uniform")]
        public async Task<string> Uniform()
        {
            using (_metrics.Measure.Timer.Time(MetricsRegistry.TimerUsingAlgorithmRReservoir))
            {
                await Delay();
            }

            return "OK";
        }

        private Task Delay()
        {
            return Task.Delay(DateTime.Now.Second, HttpContext.RequestAborted);
        }
    }
}