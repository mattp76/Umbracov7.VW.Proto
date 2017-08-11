using SolisSearch.Configuration.ConfigurationElements;
using SolisSearch.Interfaces;
using System;

namespace SolisSearch.Parsers
{
    internal class DefaultParser : IPropertyParser
    {
        public ICmsContent CurrentCmsNode { get; set; }

        public ICmsProperty CurrentCmsProperty { get; set; }

        public Property CurrentSolisProperty { get; set; }

        public string GetPropertyValue(object cmsPropertyValue)
        {
            if (this.CurrentSolisProperty.Type == "date" && cmsPropertyValue != null)
            {
                string s = cmsPropertyValue as string;
                if (!string.IsNullOrEmpty(s))
                    s = DateTime.Parse(s).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                return s;
            }
            if (cmsPropertyValue == null)
                return string.Empty;
            return cmsPropertyValue.ToString();
        }
    }
}
