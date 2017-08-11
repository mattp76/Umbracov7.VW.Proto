using System;
using System.IO;
using System.Web;
using System.Xml;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace SolisSearch.Umb.UmbracoIntegration
{
    public class MediaResolver
    {
        public static IMedia ResolveMedia(string url)
        {
            int startIndex = url.IndexOf("/media/", StringComparison.CurrentCultureIgnoreCase);
            if (startIndex > 0)
                url = url.Substring(startIndex);
            return ApplicationContext.Current.Services.MediaService.GetMediaByPath(url);
        }

        public static IMedia GetMedia(int id)
        {
            return ApplicationContext.Current.Services.MediaService.GetById(id);
        }

        public static string GetStandaloneMediaId(string url)
        {
            XmlDocument xmlRegister = MediaResolver.EnsureStandaloneMediaRegister();
            XmlNode xmlNode = xmlRegister.SelectSingleNode("//files/file[@url='" + url + "']");
            if (xmlNode != null)
                return xmlNode.Attributes["id"].Value;
            XmlNode node = xmlRegister.CreateNode(XmlNodeType.Element, "file", string.Empty);
            XmlAttribute attribute1 = xmlRegister.CreateAttribute("url");
            attribute1.Value = url;
            XmlAttribute attribute2 = xmlRegister.CreateAttribute("id");
            attribute2.Value = Guid.NewGuid().ToString();
            node.Attributes.Append(attribute1);
            node.Attributes.Append(attribute2);
            xmlRegister.SelectSingleNode("//files").AppendChild(node);
            MediaResolver.SaveStandaloneFile(xmlRegister);
            return attribute2.Value;
        }

        public static void RemoveStandAloneMedia(string mediaId)
        {
            XmlDocument xmlRegister = MediaResolver.EnsureStandaloneMediaRegister();
            XmlNode oldChild = xmlRegister.SelectSingleNode("//files/file[@id='" + mediaId + "']");
            if (oldChild == null)
                return;
            oldChild.ParentNode.RemoveChild(oldChild);
            MediaResolver.SaveStandaloneFile(xmlRegister);
        }

        private static XmlDocument EnsureStandaloneMediaRegister()
        {
            string str = HttpContext.Current.Server.MapPath("/App_Data/SolisSearch/StandAloneMediaLibrary.xml");
            XmlDocument xmlDocument = new XmlDocument();
            if (System.IO.File.Exists(str))
            {
                try
                {
                    xmlDocument.Load(str);
                    return xmlDocument;
                }
                catch
                {
                }
            }
            xmlDocument.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><files />");
            return xmlDocument;
        }

        private static void SaveStandaloneFile(XmlDocument xmlRegister)
        {
            string str = HttpContext.Current.Server.MapPath("/App_Data/SolisSearch/");
            string filename = Path.Combine(str, "StandAloneMediaLibrary.xml");
            if (!Directory.Exists(str))
                Directory.CreateDirectory(str);
            xmlRegister.Save(filename);
        }
    }
}
