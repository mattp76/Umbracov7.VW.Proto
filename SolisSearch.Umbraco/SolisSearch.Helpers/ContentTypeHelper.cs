using System;
using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.EntityBase;
using Umbraco.Core.Services;


namespace SolisSearch.Helpers
{
    internal class ContentTypeHelper
    {
        internal static List<string> GetContentTypes(IContent node)
        {
            List<string> stringList = new List<string>();
            if (((IUmbracoEntity)node.GetContentType()).Path.Length == 2)
            {
                stringList.Add(((IContentTypeBase)node.GetContentType()).Alias);
            }
            else
            {
                IContentTypeService contentTypeService = ApplicationContext.Current.Services.ContentTypeService;
                string path = ((IUmbracoEntity)node.GetContentType()).Path;
                char[] chArray = new char[1] { ',' };
                foreach (string str in path.Split(chArray))
                {
                    if (!(str == "-1"))
                    {
                        IContentType contentType = contentTypeService.GetContentType(Convert.ToInt32(str));
                        stringList.Add(((IContentTypeBase)contentType).Alias);
                    }
                }
            }
            return stringList;
        }
    }
}
