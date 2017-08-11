using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace tribal.umbraco7.vw.webapp.Extensions
{
    public static class VWExtensions
    {
        public static string Description(this Enum value)
        {
            return GetDescription(value);
        }

        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr =
                           Attribute.GetCustomAttribute(field,
                             typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }


        public static bool Is_PRC_Hidden(this IPublishedContent c, string[] values)
        {
            if (c == null || values == null)
            {
                return false;
            }

            bool prc_hidden = ((new string[] { c.CheckNodePropertyAlias("prcHidden") }) ?? new string[] { }).Any(p => values.Any(v => string.Compare(v, p, true) == 0));
            return prc_hidden;
        }



        public static string Slug(this string phrase)
        {
            var str = phrase;

            if (!string.IsNullOrEmpty(phrase))
            {
                str = phrase.RemoveAccent().ToLower();
                // invalid chars           
                str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
                // convert multiple spaces into one space   
                str = Regex.Replace(str, @"\s+", " ").Trim();
                // cut and trim 
                str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
                str = Regex.Replace(str, @"\s", "-"); // hyphens
            }

            return str;
        }

        public static string RemoveAccent(this string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

    }
}