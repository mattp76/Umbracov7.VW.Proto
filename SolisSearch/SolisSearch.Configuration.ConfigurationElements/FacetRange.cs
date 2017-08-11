using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SolisSearch.Configuration.ConfigurationElements
{
    public class FacetRange : ConfigurationElement
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

        [ConfigurationProperty("start")]
        public string Start
        {
            get
            {
                return (string)this["start"];
            }
            set
            {
                this["start"] = (object)value;
            }
        }

        [ConfigurationProperty("end")]
        public string End
        {
            get
            {
                return (string)this["end"];
            }
            set
            {
                this["end"] = (object)value;
            }
        }

        [ConfigurationProperty("gap")]
        public string Gap
        {
            get
            {
                return (string)this["gap"];
            }
            set
            {
                this["gap"] = (object)value;
            }
        }

        [ConfigurationProperty("dataType", DefaultValue = "int")]
        public string DataType
        {
            get
            {
                return (string)this["dataType"];
            }
            set
            {
                this["dataType"] = (object)value;
            }
        }

        [ConfigurationProperty("dynamic")]
        public string Dynamic
        {
            get
            {
                return (string)this["dynamic"];
            }
            set
            {
                this["dynamic"] = (object)value;
            }
        }
    }
}
