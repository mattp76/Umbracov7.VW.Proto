using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SolisSearch.Configuration.ConfigurationElements
{
    public class Language : ConfigurationElement
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

        [ConfigurationProperty("rootNode", IsRequired = true)]
        public int IndexRoot
        {
            get
            {
                return (int)this["rootNode"];
            }
            set
            {
                this["rootNode"] = (object)value;
            }
        }
    }
}
