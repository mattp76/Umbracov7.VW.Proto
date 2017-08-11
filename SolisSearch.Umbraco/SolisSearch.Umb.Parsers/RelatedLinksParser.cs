using SolisSearch.Configuration;
using SolisSearch.Configuration.ConfigurationElements;
using SolisSearch.Interfaces;
using SolisSearch.Umb.Log;
using SolisSearch.Umb.UmbracoIntegration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using Umbraco.Core.Models;


namespace SolisSearch.Umb.Parsers
{
    internal class RelatedLinksParser : IPropertyParser
    {
        private readonly LogFacade log = new LogFacade(typeof(RelatedLinksParser));

        public ICmsContent CurrentCmsNode { get; set; }

        public ICmsProperty CurrentCmsProperty { get; set; }

        public Configuration.ConfigurationElements.Property CurrentSolisProperty { get; set; }

        public string GetPropertyValue(object cmsPropertyValue)
        {
            if (cmsPropertyValue == null)
                return string.Empty;
            string xml = cmsPropertyValue.ToString();
            if (string.IsNullOrEmpty(xml))
                return string.Empty;
            string[] strArray = CurrentConfiguration.SearchSettings.RichTextFileTypes.Split(new string[1]
            {
        ","
            }, StringSplitOptions.RemoveEmptyEntries);
            string empty = string.Empty;
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xml);
                XmlNodeList source1 = xmlDocument.SelectNodes("//link[@type='media']");
                if (source1 != null && source1.Count > 0)
                {
                    this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("Detected native related links property, found {0} media links in property", (object)source1.Count), (Exception)null);
                    foreach (XmlNode xmlNode in source1.Cast<XmlNode>())
                    {
                        IMedia media = MediaResolver.GetMedia(Convert.ToInt32(xmlNode.Attributes["link"].Value));
                        string str1 = xmlNode.Attributes["title"].Value;
                        object obj = ((KeyedCollection<string, Umbraco.Core.Models.Property>)((IContentBase)media).Properties)["umbracoFile"];
                        this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("Found media item with path {0} and title {1}", obj, (object)str1), (Exception)null);
                        string str2 = Path.GetExtension(obj.ToString()).Trim('.');
                        if (((IEnumerable<string>)strArray).Contains<string>(str2))
                            empty += string.Format("<a href=\"{0}\">{1}</a>", obj, (object)str1);
                        else
                            this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("File extension {0} is not to be index according to configuration, skipping", (object)str2), (Exception)null);
                    }
                }
                XmlNodeList source2 = xmlDocument.SelectNodes("//url-picker[@mode='Media']");
                if (source2 != null && source2.Count > 0)
                {
                    this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("Detected uComponents Multi-URL picker, found {0} media links in property", (object)source2.Count), (Exception)null);
                    foreach (XmlNode xmlNode in source2.Cast<XmlNode>())
                    {
                        string innerText = xmlNode.SelectSingleNode("./url").InnerText;
                        string str1 = xmlNode.SelectSingleNode("./link-title").InnerText;
                        if (string.IsNullOrEmpty(str1))
                            str1 = innerText;
                        this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("Found media item with path {0} and title {1}", (object)innerText, (object)str1), (Exception)null);
                        string str2 = Path.GetExtension(innerText).Trim('.');
                        if (((IEnumerable<string>)strArray).Contains<string>(str2))
                            empty += string.Format("<a href=\"{0}\">{1}</a>", (object)innerText, (object)str1);
                        else
                            this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("File extension {0} is not to be index according to configuration, skipping", (object)str2), (Exception)null);
                    }
                }
                XmlNodeList source3 = xmlDocument.SelectNodes("//DAMP/mediaItem");
                if (source3 != null)
                {
                    if (source3.Count > 0)
                    {
                        this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("Detected Digibiz Advanced media picker, found {0} media links in property", (object)source3.Count), (Exception)null);
                        foreach (XmlNode xmlNode in source3.Cast<XmlNode>())
                        {
                            string innerText = xmlNode.SelectSingleNode("./File/umbracoFile").InnerText;
                            string str1 = innerText;
                            this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("Found media item with path {0} and title {1}", (object)innerText, (object)str1), (Exception)null);
                            string str2 = Path.GetExtension(innerText).Trim('.');
                            if (((IEnumerable<string>)strArray).Contains<string>(str2))
                                empty += string.Format("<a href=\"{0}\">{1}</a>", (object)innerText, (object)str1);
                            else
                                this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("File extension {0} is not to be index according to configuration, skipping", (object)str2), (Exception)null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, "Related links property not xml format, trying to parse Media id:s from string", (Exception)null);
                string str1 = xml;
                char[] chArray = new char[1] { ',' };
                foreach (string str2 in str1.Split(chArray))
                {
                    int result;
                    if (int.TryParse(str2, out result))
                    {
                        IMedia media = MediaResolver.GetMedia(result);
                        if (media == null)
                        {
                            this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("Id {0} found, but is not detected as media item, skipping", (object)str2), (Exception)null);
                        }
                        else
                        {
                            object obj1 = ((KeyedCollection<string, Umbraco.Core.Models.Property>)((IContentBase)media).Properties)["umbracoFile"];
                            object obj2 = obj1;
                            this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("Found media item with path {0} and title {1}", obj1, obj2), (Exception)null);
                            string str3 = Path.GetExtension(obj1.ToString()).Trim('.');
                            if (((IEnumerable<string>)strArray).Contains<string>(str3))
                                empty += string.Format("<a href=\"{0}\">{1}</a>", obj1, obj2);
                            else
                                this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("File extension {0} is not to be index according to configuration, skipping", (object)str3), (Exception)null);
                        }
                    }
                    else if (str2.Length > 0 && str2.Contains("/media/"))
                    {
                        this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("Media item is not numeric, assuming path to media item {0}", (object)str2), (Exception)null);
                        IMedia imedia = MediaResolver.ResolveMedia(str2);
                        if (imedia != null)
                        {
                            object obj = ((KeyedCollection<string, Umbraco.Core.Models.Property>)((IContentBase)imedia).Properties)["umbracoFile"];
                            empty += string.Format("<a href=\"{0}\">{1}</a>", obj, obj);
                        }
                        else
                        {
                            this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("Media item not found in media library, assuming upload property type, file existence will be verified."), (Exception)null);
                            if (!str2.StartsWith("/media/") ? System.IO.File.Exists(str2) : System.IO.File.Exists(HttpContext.Current.Server.MapPath(str2)))
                                empty += string.Format("<a href=\"{0}\">{1}</a>", (object)str2, (object)str2);
                        }
                    }
                    else
                        this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("Cannot resolve value {0} to media id, skipping.", (object)str2), (Exception)null);
                }
            }
            return empty;
        }
    }
}
