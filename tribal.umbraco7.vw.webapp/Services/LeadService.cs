using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tribal.umbraco7.vw.webapp.Extensions;
using tribal.umbraco7.vw.webapp.Interfaces;
using Umbraco.Core.Models;
using Umbraco.Web;
using tribal.umbraco7.vw.webapp.Helpers;

namespace tribal.umbraco7.vw.webapp.Services
{
    public class LeadService : ILeadService
    {

        private readonly IVWHelper _vwhelper;
        string[] PRC_Hidden_Overview = new string[] { "Overview" };

        public LeadService(IVWHelper vwhelper)
        {
            _vwhelper = vwhelper;
        }


        public List<dynamic> GetVehicles(IPublishedContent VehicleNode)
        {
            var umbracoVehicles = new List<object>();
            var children = VehicleNode.Children;
            var descendants = VehicleNode.Descendants().Where(x => x.DocumentTypeAlias.ToLower() == "vehicle" || x.DocumentTypeAlias.ToLower() == "variant" || x.DocumentTypeAlias.ToLower() == "subvariant");
  
            try
            {
                foreach (IPublishedContent c in descendants)
                {
                    //hide those with overview in prcHidden
                    bool prc_hidden = c.Is_PRC_Hidden(PRC_Hidden_Overview);

                    if (!prc_hidden)
                    {
                        umbracoVehicles.Add(new
                        {
                            nid = c.CheckNodePropertyAlias("nid"),
                            uuid = c.CheckNodePropertyAlias("uuid"),
                            consona_code = c.CheckNodePropertyAlias("consonaCode"),
                            model_code = c.CheckNodePropertyAlias("modelCode"),
                            name = c.CheckNodePropertyAlias("title"),
                            brochure = c.CheckNodePropertyAlias("brochure"),
                            //image = ((string)vehicle["attribute.image_thumb"] ?? "").UseImageProxy(),
                            imageHero = c.CheckNodePropertyAlias("imageHero"),
                            imageThumb = c.CheckNodePropertyAlias("imageThumb"),
                            visible = _vwhelper.GetLeadTypes(c),
                            brand = _vwhelper.GetBrand(c),
                            code_hdcc = c.CheckNodePropertyAlias("codeHDCC"),
                            doc_type = c.DocumentTypeAlias
                        });
                    }
                }
            }
            catch (Exception e)
            {

            }


            return umbracoVehicles;


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