using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolisSearch.Configuration.ConfigurationElements;
using System.Configuration;

namespace SolisSearch.Configuration
{
    public static class CurrentConfiguration
    {
        private static SolisSearchConfigurationSection solisSearchConfiguration;

        public static bool ConfigurationExists { get; private set; }

        public static SolisSearchConfigurationSection ActiveSolisSearchConfigurationSection
        {
            get
            {
                return CurrentConfiguration.solisSearchConfiguration;
            }
        }

        public static SearchSettings SearchSettings
        {
            get
            {
                return CurrentConfiguration.solisSearchConfiguration.SearchSettings;
            }
        }

        public static DocTypes DocTypes
        {
            get
            {
                return CurrentConfiguration.solisSearchConfiguration.DocTypes;
            }
        }

        public static Cores Cores
        {
            get
            {
                return CurrentConfiguration.solisSearchConfiguration.Cores;
            }
        }

        public static SolrServer SolrServer
        {
            get
            {
                return CurrentConfiguration.solisSearchConfiguration.SolrServer;
            }
        }

        public static Languages Languages
        {
            get
            {
                return CurrentConfiguration.solisSearchConfiguration.Languages;
            }
        }

        public static Facets Facets
        {
            get
            {
                return CurrentConfiguration.solisSearchConfiguration.Facets;
            }
        }

        static CurrentConfiguration()
        {
            SolisSearchConfigurationSection section = (SolisSearchConfigurationSection)ConfigurationManager.GetSection("SolisSearch");
            if (section != null)
            {
                CurrentConfiguration.ConfigurationExists = true;
                CurrentConfiguration.solisSearchConfiguration = section;
            }
            else
                CurrentConfiguration.ConfigurationExists = false;
        }
    }
}
