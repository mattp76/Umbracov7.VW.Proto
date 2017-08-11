using SolisSearch.Configuration;
using SolisSearch.Configuration.ConfigurationElements;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using umbraco;
using umbraco.cms.businesslogic.web;
using Umbraco.Core.Models;
using Umbraco.Core.Models.EntityBase;

namespace SolisSearch.Umb.Helpers
{
    internal class LanguageHelper
    {
        internal static List<string> GetNodeLanguages(IContent node)
        {
            if (CurrentConfiguration.ConfigurationExists && !CurrentConfiguration.SearchSettings.EnableLanguageSupport)
                return new List<string>();
            Domain[] currentDomains = library.GetCurrentDomains(((IEntity)node).Id);
            List<string> stringList = new List<string>();
            if (currentDomains != null)
            {
                foreach (Domain domain in currentDomains)
                {
                    if (domain.Language != null)
                    {
                        string str = domain.Language.CultureAlias;
                        if (!string.IsNullOrEmpty(str))
                        {
                            if (str.Length > 2)
                                str = str.Substring(0, 2);
                            stringList.Add(str.ToLower());
                        }
                    }
                }
            }
            else if (CurrentConfiguration.Languages.Count > 0)
            {
                string[] strArray = ((IUmbracoEntity)node).Path.Split(',');
                foreach (Configuration.ConfigurationElements.Language language in CurrentConfiguration.Languages.Cast<Configuration.ConfigurationElements.Language>())
                {
                    string str1 = language.IndexRoot.ToString((IFormatProvider)CultureInfo.InvariantCulture);
                    if (((IEnumerable<string>)strArray).Contains<string>(str1))
                    {
                        string str2 = language.Name;
                        if (!string.IsNullOrEmpty(str2))
                        {
                            if (str2.Length > 2)
                                str2 = str2.Substring(0, 2);
                            stringList.Add(str2.ToLower());
                        }
                    }
                }
            }
            return stringList;
        }
    }
}
