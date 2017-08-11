using SolisSearch.Configuration;
using SolisSearch.Configuration.ConfigurationElements;
using SolisSearch.Interfaces;
using SolisSearch.Umb.Extensions;
using SolisSearch.Umb.Log;
using SolisSearch.Umb.UmbracoIntegration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml.Linq;
using umbraco;
using umbraco.cms.businesslogic.web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.EntityBase;
using Umbraco.Core.Services;

namespace SolisSearch.Umb.CmsEntities
{
    public class UmbracoIndexer : ICmsIndexer
    {
        private readonly LogFacade log = new LogFacade(typeof(UmbracoIndexer));

        public string CmsRootId
        {
            get
            {
                return "-1";
            }
        }

        public ICmsEntityFactory CmsEntityFactory { get; private set; }

        public UmbracoIndexer()
        {
            this.CmsEntityFactory = (ICmsEntityFactory)new SolisSearch.Umb.CmsEntities.CmsEntityFactory();
        }

        public ICmsProperty GetProperty(ICmsContent cmsNode, string propertyName)
        {
            Content nativeNodeObject = cmsNode.NativeNodeObject as Content;
            if (nativeNodeObject == null)
                return (ICmsProperty)null;
            if (!((ContentBase)nativeNodeObject).Properties.Contains(propertyName))
                return (ICmsProperty)null;


            return this.CmsEntityFactory.CreateCmsProperty((object)((KeyedCollection<string, Umbraco.Core.Models.Property>)nativeNodeObject.Properties)[propertyName]);
        }

        public object GetPropertyValue(ICmsContent cmsNode, string propertyName)
        {
            Content nativeNodeObject = cmsNode.NativeNodeObject as Content;
            if (nativeNodeObject == null)
                return (object)null;
            if (!((ContentBase)nativeNodeObject).Properties.Contains(propertyName))
                return (object)null;
            return ((ContentBase)nativeNodeObject).GetValue(propertyName);
        }

        public T GetPropertyValue<T>(ICmsContent cmsNode, string propertyName)
        {
            return ((ContentBase)(cmsNode.NativeNodeObject as Content)).GetValue<T>(propertyName);
        }

        public ICollection<string> GetNodeAcl(ICmsContent cmsNode)
        {
            IContent nativeNodeObject = cmsNode.NativeNodeObject as IContent;
            Collection<string> collection = new Collection<string>();
            string[] accessingMembershipRoles = Access.GetAccessingMembershipRoles(((IEntity)nativeNodeObject).Id, ((IUmbracoEntity)nativeNodeObject).Path);
            if (accessingMembershipRoles != null && ((IEnumerable<string>)accessingMembershipRoles).Any<string>())
            {
                foreach (string str in accessingMembershipRoles)
                    collection.Add(str);
            }
            else
                collection.Add("Everyone");
            return (ICollection<string>)collection;
        }

        public string GetContentUrl(ICmsContent cmsNode)
        {
            IContent nativeNodeObject = cmsNode.NativeNodeObject as IContent;
            string str1 = library.NiceUrl(((IEntity)nativeNodeObject).Id);
            if (str1 != "#")
                return str1;
            string str2 = library.NiceUrl(((IUmbracoEntity)nativeNodeObject).ParentId);
            string str3 = Umbraco.Core.Models.ContentExtensions.ToXml(nativeNodeObject).Attribute((XName)"urlName").Value;
            return !str2.EndsWith(".aspx") ? (str2.EndsWith("/") ? str2 + str3 : str2 + "/" + str3) : str2.Insert(str2.Length - 5, "/" + str3);
        }

        public ICollection<string> GetBreadcrumbs(ICmsContent cmsNode)
        {
            IContent nativeNodeObject = cmsNode.NativeNodeObject as IContent;
            Collection<string> source = new Collection<string>();
            if (((IUmbracoEntity)nativeNodeObject).ParentId < 0)
                return (ICollection<string>)source;
            for (IContent byId = ApplicationContext.Current.Services.ContentService.GetById(((IUmbracoEntity)nativeNodeObject).ParentId); byId != null; byId = ApplicationContext.Current.Services.ContentService.GetById(((IUmbracoEntity)byId).ParentId))
                source.Add(string.Format("{0};{1};{2}", (object)((IEntity)byId).Id, (object)((IUmbracoEntity)byId).Name, (object)library.NiceUrl(((IEntity)byId).Id)));
            return (ICollection<string>)source.Reverse<string>().ToList<string>();
        }

