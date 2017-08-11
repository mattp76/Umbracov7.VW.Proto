using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SolisSearch.Configuration.ConfigurationElements
{
    public class DocTypes : ConfigurationElementCollection
    {
        public DocType this[int index]
        {
            get
            {
                return (DocType)this.BaseGet(index);
            }
            set
            {
                if (this.BaseGet(index) != null)
                    this.BaseRemoveAt(index);
                this.BaseAdd(index, (ConfigurationElement)value);
            }
        }

        public DocType this[string Name]
        {
            get
            {
                return (DocType)this.BaseGet((object)Name);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return (ConfigurationElement)new DocType();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (object)((DocType)element).Name;
        }
    }
}
