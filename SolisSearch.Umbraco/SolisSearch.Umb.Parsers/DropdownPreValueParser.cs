using SolisSearch.Configuration.ConfigurationElements;
using SolisSearch.Interfaces;
using umbraco;

namespace SolisSearch.Umb.Parsers
{
    public class DropdownPreValueParser : IPropertyParser
    {
        public ICmsContent CurrentCmsNode { get; set; }

        public ICmsProperty CurrentCmsProperty { get; set; }

        public Property CurrentSolisProperty { get; set; }

        public string GetPropertyValue(object cmsPropertyValue)
        {
            int result;
            if (int.TryParse(cmsPropertyValue.ToString(), out result))
                return library.GetPreValueAsString(result);
            return cmsPropertyValue.ToString();
        }
    }
}
