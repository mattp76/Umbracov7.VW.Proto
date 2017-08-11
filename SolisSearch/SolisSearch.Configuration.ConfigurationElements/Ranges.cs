using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SolisSearch.Configuration.ConfigurationElements
{
    public class Ranges : ConfigurationElementCollection
    {
        public FacetRange this[int index]
        {
            get
            {
                return (FacetRange)this.BaseGet(index);
            }
            set
            {
                if (this.BaseGet(index) != null)
                    this.BaseRemoveAt(index);
                this.BaseAdd(index, (ConfigurationElement)value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return (ConfigurationElement)new FacetRange();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (object)((FacetRange)element).Name;
        }
    }
}
