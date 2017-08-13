using Microsoft.Practices.ServiceLocation;
using SolisSearch.Configuration;
using SolisSearch.Configuration.ConfigurationElements;
using SolisSearch.Entities;
using SolrNet;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SolisSearch.Helpers
{
    public static class InstanceHelper
    {
        public static bool IsInstance(dynamic item)
        {
            var test = item.GetType().Name;

            if (item.GetType().Name == "CmsSearchResultItem") {
                return (ServiceLocator.Current.GetAllInstances<ISolrOperations<CmsSearchResultItem>>().Count() > 0) ? true : false;
            }

            if (item.GetType().Name == "CmsSearchResultItemPublished")
            {
                return (ServiceLocator.Current.GetAllInstances<ISolrOperations<CmsSearchResultItemPublished>>().Count() > 0) ? true : false;
            }

            return false;
        }
    }
}
