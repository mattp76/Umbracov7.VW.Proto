using SolisSearch.Configuration;
using SolisSearch.Licensing;
using System;
using System.Configuration;
using System.IO;
using System.Web;

namespace SolisSearch.Umb.UmbracoIntegration
{
    public class SolisHttpModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.EndRequest += new EventHandler(this.context_EndRequest);
        }

        public void Dispose()
        {
        }

        private void context_EndRequest(object sender, EventArgs e)
        {
            this.CheckLicense(sender);
        }

        private void CheckLicense(object sender)
        {
            if (License.IsValidated || !CurrentConfiguration.ConfigurationExists || License.IsValid(CurrentConfiguration.SolrServer.Username, CurrentConfiguration.SolrServer.LicenseKey))
                return;
            HttpApplication httpApplication = (HttpApplication)sender;
            string extension = Path.GetExtension(httpApplication.Request.Path);
            string lower = httpApplication.Request.Url.PathAndQuery.ToLower();
            string str = (ConfigurationManager.AppSettings["umbracoPath"] ?? "/umbraco").ToLower().TrimStart('~').TrimEnd('/');
            if (httpApplication.Response.StatusCode >= 400 || !httpApplication.Response.ContentType.Contains("html") || (lower.StartsWith(str) || lower.StartsWith("/install")) || !(extension == ".aspx") && !string.IsNullOrEmpty(extension))
                return;
            httpApplication.Response.Output.WriteLine(License.UnlicensedJavascript);
        }
    }
}
