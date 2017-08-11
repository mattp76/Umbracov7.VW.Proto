using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using umbraco.NodeFactory;
using Umbraco.Core.Models;
using Umbraco.Web.WebApi;

namespace tribal.umbraco7.vw.webapp.Controllers
{
    public class BaseApiController : UmbracoApiController
    {

        private IPublishedContent _currentNode;
        private IPublishedContent _rootNode;
        private IPublishedContent _vehicleNode;


        protected virtual IPublishedContent CurrentNode
        {
            get { return _currentNode ?? (_currentNode = Umbraco.TypedContent(UmbracoContext.PageId)); }
        }

        protected virtual IPublishedContent RootNode
        {
            get { return _rootNode ?? (_currentNode = Umbraco.TypedContent(ConfigurationManager.AppSettings["Root.id"])); }
        }

        protected virtual IPublishedContent VehicleNode
        {
            get { return _vehicleNode ?? (_currentNode = Umbraco.TypedContent(ConfigurationManager.AppSettings["Vehicles.id"])); }
        }

    }
}