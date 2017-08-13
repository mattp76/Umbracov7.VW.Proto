using SolisSearch.Configuration;
using SolisSearch.Entities;
using SolisSearch.Interfaces;
using SolisSearch.Log;
using SolisSearch.Repositories;
using SolisSearch.Umb.CmsEntities;
using SolisSearch.Umb.Log;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.BusinessLogic;
using System.Configuration;

namespace SolisSearch.Umb.Web.UserControls.Search
{
    public class SolisSearchControl : UserControl
    {
        private readonly LogFacade log = new LogFacade(typeof(SolisSearchControl));
        protected SolisSearchConfigurationSection SolisSearchConfiguration;
        protected Statistics Statistics;
        protected Button btnRebuildIndex;
        protected Label lblStatus;
        protected Panel pnlViewConfig;
        protected LinkButton lbViewConfig;
        protected Literal litRunningConfig;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!User.GetCurrent().IsAdmin() || !File.Exists(this.Server.MapPath("~\\config\\SolisSearch.config")))
                this.pnlViewConfig.Visible = false;
            if (CurrentConfiguration.ConfigurationExists)
                this.SolisSearchConfiguration = CurrentConfiguration.ActiveSolisSearchConfigurationSection;
            if (this.SolisSearchConfiguration == null)
            {
                this.lblStatus.Text = "No Solis Search configuration section found, check your web.config.";
            }
            else
            {
                if (this.Page.IsPostBack)
                    return;
                this.LoadStatistics();
            }
        }

        private void LoadStatistics()
        {
            try
            {
                this.Statistics = new StatisticsRepository().GetStatistics();
            }
            catch (SocketException ex)
            {
                this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Error, "Could not connect to Apache Solr instance, is the server running?", (Exception)ex);
                this.lblStatus.Text = "Could not connect to Apache Solr instance, is the server running?";
            }
            catch (Exception ex)
            {
                this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Error, "Unknown error occured", ex);
                this.lblStatus.Text = "Error: " + ex.Message;
            }
        }

        protected string GetContentTypeStats()
        {
            if (this.Statistics == null)
                return "<p>No content type statistics available.</p>";
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string allKey in this.Statistics.RichTextDocuments.AllKeys)
                stringBuilder.AppendFormat("<p>{0}: {1}</p>", (object)allKey, (object)this.Statistics.RichTextDocuments[allKey]);
            return stringBuilder.ToString();
        }

        protected void btnRebuildIndex_Click(object sender, EventArgs e)
        {
            IndexingRepository indexingRepository = new IndexingRepository((ICmsIndexer)new UmbracoIndexer(), (ILogFacade)new LogFacade(typeof(IndexingRepository)));
           
            try
            {
                DateTime now1 = DateTime.Now;
                indexingRepository.RebuildIndex();
                indexingRepository.BuildSpellcheckDictionary();
                DateTime now2 = DateTime.Now;
                this.LoadStatistics();
                this.lblStatus.Text = string.Format("Done, index rebuilt in {0} seconds.", (object)(now2 - now1).TotalSeconds.ToString("N2"));
            }
            catch (Exception ex)
            {
                this.lblStatus.Text = "An error occured, index not rebuilt. Errormessage: " + ex.Message;
                this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Error, "Error rebuilding index", ex);
            }
        }


        protected void btnClearIndex_Click(object sender, EventArgs e)
        {
            IndexingRepository indexingRepository = new IndexingRepository((ICmsIndexer)new UmbracoIndexer(), (ILogFacade)new LogFacade(typeof(IndexingRepository)));

            try
            {
                DateTime now1 = DateTime.Now;
                indexingRepository.ClearIndex();
                DateTime now2 = DateTime.Now;
                this.LoadStatistics();
                this.lblStatus.Text = string.Format("Done, index cleared in {0} seconds.", (object)(now2 - now1).TotalSeconds.ToString("N2"));
            }
            catch (Exception ex)
            {
                this.lblStatus.Text = "An error occured, index not cleared. Errormessage: " + ex.Message;
                this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Error, "Error rebuilding index", ex);
            }
        }

        protected void lbViewConfig_Click(object sender, EventArgs e)
        {
            if (!File.Exists(this.Server.MapPath("~\\config\\SolisSearch.config")))
                return;
            this.litRunningConfig.Text = HttpUtility.HtmlEncode(File.ReadAllText(this.Server.MapPath("~\\config\\SolisSearch.config")));
        }
    }
}
