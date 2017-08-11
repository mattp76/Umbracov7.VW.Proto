<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SolisSearchControl.ascx.cs" Inherits="SolisSearch.Umb.Web.UserControls.Search.SolisSearchControl" %>
<%@ Import Namespace="SolisSearch.Configuration.ConfigurationElements" %>
<style>
    ul li { margin-bottom: 20px;}
    li p { margin-bottom: 5px;margin-top: 0;}
    .excluded { text-decoration: line-through; }
</style>
<h1>Solis Search</h1>
<% if (SolisSearchConfiguration != null)
   { %>
       
<ul>
    <li>Solr server: <%= SolisSearchConfiguration.SolrServer.Address %></li>
    <li><p><strong>Search settings:</strong></p>
        <p>Default field: <%= SolisSearchConfiguration.SearchSettings.DefaultField %></p>
        <p>Highlight: <%= SolisSearchConfiguration.SearchSettings.Highlight %></p>
        <p>Highlight fields: <%= SolisSearchConfiguration.SearchSettings.HighlightFields %></p>
        <p>Language support enabled: <%= SolisSearchConfiguration.SearchSettings.EnableLanguageSupport %></p>
        <% if (SolisSearchConfiguration.SearchSettings.EnableLanguageSupport)
           {  %>
            <p>Enabled languages: <%= SolisSearchConfiguration.SearchSettings.EnabledLanguages %> </p>
        <% } %>
        
        <p>Index root: <%= SolisSearchConfiguration.SearchSettings.IndexRoot.ToString() %></p>
    </li>
    <% if (Statistics != null)
       { %>
        <li>
            <p>Total docs: <%= Statistics.NumDocs %></p>
            <%= GetContentTypeStats() %>
        </li>
    <% } %>
    <li>
      <p><strong>DocTypes:</strong></p>
        <% foreach (var docType in SolisSearchConfiguration.DocTypes.Cast<DocType>())
           { %>
              <p <%= docType.Exclude ? "title=\"Excluded DocType\" class=\"excluded\"" : "" %> ><%= docType.Name %></p>
        <% } %>
    </li>
</ul>
    <asp:Button ID="btnRebuildIndex" runat="server" Text="Rebuild Index" OnClick="btnRebuildIndex_Click" />
<%} %>

<p>
    <asp:Label runat="server" ID="lblStatus"></asp:Label>
</p>
<asp:Panel ID="pnlViewConfig" runat="server">
    <asp:LinkButton ID="lbViewConfig" runat="server" OnClick="lbViewConfig_Click">View SolisSearch.config</asp:LinkButton>
    <div>
        <pre>
            <asp:Literal runat="server" ID="litRunningConfig"></asp:Literal>
        </pre>
    </div>
</asp:Panel>

