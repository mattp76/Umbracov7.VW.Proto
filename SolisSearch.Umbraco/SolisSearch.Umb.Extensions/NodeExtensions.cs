using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace SolisSearch.Umb.Extensions
{
    public static class NodeExtensions
    {
        internal static IEnumerable<IContent> GetDescendants(this IContent node)
        {

            var desc = ApplicationContext.Current.Services.ContentService.GetDescendants(node);

            return desc;

            // ISSUE: object of a compiler-generated type is created
            //return (IEnumerable<IContent>)GetDescendants(node);

        }
    }
}
