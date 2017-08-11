using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SolisSearch.Configuration.ConfigurationElements
{
    public class Facet : ConfigurationElement
    {
        [ConfigurationProperty("field", IsKey = true, IsRequired = true)]
        public string Field
        {
            get
            {
                return (string)this["field"];
            }
            set
            {
                this["field"] = (object)value;
            }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get
            {
                return (string)this["type"];
            }
            set
            {
                this["type"] = (object)value;
            }
        }

        [ConfigurationProperty("mincount", DefaultValue = 0)]
        public int MinCount
        {
            get
            {
                return (int)this["mincount"];
            }
            set
            {
                this["mincount"] = (object)value;
            }
        }

        [ConfigurationProperty("limit", DefaultValue = "100")]
        public int Limit
        {
            get
            {
                return (int)this["limit"];
            }
            set
            {
                this["limit"] = (object)value;
            }
        }

        [ConfigurationProperty("sort")]
        public bool? Sort
        {
            get
            {
                return new bool?((bool)this["sort"]);
            }
            set
            {
                this["sort"] = (object)value;
            }
        }

        [ConfigurationProperty("Ranges")]
        [ConfigurationCollection(typeof(FacetRange), AddItemName = "FacetRange", ClearItemsName = "clear", RemoveItemName = "remove")]
        public Ranges Ranges
        {
            get
            {
                return (Ranges)this["Ranges"];
            }
            set
            {
                this["Ranges"] = (object)value;
            }
        }
    }
}
