using System.Text.RegularExpressions;

namespace SolisSearch.Extensions
{
    public static class StringExtensions
    {
        public static string StripHtml(this string html)
        {
            return Regex.Replace(html, "<[^>]*>", string.Empty);
        }

        public static string StripSolrFields(this string query)
        {
            return Regex.Replace(query, "(\\w+(\\s*):)|AND|OR|(\\^\\w+)", string.Empty).Trim();
        }
    }
}
