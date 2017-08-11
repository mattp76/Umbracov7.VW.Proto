using SolisSearch.Configuration;
using SolisSearch.Configuration.ConfigurationElements;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SolisSearch.Helpers
{
    public static class FieldNameHelper
    {
        public static string GetDynamicFieldName(string field, string doctype = null)
        {
            foreach (DocType docType in CurrentConfiguration.DocTypes.Cast<DocType>())
            {
                if (string.IsNullOrEmpty(doctype) || !(docType.Name != doctype))
                {
                    foreach (Property property in docType.DocTypeProperties.Cast<Property>())
                    {
                        if (property.Name == field || property.PropertyName == field)
                            return property.PropertyName + FieldNameHelper.GetDynamicFieldExtension(property);
                    }
                }
            }
            return field;
        }

        internal static string GetFacetField(string field)
        {
            List<string> source = new List<string>();
            foreach (DocType docType in CurrentConfiguration.DocTypes.Cast<DocType>())
            {
                foreach (Property property in docType.DocTypeProperties.Cast<Property>())
                {
                    if (property.PropertyName == field)
                        source.Add(field + FieldNameHelper.GetDynamicFieldExtension(property));
                }
            }
            if (source.Distinct<string>().Count<string>() > 1)
                throw new AmbiguousMatchException("The facet field " + field + " is configured as properties with different types, cannot resolve which property to use. Configure facet field explicit with both field name and dynamic extension, e.g bodyText_umbraco_t");
            if (source.Count > 0)
                field = source[0];
            return field;
        }

        internal static string GetDynamicFieldExtension(Property property)
        {
            string str = property.ForceMultiValued ? "_solis_multi_t" : "_solis_t";
            switch (property.Type)
            {
                case "int":
                    str = "_solis_i";
                    break;
                case "date":
                    str = "_solis_dt";
                    break;
                case "string":
                    str = property.ForceMultiValued ? "_solis_multi_s" : "_solis_s";
                    break;
            }
            return str;
        }
    }
}
