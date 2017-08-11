using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace tribal.umbraco7.vw.webapp.Extensions
{
    public static class IPublishedContentExtensions
    {
        public static string CheckNodePropertyAlias(this IPublishedContent node, string alias)
        {
            var str = string.Empty;

            if (node.GetProperty(alias) != null)
            {
                if (node.GetProperty(alias).HasValue)
                {
                    str = node.GetProperty(alias).Value.ToString();
                }
            }

            return str;
        }



        public static string[] CheckNodePropertyAliasArray(this IPublishedContent node, string alias)
        {
            var arr = new string[] { };

            if (node.GetProperty(alias) != null)
            {
                if (node.GetProperty(alias).HasValue)
                {
                    arr = node.GetPropertyValue<string[]>(alias).Where(x => x.Length > 0).ToArray();
                }
            }

            return arr;
        }

        public static bool CheckNodePropertyAliasBool(this IPublishedContent node, string alias)
        {
            var val = false;

            if (node.GetProperty(alias) != null)
            {
                if (node.GetProperty(alias).HasValue)
                {
                    val = (bool)node.GetProperty(alias).Value;
                }
            }

            return val;
        }
    }
}