using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using Umbraco.Core.Models;

namespace tribal.umbraco7.vw.webapp.Interfaces
{
    public interface IPricingService
    {
        HttpResponseMessage OverView(IPublishedContent VehicleNode);
    }
}