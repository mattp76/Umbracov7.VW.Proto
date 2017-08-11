using SolisSearch.Entities;
using SolrNet;

namespace SolisSearch.Solr
{
    public class Initializer
    {
        private static bool Initialized;

        public static void InitSolr(string serverUrl)
        {
            if (Initializer.Initialized)
                return;
            Startup.Init<CmsSearchResultItem>(serverUrl);
            Initializer.Initialized = true;
        }
    }
}
