using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tribal.umbraco7.vw.webapp.Enums;
using tribal.umbraco7.vw.webapp.Extensions;
using Umbraco.Core.Models;
using tribal.umbraco7.vw.webapp.Interfaces;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace tribal.umbraco7.vw.webapp.Helpers
{
    public class VWHelper : IVWHelper
    {

        public IEnumerable<string> GetLeadTypes(IPublishedContent c)
        {

            string[] vals = c.CheckNodePropertyAlias("hidden").Split(',').Select(p => p.Trim()).ToArray();
            var res = GetLeadTypeNames().Except(vals ?? new string[] { });

            return res;
            
        }


        public string[] GetLeadTypeNames()
        {
           return new string[] { LeadType.Brocure.Description(), LeadType.TestDrive.Description(), LeadType.Callback.Description() };
        }


        public string[] GetBrand(IPublishedContent c)
        {

            string[] vals = new[] { c.CheckNodePropertyAlias("brand") };
            return vals;

        }

        public HttpResponseMessage JsonResponse(JToken token)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(token.ToString(Newtonsoft.Json.Formatting.Indented), Encoding.UTF8, "application/json")
            };

            response.Headers.Add("Access-Control-Allow-Origin", "*");

            return response;
        }

        public JObject JsonResult(bool success = true, HttpStatusCode code = HttpStatusCode.OK, string trigger = "", string message = "", object data = null)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            result["success"] = success;
            result["code"] = code;
            if (trigger != null)
            {
                result["trigger"] = trigger;
            }
            if (message != null)
            {
                result["message"] = message;
            }
            if (data != null)
            {
                result["data"] = data;
            }
            return JObject.FromObject(result);
        }

        public JObject JsonResult(object data = null)
        {
            return JsonResult(true, HttpStatusCode.OK, null, "OK", data);
        }


        public JObject JsonResult(string message = "", object data = null)
        {
            return JsonResult(true, HttpStatusCode.OK, message, "OK", data);
        }

        public JObject JsonErrorResult(string trigger = "", string message = "", object data = null)
        {
            return JsonResult(false, HttpStatusCode.InternalServerError, trigger, message, data);
        }
    }
}