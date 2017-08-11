using SolisSearch.Configuration.ConfigurationElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolisSearch.Interfaces
{
    public interface IPropertyParser
    {
        ICmsContent CurrentCmsNode { get; set; }

        ICmsProperty CurrentCmsProperty { get; set; }

        Property CurrentSolisProperty { get; set; }

        string GetPropertyValue(object cmsPropertyValue);
    }
}