        public ICmsMedia ResolveMedia(string mediaUrl)
        {
            IMedia imedia = MediaResolver.ResolveMedia(mediaUrl);
            if (imedia != null)
                return this.CmsEntityFactory.CreateCmsMedia((object)imedia);
            this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, "Media item could not be resolved, checking stand alone register.", (Exception)null);
            string standaloneMediaId = MediaResolver.GetStandaloneMediaId(mediaUrl);
            return (ICmsMedia)new UmbracoMedia()
            {
                Id = standaloneMediaId
            };
        }

        public ICmsContent GetContentById(int id)
        {
            IContent byId = ApplicationContext.Current.Services.ContentService.GetById(id);
            if (byId == null)
                return (ICmsContent)null;
            return this.CmsEntityFactory.CreateCmsContent((object)byId);
        }

        public IList<ICmsContent> GetIndexingRoot(object rootIndex)
        {
            List<ICmsContent> cmsContentList = new List<ICmsContent>();
            int int32 = Convert.ToInt32(rootIndex);
            if (int32 > -1)
            {
                IContent byId = ApplicationContext.Current.Services.ContentService.GetById(int32);
                if (byId == null)
                    return (IList<ICmsContent>)cmsContentList;
                cmsContentList.Add(this.CmsEntityFactory.CreateCmsContent((object)byId));
            }
            else
            {
                IEnumerable<IContent> rootContent = ApplicationContext.Current.Services.ContentService.GetRootContent();
                cmsContentList.AddRange(((IEnumerable)rootContent).Cast<Content>().Select<Content, ICmsContent>(new Func<Content, ICmsContent>(this.CmsEntityFactory.CreateCmsContent)));
            }
            return (IList<ICmsContent>)cmsContentList;
        }

        public IEnumerable<ICmsContent> GetDescendants(ICmsContent node)
        {
            return (IEnumerable<ICmsContent>)EnumerableExtensions.WhereNotNull<ICmsContent>((IEnumerable<ICmsContent>)(node.NativeNodeObject as IContent).GetDescendants().Select<IContent, ICmsContent>(new Func<IContent, ICmsContent>(this.CmsEntityFactory.CreateCmsContent)));
        }

        public ICollection<string> GetNodeLanguages(ICmsContent node)
        {
            if (CurrentConfiguration.ConfigurationExists && !CurrentConfiguration.SearchSettings.EnableLanguageSupport)
                return (ICollection<string>)new List<string>();
            Domain[] currentDomains = library.GetCurrentDomains(Convert.ToInt32(node.Id));
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
                string[] strArray = node.Path.Split(',');
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
            return (ICollection<string>)stringList;
        }

        public ICollection<string> GetContentTypes(ICmsContent node)
        {
            IContent nativeNodeObject = node.NativeNodeObject as IContent;
            List<string> stringList = new List<string>();
            if (((IUmbracoEntity)nativeNodeObject.ContentType).Path.Length == 2)
            {
                stringList.Add(((IContentTypeBase)nativeNodeObject.ContentType).Alias);
            }
            else
            {
                IContentTypeService contentTypeService = ApplicationContext.Current.Services.ContentTypeService;
                string path = ((IUmbracoEntity)nativeNodeObject.ContentType).Path;
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
            return (ICollection<string>)stringList;
        }

        public ICmsContent GetParent(ICmsContent node)
        {
            IContent icontent = Umbraco.Core.Models.ContentExtensions.Parent(node.NativeNodeObject as IContent);
            if (icontent == null)
                return (ICmsContent)null;
            return this.CmsEntityFactory.CreateCmsContent((object)icontent);
        }

        public bool IsFileToIndex(string filename, string friendlyFilename)
        {
            bool flag = filename.Contains("/media/");
            this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("Should {0} be indexed: {1}", (object)filename, (object)flag), (Exception)null);
            return flag;
        }

        public string GetMediaFriendlyUrl(string input)
        {
            return input;
        }

        public Stream GetFile(string path)
        {
            if (!path.StartsWith("http"))
            {
                string host = HttpContext.Current.Request.Url.Host;
                int port = HttpContext.Current.Request.Url.Port;
                string str = (HttpContext.Current.Request.IsSecureConnection ? "https://" : "http://") + host;
                if (port != 80)
                    str = str + ":" + (object)port;
                path = str + path;
            }
            MemoryStream memoryStream;
            using (Stream responseStream = WebRequest.Create(path).GetResponse().GetResponseStream())
            {
                memoryStream = new MemoryStream();
                int count;
                do
                {
                    byte[] buffer = new byte[1024];
                    count = responseStream.Read(buffer, 0, 1024);
                    memoryStream.Write(buffer, 0, count);
                }
                while (responseStream.CanRead && count > 0);
                memoryStream.Position = 0L;
            }
            return (Stream)memoryStream;
        }
    }
}
