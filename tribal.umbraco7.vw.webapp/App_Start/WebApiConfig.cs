using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using tribal.umbraco7.vw.webapp.Formatters;

namespace tribal.umbraco7.vw.webapp
{
  public static class WebApiConfig
  {
    public static string ApiUrlPrefix { get { return "api"; } }
    public static string CoreUrlPrefix { get { return "core"; } }

    public static void Register(HttpConfiguration config)
    {

        config.Routes.MapHttpRoute(
            name: "DefaultApi",
            routeTemplate: WebApiConfig.ApiUrlPrefix + "/{controller}/{action}/{id}",
            defaults: new { controller = "Vehicle", action = "Index", id = RouteParameter.Optional }
        );

       config.Formatters.Add(new BrowserJsonFormatter());
    }
  }
}
