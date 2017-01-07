using System.Web;
using System.Web.Mvc;

namespace Web.Mvc.Net452
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
