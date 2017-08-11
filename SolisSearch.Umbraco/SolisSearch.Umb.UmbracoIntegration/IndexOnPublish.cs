using SolisSearch.Configuration;
using SolisSearch.Interfaces;
using SolisSearch.Licensing;
using SolisSearch.Log;
using SolisSearch.Repositories;
using SolisSearch.Solr;
using SolisSearch.Umb.CmsEntities;
using SolisSearch.Umb.Log;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.web;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Models.EntityBase;
using Umbraco.Core.Publishing;
using Umbraco.Core.Services;
using umbraco.interfaces;


namespace SolisSearch.Umb.UmbracoIntegration
{
    public class IndexOnPublish : IApplicationEventHandler
    {
        private readonly LogFacade log = new LogFacade(typeof(IndexOnPublish));

        public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }

        public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            if (!CurrentConfiguration.ConfigurationExists)
            {
                this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Error, "No SolisSearch configuration found, indexing is disabled.", (Exception)null);
            }
            else
            {
                Initializer.InitSolr(CurrentConfiguration.SolrServer.Address);
                Initializer.InitSolr(CurrentConfiguration.SolrServer.Address);



                //https://github.com/SolrNet/SolrNet/blob/master/Documentation/Multi-core-instance.md
                string appSetting = ConfigurationManager.AppSettings["umbracoConfigurationStatus"];
                if (!string.IsNullOrWhiteSpace(appSetting))
                {
                    int int32 = Convert.ToInt32(appSetting.Substring(0, 1));
                    this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("Detected major version {0} of Umbraco, registering events", (object)int32), (Exception)null);
                    if (int32 >= 7)
                    {
                        this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, "Registering ContentServiceOnPublished to ContentService.Published event", (Exception)null);
                        // ISSUE: method pointer
                        ContentService.Published += ContentServiceOnPublished;
                    }
                    else
                    {
                        this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, "Registering DocumentOnAfterPublish to Document.AfterPublish event", (Exception)null);
                        // ISSUE: method pointer
                        Document.AfterPublish += DocumentOnAfterPublish;

                    }
                }
                else
                    this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Error, "Cannot detect umbraco version, check umbracoConfigurationStatus in web.config.", (Exception)null);
                this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, "Registering PublishingStrategyOnUnPublished to PublishingStrategy.UnPublished event", (Exception)null);
                // ISSUE: method pointer

                Umbraco.Core.Publishing.PublishingStrategy.UnPublished += PublishingStrategyOnUnPublished;

                this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, "Registering MediaServiceDeleted to MediaService.Deleted event", (Exception)null);
                // ISSUE: method pointer
                MediaService.Deleted += MediaServiceDeleted;

                ((HttpApplication)umbracoApplication).EndRequest += new EventHandler(this.UmbracoApplicationOnEndRequest);
            }
        }

        private void UmbracoApplicationOnEndRequest(object sender, EventArgs eventArgs)
        {
            this.CheckLicense(sender);
        }

        private void CheckLicense(object sender)
        {
            if (License.IsValidated || !CurrentConfiguration.ConfigurationExists || License.IsValid(CurrentConfiguration.SolrServer.Username, CurrentConfiguration.SolrServer.LicenseKey))
                return;
            HttpApplication httpApplication = (HttpApplication)sender;
            string extension = Path.GetExtension(httpApplication.Request.Path);
            if (httpApplication.Request.Url.PathAndQuery.ToLower().StartsWith("/umbraco") || httpApplication.Request.Url.PathAndQuery.ToLower().StartsWith("/install") || !(extension == ".aspx") && !string.IsNullOrEmpty(extension))
                return;
            httpApplication.Response.Output.WriteLine("<script> document.title =  document.title + ' - Unregistered SolisSearch - Not for production use'; document.body.innerHTML +='<div style=\"position:fixed;right: 40px;bottom:0; width:200px;text-align:center;height:30px;opacity:0.3;z-index:100;background:#000;color:#fff;\">Solis Search - Unregistered</div>';</script>");
        }

        private void MediaServiceDeleted(IMediaService sender, DeleteEventArgs<IMedia> e)
        {
            IndexingRepository indexingRepository = new IndexingRepository((ICmsIndexer)new UmbracoIndexer(), (ILogFacade)new LogFacade(typeof(IndexingRepository)));
            using (IEnumerator<IMedia> enumerator = e.DeletedEntities.GetEnumerator())
            {
                while (((IEnumerator)enumerator).MoveNext())
                {
                    IMedia current = enumerator.Current;
                    try
                    {
                        this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, "Deleted media item \"" + ((IUmbracoEntity)current).Name + "\", removing from Solr index.", (Exception)null);
                        indexingRepository.DeleteMediaFromIndex(((IEntity)current).Id.ToString((IFormatProvider)CultureInfo.InvariantCulture));
                    }
                    catch (Exception ex)
                    {
                        this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Error, "Error removing media item " + (object)((IEntity)current).Id + " from index, are you sure Solr is running?", ex);
                    }
                }
            }
        }

        private void ContentServiceOnPublished(IPublishingStrategy sender, PublishEventArgs<IContent> publishEventArgs)
        {
            if (!publishEventArgs.PublishedEntities.Any<IContent>())
                return;
            IndexingRepository indexingRepository = new IndexingRepository((ICmsIndexer)new UmbracoIndexer(), (ILogFacade)new LogFacade(typeof(IndexingRepository)));
            List<int> intList = new List<int>();
            using (IEnumerator<IContent> enumerator = publishEventArgs.PublishedEntities.GetEnumerator())
            {
                while (((IEnumerator)enumerator).MoveNext())
                {
                    IContent current = enumerator.Current;
                    indexingRepository.IndexNode(((IEntity)current).Id);
                }
            }
            indexingRepository.BuildSpellcheckDictionary();
        }

        private void DocumentOnAfterPublish(Document sender, PublishEventArgs publishEventArgs)
        {
            IndexingRepository indexingRepository = new IndexingRepository((ICmsIndexer)new UmbracoIndexer(), (ILogFacade)new LogFacade(typeof(IndexingRepository)));
            indexingRepository.IndexNode(((CMSNode)sender).Id);
            indexingRepository.BuildSpellcheckDictionary();
        }

        private void PublishingStrategyOnUnPublished(IPublishingStrategy sender, PublishEventArgs<IContent> publishEventArgs)
        {
            if (!publishEventArgs.PublishedEntities.Any<IContent>())
                return;
            IndexingRepository indexingRepository = new IndexingRepository((ICmsIndexer)new UmbracoIndexer(), (ILogFacade)new LogFacade(typeof(IndexingRepository)));
            using (IEnumerator<IContent> enumerator1 = publishEventArgs.PublishedEntities.GetEnumerator())
            {
                while (((IEnumerator)enumerator1).MoveNext())
                {
                    IContent current1 = enumerator1.Current;
                    try
                    {
                        using (IEnumerator<PropertyType> enumerator2 = ((IContentBase)current1).PropertyTypes.GetEnumerator())
                        {
                            while (((IEnumerator)enumerator2).MoveNext())
                            {
                                PropertyType current2 = enumerator2.Current;
                                try
                                {
                                    if (current2.DataTypeDefinitionId == -90)
                                    {
                                        this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("{0} was detected as upload property, will remove file from index", (object)current2.Alias), (Exception)null);
                                        Property property = ((KeyedCollection<string, Property>)((IContentBase)current1).Properties)[current2.Alias];
                                        if (property.Value != null && property.Value.ToString() != string.Empty)
                                        {
                                            string standaloneMediaId = MediaResolver.GetStandaloneMediaId(property.Value.ToString());
                                            if (!string.IsNullOrWhiteSpace(standaloneMediaId))
                                            {
                                                indexingRepository.DeleteMediaFromIndex("media" + standaloneMediaId);
                                                MediaResolver.RemoveStandAloneMedia(standaloneMediaId);
                                            }
                                        }
                                        else
                                            this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, "Property value is empty, nothing to remove", (Exception)null);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Error, "Error removing upload property media item from index.", ex);
                                }
                            }
                        }
                        this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, "Unpublishing document \"" + ((IUmbracoEntity)current1).Name + "\", removing from Solr index.", (Exception)null);
                        indexingRepository.DeleteFromIndex((object)((IEntity)current1).Id);
                    }
                    catch (Exception ex)
                    {
                        this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Error, "Error removing node " + (object)((IEntity)current1).Id + " from index, are you sure Solr is running?", ex);
                    }
                }
            }
        }

        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }
    }
}
