using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SolisSearch.Configuration.ConfigurationElements
{
    public class Cores : ConfigurationElementCollection
    {

        public Core this[string Name]
        {
            get
            {
                return (Core)this.BaseGet((object)Name);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return (ConfigurationElement)new Core();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (object)((Core)element).Name;
        }
    }
}
