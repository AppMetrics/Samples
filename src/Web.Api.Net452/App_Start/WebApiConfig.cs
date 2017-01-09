using System.Web.Http;

namespace Web.Api.Net452
{
    public static class WebApiConfig
    {
        public static void RegisterWebApi(this HttpConfiguration httpConfiguration)
        {
            httpConfiguration.MapHttpAttributeRoutes();
        }
    }
}