using System.Web;
using System.Web.Routing;
using Web.Mvc.Net452.Infrastructure;

namespace Web.Mvc.Net452
{
    public class MetricsMvcRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new MetricsMvcHttpHandler(requestContext);
        }
    }
}