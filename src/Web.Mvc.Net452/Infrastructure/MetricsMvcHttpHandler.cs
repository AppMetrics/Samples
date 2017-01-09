using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Web.Mvc.Net452.Infrastructure
{
    public class MetricsMvcHttpHandler : IHttpHandler
    {
        public MetricsMvcHttpHandler(RequestContext requestContext)
        {
            RequestContext = requestContext;
        }

        public bool IsReusable => true;

        public RequestContext RequestContext { get; set; }

        public void ProcessRequest(HttpContext context)
        {
            var controller = RequestContext.RouteData.GetRequiredString("controller");
            IController controllerInstance = null;
            IControllerFactory factory = null;

            try
            {
                factory = ControllerBuilder.Current.GetControllerFactory();
                controllerInstance = factory.CreateController(RequestContext, controller);

                var controllerName = RequestContext.RouteData.Values["controller"] as string;
                var actionName = RequestContext.RouteData.Values["action"] as string;

                context.GetOwinContext().Environment.Add("__Mertics.CurrentRouteName__",
                    $"{controllerName?.ToLowerInvariant()}/{actionName?.ToLowerInvariant()}");

                controllerInstance?.Execute(RequestContext);
            }
            finally
            {
                factory?.ReleaseController(controllerInstance);
            }
        }
    }
}