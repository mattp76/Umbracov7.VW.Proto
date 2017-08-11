using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SolisSearch.Configuration.ConfigurationElements
{
    public class Languages : ConfigurationElementCollection
    {
        public Language this[int index]
        {
            get
            {
                return (Language)this.BaseGet(index);
            }
            set
            {
                if (this.BaseGet(index) != null)
                    this.BaseRemoveAt(index);
                this.BaseAdd(index, (ConfigurationElement)value);
            }
        }

        public Language this[string Name]
        {
            get
            {
                return (Language)this.BaseGet((object)Name);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return (ConfigurationElement)new Language();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (object)((Language)element).Name;
        }
    }
}
