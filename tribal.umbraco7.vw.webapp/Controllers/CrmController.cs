using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using tribal.umbraco7.vw.webapp.Interfaces;

namespace tribal.umbraco7.vw.webapp.Controllers
{
    
    public class CrmController : BaseApiController
    {
        private readonly ILeadService _leadService;
        private readonly IVWHelper _vwHelper;

        public CrmController(ILeadService leadService, IVWHelper vwHelper)
        {
            _leadService = leadService;
            _vwHelper = vwHelper;
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage Index()
        {
            var result = JObject.FromObject(new { message = "Please use /api/action for api calls." });
            var response = _vwHelper.JsonResponse(result);

            return response;
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage Vehicles()
        {
            try
            {
                var vehicles = _leadService.GetVehicles(VehicleNode);
                var data = new
                {
                    vehicles = vehicles
                };
                return _vwHelper.JsonResponse(_vwHelper.JsonResult(data));
            }
            catch (Exception e)
            {
                return _vwHelper.JsonResponse(_vwHelper.JsonErrorResult("ROVE00", e.Message));
            }
        }
    }
}