using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using Umbraco.Core.Models;

namespace tribal.umbraco7.vw.webapp.Interfaces
{
    public interface IVWHelper
    {
        IEnumerable<string> GetLeadTypes(IPublishedContent c);
        string[] GetLeadTypeNames();
        string[] GetBrand(IPublishedContent c);
        HttpResponseMessage JsonResponse(JToken token);
        JObject JsonResult(bool success = true, HttpStatusCode code = HttpStatusCode.OK, string trigger = "", string message = "", object data = null);
        JObject JsonResult(object data = null);
        JObject JsonResult(string message = "", object data = null);
        JObject JsonErrorResult(string trigger = "", string message = "", object data = null);
    }
}