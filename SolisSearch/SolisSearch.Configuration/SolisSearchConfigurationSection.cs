using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using SolisSearch.Configuration.ConfigurationElements;

namespace SolisSearch.Configuration
{
    public class SolisSearchConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("SolrServer")]
        public SolrServer SolrServer
        {
            get
            {
                return (SolrServer)this["SolrServer"];
            }
            set
            {
                this["SolrServer"] = (object)value;
            }
        }

        [ConfigurationProperty("SearchSettings")]
        public SearchSettings SearchSettings
        {
            get
            {
                return (SearchSettings)this["SearchSettings"];
            }
            set
            {
                this["SearchSettings"] = (object)value;
            }
        }

        [ConfigurationProperty("Cores")]
        [ConfigurationCollection(typeof(Cores), AddItemName = "Core", ClearItemsName = "clear", RemoveItemName = "remove")]
        public Cores Cores
        {
            get
            {
                return (Cores)this["Cores"];
            }
            set
            {
                this["Cores"] = (object)value;
            }
        }

        [ConfigurationProperty("DocTypes")]
        [ConfigurationCollection(typeof(DocTypes), AddItemName = "DocType", ClearItemsName = "clear", RemoveItemName = "remove")]
        public DocTypes DocTypes
        {
            get
            {
                return (DocTypes)this["DocTypes"];
            }
            set
            {
                this["DocTypes"] = (object)value;
            }
        }

        [ConfigurationCollection(typeof(Facet), AddItemName = "Facet", ClearItemsName = "clear", RemoveItemName = "remove")]
        [ConfigurationProperty("Facets")]
        public Facets Facets
        {
            get
            {
                return (Facets)this["Facets"];
            }
            set
            {
                this["Facets"] = (object)value;
            }
        }

        [ConfigurationProperty("Languages")]
        [ConfigurationCollection(typeof(Language), AddItemName = "Language", ClearItemsName = "clear", RemoveItemName = "remove")]
        public Languages Languages
        {
            get
            {
                return (Languages)this["Languages"];
            }
            set
            {
                this["Languages"] = (object)value;
            }
        }
    }
}

