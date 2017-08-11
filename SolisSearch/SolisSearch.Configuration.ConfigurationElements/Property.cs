using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SolisSearch.Configuration.ConfigurationElements
{
    public class Property : ConfigurationElement
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

        [ConfigurationProperty("property", IsRequired = true)]
        public string PropertyName
        {
            get
            {
                return (string)this["property"];
            }
            set
            {
                this["property"] = (object)value;
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

        [ConfigurationProperty("content", IsRequired = false)]
        public bool Content
        {
            get
            {
                return (bool)this["content"];
            }
            set
            {
                this["content"] = (object)value;
            }
        }

        [ConfigurationProperty("recursive", DefaultValue = false, IsRequired = false)]
        public bool Recursive
        {
            get
            {
                return (bool)this["recursive"];
            }
            set
            {
                this["recursive"] = (object)value;
            }
        }

        [ConfigurationProperty("alphabeticalindex", DefaultValue = false, IsRequired = false)]
        public bool AlphabeticalIndex
        {
            get
            {
                return (bool)this["alphabeticalindex"];
            }
            set
            {
                this["alphabeticalindex"] = (object)value;
            }
        }

        [ConfigurationProperty("forcemultivalued", DefaultValue = false, IsRequired = false)]
        public bool ForceMultiValued
        {
            get
            {
                return (bool)this["forcemultivalued"];
            }
            set
            {
                this["forcemultivalued"] = (object)value;
            }
        }

        [ConfigurationProperty("splitchar", DefaultValue = ",", IsRequired = false)]
        public string SplitChar
        {
            get
            {
                return (string)this["splitchar"];
            }
            set
            {
                this["splitchar"] = (object)value;
            }
        }

        [ConfigurationProperty("striphtml", DefaultValue = false, IsRequired = false)]
        public bool StripHtml
        {
            get
            {
                return (bool)this["striphtml"];
            }
            set
            {
                this["striphtml"] = (object)value;
            }
        }

        [ConfigurationProperty("parser", IsRequired = false)]
        public string Parser
        {
            get
            {
                return (string)this["parser"];
            }
            set
            {
                this["parser"] = (object)value;
            }
        }

        [ConfigurationProperty("boost", IsRequired = false)]
        public double Boost
        {
            get
            {
                return (double)this["boost"];
            }
            set
            {
                this["boost"] = (object)value;
            }
        }
    }
}
