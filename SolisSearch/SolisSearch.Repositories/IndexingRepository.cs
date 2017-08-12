using AutoMapper;
using HtmlAgilityPack;
using Microsoft.Practices.ServiceLocation;
using SolisSearch.Configuration;
using SolisSearch.Configuration.ConfigurationElements;
using SolisSearch.Entities;
using SolisSearch.Extensions;
using SolisSearch.Helpers;
using SolisSearch.Interfaces;
using SolisSearch.Log;
using SolisSearch.Mapping;
using SolisSearch.Parsers;
using SolrNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SolisSearch.Repositories
{
    public class IndexingRepository
    {
        private readonly ILogFacade log;
        private readonly ICmsIndexer cmsIndexer;

        public IndexingRepository(ICmsIndexer cmsIndexer, ILogFacade log)
        {
            this.cmsIndexer = cmsIndexer;
            this.log = log;

            AutoMapperWebConfiguration.Configure();
        }

        public void IndexItem(CmsSearchResultItem content)
        {
            var contentClone = Mapper.Map<CmsSearchResultItemClone>(content);

            if (ServiceLocator.Current.GetAllInstances<ISolrOperations<CmsSearchResultItem>>().Count() > 0)
            {
                ISolrOperations<CmsSearchResultItem> instance = ServiceLocator.Current.GetInstance<ISolrOperations<CmsSearchResultItem>>();
                instance.Add(content);
                instance.Commit();
            }

            if (ServiceLocator.Current.GetAllInstances<ISolrOperations<CmsSearchResultItemClone>>().Count() > 0)
            {
                ISolrOperations<CmsSearchResultItemClone> instanceClone = ServiceLocator.Current.GetInstance<ISolrOperations<CmsSearchResultItemClone>>();
                instanceClone.Add(contentClone);
                instanceClone.Commit();
            }

        }

        public void IndexNode(int id)
        {
            if (!CurrentConfiguration.ConfigurationExists)
            {
                this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Warn, "No Solis Search configuration found in web.config, skipping indexing.", (Exception)null);
            }
            else
            {
                ICmsContent contentById = this.cmsIndexer.GetContentById(id);
                string indexRoot = CurrentConfiguration.SearchSettings.IndexRoot;
                if (indexRoot != this.cmsIndexer.CmsRootId)
                {
                    this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("IndexRoot is set to {0}, checking if node is descendant.", (object)indexRoot), (Exception)null);
                    if (!((IEnumerable<string>)contentById.Path.Split(new string[1]
                    {
            ","
                    }, StringSplitOptions.RemoveEmptyEntries)).Contains<string>(this.cmsIndexer.CmsEntityFactory.ActualId(contentById.Id)))
                    {
                        this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("Node path {0} does not contain indexRoot {1}, node will not be indexed.", (object)contentById.Path, (object)indexRoot), (Exception)null);
                        return;
                    }
                }
                this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, "Publishing document \"" + contentById.Name + "\", adding to Solr index.", (Exception)null);
                CmsSearchResultItem searchResultItem = this.GetCmsSearchResultItem(contentById);
                if (searchResultItem == null)
                {
                    this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, "No indexitem to index", (Exception)null);
                }
                else
                {
                    this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, "Saving indexitem to index", (Exception)null);
                    this.IndexItem(searchResultItem);
                    if (!searchResultItem.Documents.Any<string>())
                        return;
                    foreach (string str in searchResultItem.Documents.Distinct<string>())
                    {
                        ICmsMedia cmsMedia = this.cmsIndexer.ResolveMedia(str);
                        if (cmsMedia != null)
                            this.IndexRichText(str, cmsMedia.Id.ToString((IFormatProvider)CultureInfo.InvariantCulture), searchResultItem.Acl, searchResultItem.Languages, searchResultItem.StartPublish, searchResultItem.EndPublish, searchResultItem.Id);
                    }
                }
            }
        }

        public void IndexRichText(string path, string id, ICollection<string> acl, ICollection<string> languages, DateTime startPublish, DateTime endPublish, string cmsContentId = null)
        {
            Path.GetFileName(path);
            try
            {
                ISolrOperations<CmsSearchResultItem> instance = ServiceLocator.Current.GetInstance<ISolrOperations<CmsSearchResultItem>>();
                using (Stream file = this.cmsIndexer.GetFile(path))
                {
                    ExtractParameters parameters = new ExtractParameters(file, "media" + id, path)
                    {
                        ExtractFormat = ExtractFormat.Text,
                        ExtractOnly = false,
                        Fields = (IEnumerable<ExtractField>)new ExtractField[3]
                      {
              new ExtractField("doctypes", "media"),
              new ExtractField("start_publish", startPublish.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")),
              new ExtractField("end_publish", endPublish.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"))
                      }
                    };
                    List<ExtractField> list = parameters.Fields.ToList<ExtractField>();
                    if (!string.IsNullOrEmpty(cmsContentId))
                        list.Add(new ExtractField("content_ref", cmsContentId));
                    foreach (string str in (IEnumerable<string>)acl)
                        list.Add(new ExtractField("acl", str));
                    foreach (string language in (IEnumerable<string>)languages)
                        list.Add(new ExtractField("lang", language));
                    parameters.Fields = (IEnumerable<ExtractField>)list;
                    instance.Extract(parameters);
                }
                instance.Commit();
            }
            catch (Exception ex)
            {
                this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Error, string.Format("Error indexing data from richtext document {0} with id {1}", (object)path, (object)id), ex);
            }
        }

        public int IndexItems(List<CmsSearchResultItem> items)
        {
            ISolrOperations<CmsSearchResultItem> instance = ServiceLocator.Current.GetInstance<ISolrOperations<CmsSearchResultItem>>();
            int int32 = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(items.Count) / new Decimal(100)));
            for (int index = 0; index < int32; ++index)
            {
                this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("Indexing items {0} to {1}", (object)(100 * index), (object)(100 * (index + 1))), (Exception)null);
                List<CmsSearchResultItem> list = items.Skip<CmsSearchResultItem>(100 * index).Take<CmsSearchResultItem>(100 * (index + 1)).ToList<CmsSearchResultItem>();
                instance.AddRange((IEnumerable<CmsSearchResultItem>)list);
                this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, "Indexing documents in collection", (Exception)null);
                foreach (CmsSearchResultItem searchResultItem in list.Where<CmsSearchResultItem>((Func<CmsSearchResultItem, bool>)(item => item.Documents.Any<string>())))
                {
                    foreach (string document in searchResultItem.Documents)
                    {
                        this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, "Finding media for document " + document, (Exception)null);
                        ICmsMedia cmsMedia = this.cmsIndexer.ResolveMedia(document);
                        if (cmsMedia != null)
                        {
                            this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, "Indexing media item " + cmsMedia.Name, (Exception)null);
                            this.IndexRichText(document, cmsMedia.Id, searchResultItem.Acl, searchResultItem.Languages, searchResultItem.StartPublish, searchResultItem.EndPublish, searchResultItem.Id);
                        }
                    }
                }
            }
            instance.Commit();
            instance.Optimize();
            return items.Count;
        }

        public void DeleteFromIndex(object id)
        {
            ISolrOperations<CmsSearchResultItem> instance = ServiceLocator.Current.GetInstance<ISolrOperations<CmsSearchResultItem>>();
            instance.Delete(id.ToString());
            instance.Commit();
        }

        public void DeleteMediaFromIndex(string mediaId)
        {
            ISolrOperations<CmsSearchResultItem> instance = ServiceLocator.Current.GetInstance<ISolrOperations<CmsSearchResultItem>>();
            instance.Delete(mediaId.StartsWith("media") ? mediaId.ToString((IFormatProvider)CultureInfo.InvariantCulture) : "media" + mediaId.ToString((IFormatProvider)CultureInfo.InvariantCulture));
            instance.Commit();
        }

        public void ClearIndex()
        {
            ISolrOperations<CmsSearchResultItem> instance = ServiceLocator.Current.GetInstance<ISolrOperations<CmsSearchResultItem>>();
            this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Info, "Clearing index", (Exception)null);
            instance.Delete((ISolrQuery)new SolrQuery("id:[* TO *]"));
            instance.Commit();
        }

        public void RebuildIndex()
        {
            if (!CurrentConfiguration.ConfigurationExists)
            {
                this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Warn, "No Solis Search configuration found in web.config, skipping indexing.", (Exception)null);
            }
            else
            {
                IList<ICmsContent> indexingRoot = this.cmsIndexer.GetIndexingRoot((object)CurrentConfiguration.SearchSettings.IndexRoot);
                if (indexingRoot.Any<ICmsContent>())
                {
                    List<CmsSearchResultItem> items = new List<CmsSearchResultItem>();
                    foreach (ICmsContent node in (IEnumerable<ICmsContent>)indexingRoot)
                    {
                        this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Info, string.Format("Rebuilding index from node {0}, id {1}.", (object)node.Name, (object)node.Id), (Exception)null);
                        if (node.Id != this.cmsIndexer.CmsRootId)
                        {
                            CmsSearchResultItem searchResultItem = this.GetCmsSearchResultItem(node);
                            if (searchResultItem != null)
                                items.Add(searchResultItem);
                        }
                        List<CmsSearchResultItem> list = this.cmsIndexer.GetDescendants(node).Select<ICmsContent, CmsSearchResultItem>(new Func<ICmsContent, CmsSearchResultItem>(this.GetCmsSearchResultItem)).ToList<CmsSearchResultItem>();
                        if (list.Any<CmsSearchResultItem>())
                            items.AddRange(list.Where<CmsSearchResultItem>((Func<CmsSearchResultItem, bool>)(i => i != null)));
                    }
                    this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Info, string.Format("Found total of {0} nodes to index", (object)items.Count), (Exception)null);
                    this.ClearIndex();
                    this.IndexItems(items);
                    this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, "Done, index rebuilt", (Exception)null);
                }
                else
                    this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Error, "No nodes to index found, is there no content or configuration error?", (Exception)null);
            }
        }

        public void BuildSpellcheckDictionary()
        {
            if (!CurrentConfiguration.ConfigurationExists || !CurrentConfiguration.SearchSettings.SpellCheck)
                return;
            this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Info, "Spellchecking is enabled, building SpellcheckDictionary", (Exception)null);
            ServiceLocator.Current.GetInstance<ISolrOperations<CmsSearchResultItem>>().BuildSpellCheckDictionary();
        }

        private CmsSearchResultItem GetCmsSearchResultItem(ICmsContent node)
        {
            if (node.Id == null)
            {
                this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, "Node id is null, not recognized object for indexing. Skipping.", (Exception)null);
                return (CmsSearchResultItem)null;
            }
            object propertyValue1 = this.cmsIndexer.GetPropertyValue(node, "solisSearchNoIndex");
            if (propertyValue1 != null)
            {
                try
                {
                    if (Convert.ToBoolean(propertyValue1))
                    {
                        this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, "solisSearchNoIndex is true, removing it from index if exist and skips node.", (Exception)null);
                        this.DeleteFromIndex((object)node.Id);
                        return (CmsSearchResultItem)null;
                    }
                }
                catch (Exception ex)
                {
                    this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Error, "Error checking if node should be indexed.", ex);
                }
            }
            DocType docType = CurrentConfiguration.DocTypes[node.ContentType];
            if (docType != null && docType.Exclude)
            {
                this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("DocType {0} is configured to be excluded from index, skipping", (object)node.ContentType), (Exception)null);
                return (CmsSearchResultItem)null;
            }
            if (docType == null)
            {
                this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Warn, string.Format("Cannot find any configuration for DocType {0}, loading default configuration", (object)node.ContentType), (Exception)null);
                docType = CurrentConfiguration.DocTypes["default"];
            }
            if (docType == null)
            {
                this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Warn, string.Format("Cannot find any configuration for DocType {0}, not even default backup configuration, cannot index content.", (object)node.ContentType), (Exception)null);
                return (CmsSearchResultItem)null;
            }
            CmsSearchResultItem searchResultItem = new CmsSearchResultItem()
            {
                Id = node.Id,
                Name = node.Name,
                LinkUrl = this.cmsIndexer.GetContentUrl(node),
                Created = node.CreateDate,
                StartPublish = node.StartPublish,
                LastModified = node.UpdateDate,
                EndPublish = node.EndPublish,
                Breadcrumbs = this.cmsIndexer.GetBreadcrumbs(node),
                Acl = this.cmsIndexer.GetNodeAcl(node),
                Languages = this.cmsIndexer.GetNodeLanguages(node),
                DocTypes = this.cmsIndexer.GetContentTypes(node)
            };
            try
            {
                object propertyValue2 = this.cmsIndexer.GetPropertyValue(node, "solisSearchCustomUrl");
                if (propertyValue2 != null)
                {
                    if (!string.IsNullOrWhiteSpace(propertyValue2 as string))
                    {
                        this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("Found custom value for node url, inserting to index {0}", propertyValue2), (Exception)null);
                        searchResultItem.LinkUrl = propertyValue2.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Error, "Error reading custom url", ex);
            }
            if (docType.AddPageNameToContent)
                searchResultItem.Content.Add(node.Name);
            foreach (Property property1 in docType.DocTypeProperties.Cast<Property>())
            {
                string propertyName = property1.PropertyName;
                ICmsProperty property2 = this.cmsIndexer.GetProperty(node, propertyName);
                if (property2 == null)
                {
                    this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("Property {0} not found on document, skipping", (object)propertyName), (Exception)null);
                }
                else
                {
                    IPropertyParser parser;
                    if (!string.IsNullOrEmpty(property1.Parser))
                    {
                        Type type = Type.GetType(property1.Parser);
                        if (type == (Type)null)
                        {
                            this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Error, string.Format("Parser was configured as {0}, but parser type is not recognized, will use default parser.", (object)property1.Parser), (Exception)null);
                            parser = (IPropertyParser)new DefaultParser();
                        }
                        else
                            parser = Activator.CreateInstance(type) as IPropertyParser;
                    }
                    else
                        parser = (IPropertyParser)new DefaultParser();
                    parser.CurrentCmsNode = node;
                    parser.CurrentCmsProperty = property2;
                    parser.CurrentSolisProperty = property1;
                    string str1 = property2 != null ? parser.GetPropertyValue(property2.Value) : string.Empty;
                    if (property1.Recursive && string.IsNullOrEmpty(str1))
                        str1 = this.GetRecursiveValue(node, propertyName, parser);
                    bool forceMultiValued = property1.ForceMultiValued;
                    string dynamicFieldExtension = FieldNameHelper.GetDynamicFieldExtension(property1);
                    if (!string.IsNullOrEmpty(str1))
                    {
                        if (property2 != null)
                        {
                            if (searchResultItem.Documents == null)
                                searchResultItem.Documents = this.ExtractDocumentLinks(str1, property2.Name);
                            else
                                searchResultItem.Documents.AddRange((IEnumerable<string>)this.ExtractDocumentLinks(str1, property2.Name));
                        }
                        if (property1.StripHtml)
                            str1 = str1.StripHtml();
                        if (property1.Content)
                        {
                            this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("Property is set as Content, adding content value from property {0} with value \"{1}\" to indexitem ", (object)propertyName, (object)str1), (Exception)null);
                            searchResultItem.Content.Add(str1);
                        }
                        string key = propertyName + dynamicFieldExtension;
                        this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("Adding property {0} with value \"{1}\" to indexitem ", (object)key, (object)str1), (Exception)null);
                        if (forceMultiValued)
                        {
                            char[] trimchars = new char[3]
                            {
                ' ',
                '\n',
                '\r'
                            };
                            IEnumerable<string> strings = ((IEnumerable<string>)Array.ConvertAll<string, string>(str1.Split(new string[1]
                            {
                property1.SplitChar
                            }, StringSplitOptions.RemoveEmptyEntries), (Converter<string, string>)(s => s.Trim(trimchars)))).AsEnumerable<string>();
                            searchResultItem.CmsProperties.Add(new KeyValuePair<string, object>(key, (object)strings));
                            if (property1.AlphabeticalIndex)
                            {
                                this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, "Multivalued property is set for alphabetical index, adding first letters to alphaindex", (Exception)null);
                                foreach (string str2 in strings)
                                {
                                    string lower = str2.Substring(0, 1).ToLower();
                                    if (!searchResultItem.AlphaIndex.Contains(lower))
                                        searchResultItem.AlphaIndex.Add(lower);
                                }
                            }
                        }
                        else
                        {
                            searchResultItem.CmsProperties.Add(new KeyValuePair<string, object>(key, (object)str1));
                            if (property1.AlphabeticalIndex)
                            {
                                this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, "Property is set for alphabetical index, adding first letter to alphaindex", (Exception)null);
                                string lower = str1.Substring(0, 1).ToLower();
                                if (!searchResultItem.AlphaIndex.Contains(lower))
                                    searchResultItem.AlphaIndex.Add(lower);
                            }
                        }
                    }
                }
            }
            return searchResultItem;
        }

        private string GetRecursiveValue(ICmsContent content, string propertyname, IPropertyParser parser)
        {
            this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("Property {0} is set as recursive, searching ancestors for node {1}", (object)propertyname, (object)content.Name), (Exception)null);
            string str = string.Empty;
            if (content.ParentId < 0)
                return str;
            for (ICmsContent cmsContent = this.cmsIndexer.GetContentById(content.ParentId); cmsContent != null && cmsContent.Id != this.cmsIndexer.CmsRootId; cmsContent = this.cmsIndexer.GetParent(cmsContent))
            {
                ICmsProperty property = this.cmsIndexer.GetProperty(cmsContent, propertyname);
                if (property != null && !string.IsNullOrEmpty(property.Value as string))
                {
                    str = parser.GetPropertyValue(property.Value);
                    this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("Found recursive value {0} for property {1} on node {2}", (object)str, (object)propertyname, (object)cmsContent.Name), (Exception)null);
                    break;
                }
            }
            return str;
        }

        private List<string> ExtractDocumentLinks(string propertyValue, string propertyAlias)
        {
            List<string> stringList = new List<string>();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(propertyValue);
            string[] strArray = CurrentConfiguration.SearchSettings.RichTextFileTypes.Split(new string[1]
            {
        ","
            }, StringSplitOptions.RemoveEmptyEntries);
            HtmlNodeCollection htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//a[@href]");
            if (htmlNodeCollection == null || htmlNodeCollection.Count() == 0)
            {
                this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("No href items found in property {0}", (object)propertyAlias), (Exception)null);
                return stringList;
            }
            this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("Found {0} href items in property {1}", (object)htmlNodeCollection.Count(), (object)propertyAlias), (Exception)null);
            using (IEnumerator<HtmlNode> enumerator = ((IEnumerable<HtmlNode>)htmlNodeCollection).GetEnumerator())
            {
                while (((IEnumerator)enumerator).MoveNext())
                {
                    HtmlNode current = enumerator.Current;
                    try
                    {
                        string str1 = current.Attributes["href"].Value;
                        string str2 = this.cmsIndexer.GetMediaFriendlyUrl(str1);
                        if (!string.IsNullOrEmpty(str2) && str2.Contains("?"))
                            str2 = str2.Substring(0, str2.IndexOf("?", StringComparison.Ordinal));
                        if (this.cmsIndexer.IsFileToIndex(str1, str2))
                        {
                            string str3 = Path.GetExtension(str2).Trim('.');
                            if (((IEnumerable<string>)strArray).Contains<string>(str3))
                            {
                                stringList.Add(str1);
                                this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Info, string.Format("Extracting RichText document links, found {0}", (object)str1), (Exception)null);
                            }
                            else
                                this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("File extension {0} is not configured as media type to index, skipping", (object)str3), (Exception)null);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Error, "Error resolving if media should be indexed for anchor " + current.InnerText, ex);
                    }
                }
            }
            return stringList;
        }
    }
}
