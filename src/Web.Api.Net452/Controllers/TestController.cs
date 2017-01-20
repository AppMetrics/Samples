using System;
using System.Web.Http;
using App.Metrics;
using Web.Api.Net452.Infrastructure;

namespace Web.Api.Net452.Controllers
{
    [RoutePrefix("v1/test")]
    public class TestController : ApiController
    {
        private readonly IMetrics _metrics;

        public TestController(IMetrics metrics)
        {
            if (metrics == null)
            {
                throw new ArgumentNullException(nameof(metrics));
            }

            _metrics = metrics;
        }

        [Route("")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            _metrics.Measure.Counter.Increment(SampleMetrics.BasicCounter);

            return Ok("testing");
        }
    }
}