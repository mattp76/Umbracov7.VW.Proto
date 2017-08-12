using SolisSearch.Configuration;
using SolisSearch.Entities;
using SolrNet;
using System;
using System.Reflection;
using SolisSearch.Configuration.ConfigurationElements;

namespace SolisSearch.Solr
{
    public class Initializer
    {
        private static bool Initialized;

        public static void InitSolr(string serverUrl, Cores Cores)
        {
            if (Initializer.Initialized)
                return;

            foreach(Core core in Cores)
            {
                Type type = Type.GetType("SolisSearch.Entities." + core.Type);

                if (type != null) {
                    var solrNetStartupInit = typeof(Startup).GetMethod("Init", new[] { typeof(string) });
                    var startupInitRef = solrNetStartupInit.MakeGenericMethod(type);
                    startupInitRef.Invoke(null, new[] { serverUrl + core.Name });
                }
            }

            //Startup.Init<CmsSearchResultItem>(serverUrl);
            Initializer.Initialized = true;

        }

        public static T ConvertExamp1<T>(object input)
        {
            return (T)Convert.ChangeType(input, typeof(T));
        }
    }
}
