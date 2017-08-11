using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SolisSearch.Configuration.ConfigurationElements
{
    public class Facets : ConfigurationElementCollection
    {
        public Facet this[int index]
        {
            get
            {
                return (Facet)this.BaseGet(index);
            }
            set
            {
                if (this.BaseGet(index) != null)
                    this.BaseRemoveAt(index);
                this.BaseAdd(index, (ConfigurationElement)value);
            }
        }

        public Facet this[string Name]
        {
            get
            {
                return (Facet)this.BaseGet((object)Name);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return (ConfigurationElement)new Facet();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (object)((Facet)element).Field;
        }
    }
}
