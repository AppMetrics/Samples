using System;
using System.Web.Mvc;
using App.Metrics;
using Web.Mvc.Net452.Infrastructure;

namespace Web.Mvc.Net452.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMetrics _metrics;

        public HomeController(IMetrics metrics)
        {
            if (metrics == null)
            {
                throw new ArgumentNullException(nameof(metrics));
            }

            _metrics = metrics;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Index()
        {
            _metrics.Increment(SampleMetrics.BasicCounter);

            return View();
        }
    }
}