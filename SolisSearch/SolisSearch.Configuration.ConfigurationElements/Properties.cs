using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SolisSearch.Configuration.ConfigurationElements
{
    public class Properties : ConfigurationElementCollection
    {
        public Property this[int index]
        {
            get
            {
                return (Property)this.BaseGet(index);
            }
            set
            {
                if (this.BaseGet(index) != null)
                    this.BaseRemoveAt(index);
                this.BaseAdd(index, (ConfigurationElement)value);
            }
        }

        public Property this[string Name]
        {
            get
            {
                return (Property)this.BaseGet((object)Name);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return (ConfigurationElement)new Property();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (object)((Property)element).Name;
        }
    }
}
