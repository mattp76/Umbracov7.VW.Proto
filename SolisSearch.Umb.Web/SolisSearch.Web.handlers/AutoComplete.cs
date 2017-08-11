using SolisSearch.Log;
using SolisSearch.Repositories;
using SolisSearch.Umb.Log;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;

namespace SolisSearch.Umb.Web.handlers
{
    public class autocomplete : IHttpHandler
    {
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            List<string> stringList = new SearchRepository((ILogFacade)new LogFacade(typeof(SearchRepository))).AutoComplete(context.Request.QueryString["searchvalue"]);
            JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
            context.Response.Write(scriptSerializer.Serialize((object)stringList));
        }
    }
}
