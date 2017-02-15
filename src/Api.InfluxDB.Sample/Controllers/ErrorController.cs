using System;
using Api.InfluxDB.Sample.ForTesting;
using App.Metrics;
using Microsoft.AspNetCore.Mvc;

namespace Api.InfluxDB.Sample.Controllers
{
    [Route("api/[controller]")]
    public class ErrorController : Controller
    {
        private readonly RequestErrorForErrorTesting _requestErrorForErrorTesting;
        private readonly IMetrics _metrics;

        public ErrorController(IMetrics metrics, RequestErrorForErrorTesting requestErrorForErrorTesting)
        {
            if (metrics == null)
            {
                throw new ArgumentNullException(nameof(metrics));
            }

            _metrics = metrics;
            _requestErrorForErrorTesting = requestErrorForErrorTesting;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return StatusCode(_requestErrorForErrorTesting.NextStatusCode);
        }
    }
}
