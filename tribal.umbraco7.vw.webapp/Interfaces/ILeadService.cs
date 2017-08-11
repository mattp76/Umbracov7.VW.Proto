using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;

namespace tribal.umbraco7.vw.webapp.Interfaces
{
    public interface ILeadService
    {
        List<dynamic> GetVehicles(IPublishedContent VehicleNode);
    }
}