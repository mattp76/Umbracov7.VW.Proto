using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tribal.umbraco7.vw.webapp.Extensions;
using tribal.umbraco7.vw.webapp.Interfaces;
using Umbraco.Core.Models;
using Umbraco.Web;
using tribal.umbraco7.vw.webapp.Helpers;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace tribal.umbraco7.vw.webapp.Services
{
    public class PricingService : IPricingService
    {

        private readonly IVWHelper _vwhelper;
        string[] PRC_Hidden_Overview = new string[] { "Overview" };

        public PricingService(IVWHelper vwhelper)
        {
            _vwhelper = vwhelper;
        }


        public HttpResponseMessage OverView(IPublishedContent VehicleNode)
        {

            Func<List<string>, object> process_array = delegate (List<string> list)
            {
                return list.Distinct().OrderBy(g => g).Select(x => new { value = x.Slug(), title = x }).ToArray();
            };

            var children = VehicleNode.Children;
            var descendants = VehicleNode.Descendants().Where(x => x.DocumentTypeAlias.ToLower() == "vehicle" || x.DocumentTypeAlias.ToLower() == "variant" || x.DocumentTypeAlias.ToLower() == "subvariant");

            var vehicles = new List<object>();
            var filters = new List<object>();
            var vehicleGroups = new List<string>();
            var vehicleTransmissions = new List<string>();
            var vehicleWheelbases = new List<string>();
            var vehicleFuelTypes = new List<string>();

            var groups = new List<string>();
            var transmissions = new List<string>();
            var wheelbases = new List<string>();
            var fuelTypes = new List<string>();

            try
            {
                foreach (IPublishedContent c in descendants)
                {
                    //hide those with overview in prcHidden
                    bool prc_hidden = c.Is_PRC_Hidden(PRC_Hidden_Overview);

                    vehicleGroups.AddRange(!string.IsNullOrEmpty(c.CheckNodePropertyAlias("codeGroup")) ? new string[] { c.CheckNodePropertyAlias("codeGroup") } : new string[] { });
                    //transmissions.AddRange(((Element)subvariant["attribute.spec_transmission"]).TitleCase());
                    //wheelbases.AddRange(((Element)subvariant["attribute.spec_wheelbase"]).TitleCase());
                    //fuelTypes.AddRange(((Element)subvariant["attribute.spec_fuel_type"]).TitleCase());

                    if (!prc_hidden)
                    {
                        vehicles.Add(new
                        {
                           nid = c.CheckNodePropertyAlias("nid"),
                           uuid = c.CheckNodePropertyAlias("uuid"),
                           imageHero = c.CheckNodePropertyAlias("imageHero"),
                           imageThumb = c.CheckNodePropertyAlias("imageThumb"),
                           title = c.CheckNodePropertyAlias("title"),

                           //new tracking codes
                           carline_id = c.CheckNodePropertyAlias("carlineId"),
                           carline_name = c.CheckNodePropertyAlias("carlineName"),
                           sales_group_id = c.CheckNodePropertyAlias("salesGroupId"),
                           sales_group_name = c.CheckNodePropertyAlias("salesGroupName"),
                           equipmentline_id = c.CheckNodePropertyAlias("equipmentLineId"),
                           equipmentline_name = c.CheckNodePropertyAlias("equipmentLineName"),

                           code_model = c.CheckNodePropertyAlias("modelCode"),
                           group = process_array((!string.IsNullOrEmpty(c.CheckNodePropertyAlias("codeGroup")) ? new string[] { c.CheckNodePropertyAlias("codeGroup") } : new string[] { }).ToList()),
                           transmission = process_array(transmissions),
                           wheelbases = process_array(wheelbases),
                           fuel_type = process_array(fuelTypes),
                           visible = _vwhelper.GetLeadTypes(c),
                           brand = _vwhelper.GetBrand(c),
                           doc_type = c.DocumentTypeAlias,
                           disclaimer = c.CheckNodePropertyAlias("textDisclaimer")
                       });
                    }
                }


               // var content = new JObject();

                var data = new
                {
                    groups = process_array(vehicleGroups),
                    vehicles = vehicles
                    //filters = new
                    //{
                        //transmissions = process_array(vehicleTransmissions),
                        //wheelbases = process_array(vehicleWheelbases),
                        //fuel_type = process_array(vehicleFuelTypes),
                    //}
                    //content = content
                };

                return _vwhelper.JsonResponse(_vwhelper.JsonResult(data));
            }
            catch (Exception e)
            {
                return _vwhelper.JsonResponse(_vwhelper.JsonErrorResult("ROOV00", e.Message));
            }

            /*PopulateVehicles();

            Snapshot snapshot = VMS.VMS.Data;

            if (snapshot == null || snapshot.site == null || snapshot.site.vehicle == null)
            {
                throw new SourceException("LEGE01", "Snapshot data is not found.");
            }

            var snapshotVehicles = new List<object>();
            foreach (VehicleElement vehicle in snapshot.site.vehicle)
            {
                snapshotVehicles.Add(new
                {
                    nid = (string)vehicle["nid"],
                    uuid = (string)vehicle["uuid"],
                    consona_code = (string)vehicle["attribute.code_consona"],
                    model_code = (string)vehicle["attribute.code_model"],
                    name = (string)vehicle["title"],
                    brochure = GetBrochureUrl((string)vehicle["attribute.code_consona"] ?? ""),
                    image = ((string)vehicle["attribute.image_thumb"] ?? "").UseImageProxy(),
                    visible = LeadTypeNames.Except((string[])vehicle["attribute.sales_hidden"] ?? new string[] { }),
                    brand = (string[])vehicle["brand"] ?? new string[] { },
                    code_hdcc = (string)vehicle["attribute.code_hdcc"],
                });
                foreach (VehicleElement variant in vehicle.children)
                {
                    snapshotVehicles.Add(new
                    {
                        nid = (string)variant["nid"],
                        uuid = (string)variant["uuid"],
                        consona_code = (string)variant["attribute.code_consona"],
                        model_code = (string)variant["attribute.code_model"],
                        name = (string)variant["title"],
                        brochure = GetBrochureUrl((string)variant["attribute.code_consona"] ?? ""),
                        image = ((string)variant["attribute.image_thumb"]).UseImageProxy(),
                        visible = LeadTypeNames.Except((string[])variant["attribute.sales_hidden"] ?? new string[] { }),
                        brand = (string[])variant["brand"] ?? new string[] { },
                        code_hdcc = (string)variant["attribute.code_hdcc"],
                    });
                }
            }
            return snapshotVehicles;*/



        }
    }
}