using System.Web;
using System.Web.Mvc;

namespace tribal.umbraco7.vw.webapp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
