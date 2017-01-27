using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Core;
using App.Metrics.Core.Options;
using App.Metrics.Gauge;
using Microsoft.AspNetCore.Mvc;

namespace Api.Sample.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private static readonly Random Rnd = new Random();

        private readonly IMetrics _metrics;

        public ValuesController(IMetrics metrics)
        {
            if (metrics == null)
            {
                throw new ArgumentNullException(nameof(metrics));
            }

            _metrics = metrics;
        }


        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var httpStatusMeter = new MeterOptions
            {
                Name = "Http Status",
                MeasurementUnit = Unit.Calls
            };

            _metrics.Measure.Meter.Mark(httpStatusMeter, 70, "200");
            _metrics.Measure.Meter.Mark(httpStatusMeter, 10, "500");
            _metrics.Measure.Meter.Mark(httpStatusMeter, 20, "401");

            _metrics.Measure.Counter.Increment(MetricsRegistry.Counters.TestCounter);
            _metrics.Measure.Counter.Increment(MetricsRegistry.Counters.TestCounter, 4);
            _metrics.Measure.Counter.Decrement(MetricsRegistry.Counters.TestCounter, 2);

            var process = Process.GetCurrentProcess();
            var physicalMemoryGauge = new FunctionGauge(() => process.WorkingSet64);

            _metrics.Measure.Gauge.SetValue(MetricsRegistry.Gauges.TestGauge, () => physicalMemoryGauge.Value);

            _metrics.Measure.Gauge.SetValue(MetricsRegistry.Gauges.DerivedGauge,
                () => new DerivedGauge(physicalMemoryGauge, g => g / 1024.0 / 1024.0));

            var cacheHits = _metrics.Provider.Meter.Instance(MetricsRegistry.Meters.CacheHits);
            var calls = _metrics.Provider.Timer.Instance(MetricsRegistry.Timers.DatabaseQueryTimer);

            var cacheHit = Rnd.Next(0, 2) == 0;
            if (cacheHit)
            {
                cacheHits.Mark();
            }

            using (calls.NewContext())
            {
                Thread.Sleep(cacheHit ? 10 : 100);
            }

            using (_metrics.Measure.Apdex.Track(MetricsRegistry.ApdexScores.TestApdex))
            {
                Thread.Sleep(cacheHit ? 10 : 100);
            }

            _metrics.Measure.Gauge.SetValue(MetricsRegistry.Gauges.CacheHitRatioGauge, () => new HitRatioGauge(cacheHits, calls, m => m.OneMinuteRate));

            var histogram = _metrics.Provider.Histogram.Instance(MetricsRegistry.Histograms.TestHAdvancedistogram);
            histogram.Update(Rnd.Next(1, 20));

            _metrics.Measure.Histogram.Update(MetricsRegistry.Histograms.TestHistogram, Rnd.Next(20, 40));

            _metrics.Measure.Timer.Time(MetricsRegistry.Timers.TestTimer, () => Thread.Sleep(20), "value1");
            _metrics.Measure.Timer.Time(MetricsRegistry.Timers.TestTimer, () => Thread.Sleep(25), "value2");

            using (_metrics.Measure.Timer.Time(MetricsRegistry.Timers.TestTimerTwo))
            {
                Thread.Sleep(15);
            }

            using (_metrics.Measure.Timer.Time(MetricsRegistry.Timers.TestTimerTwo, "value1"))
            {
                Thread.Sleep(20);
            }

            using (_metrics.Measure.Timer.Time(MetricsRegistry.Timers.TestTimerTwo, "value2"))
            {
                Thread.Sleep(25);
            }

            return new[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpGet("bad")]
        public IActionResult Get400()
        {
            return StatusCode(400);
        }

        [HttpGet("unauth")]
        public IActionResult Get401()
        {
            return StatusCode(401);
        }

        [HttpGet("slow")]
        public async Task<IActionResult> GetSlow()
        {
            await Task.Delay(Rnd.Next(500, 1200), HttpContext.RequestAborted);
            return StatusCode(200);
        }

        [HttpGet("error")]
        public IActionResult Get500()
        {
            return StatusCode(500);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }
    }
}