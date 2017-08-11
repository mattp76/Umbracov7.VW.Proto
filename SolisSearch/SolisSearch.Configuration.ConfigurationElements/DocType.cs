using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SolisSearch.Configuration.ConfigurationElements
{
    public class DocType : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = (object)value;
            }
        }

        [ConfigurationProperty("exclude", DefaultValue = false, IsRequired = false)]
        public bool Exclude
        {
            get
            {
                return (bool)this["exclude"];
            }
            set
            {
                this["exclude"] = (object)value;
            }
        }

        [ConfigurationProperty("addPageNameToContent", DefaultValue = false, IsRequired = false)]
        public bool AddPageNameToContent
        {
            get
            {
                return (bool)this["addPageNameToContent"];
            }
            set
            {
                this["addPageNameToContent"] = (object)value;
            }
        }

        [ConfigurationProperty("Properties", IsRequired = true)]
        [ConfigurationCollection(typeof(Properties), AddItemName = "Property", ClearItemsName = "clear", RemoveItemName = "remove")]
        public Properties DocTypeProperties
        {
            get
            {
                return (Properties)this["Properties"];
            }
            set
            {
                this["Properties"] = (object)value;
            }
        }
    }
}
