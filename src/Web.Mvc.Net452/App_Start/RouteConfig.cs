using System.Web.Mvc;
using System.Web.Routing;

namespace Web.Mvc.Net452
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            var defaults = new RouteValueDictionary
                { { "controller", "Home" }, { "action", "Index" }, { "id", string.Empty } };

            var customRoute = new Route("{controller}/{action}/{id}", defaults, new MetricsMvcRouteHandler());
            routes.Add(customRoute);
        }
    }
}