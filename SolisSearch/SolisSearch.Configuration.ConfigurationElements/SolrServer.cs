using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;


namespace SolisSearch.Configuration.ConfigurationElements
{
    public class SolrServer : ConfigurationElement
    {
        [ConfigurationProperty("address", DefaultValue = "http://localhost:8983/solr", IsRequired = true)]
        public string Address
        {
            get
            {
                return (string)this["address"];
            }
            set
            {
                this["address"] = (object)value;
            }
        }

        [ConfigurationProperty("username", DefaultValue = "", IsRequired = true)]
        public string Username
        {
            get
            {
                return (string)this["username"];
            }
            set
            {
                this["username"] = (object)value;
            }
        }

        [ConfigurationProperty("licenseKey", DefaultValue = "", IsRequired = true)]
        public string LicenseKey
        {
            get
            {
                return (string)this["licenseKey"];
            }
            set
            {
                this["licenseKey"] = (object)value;
            }
        }
    }
}
