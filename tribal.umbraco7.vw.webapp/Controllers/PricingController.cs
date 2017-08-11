using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using tribal.umbraco7.vw.webapp.Interfaces;

namespace tribal.umbraco7.vw.webapp.Controllers
{
    
    public class PricingController : BaseApiController
    {
        private readonly IPricingService _pricingService;
        private readonly IVWHelper _vwHelper;

        public PricingController(IPricingService pricingService, IVWHelper vwHelper)
        {
            _pricingService = pricingService;
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
        public HttpResponseMessage Overview()
        {
            try
            {
                var data = _pricingService.OverView(VehicleNode);

                return data;
            }
            catch (Exception e)
            {
                return _vwHelper.JsonResponse(_vwHelper.JsonErrorResult("ROVE00", e.Message));
            }
        }
    }
}