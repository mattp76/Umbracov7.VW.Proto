using Microsoft.Practices.ServiceLocation;
using SolisSearch.Entities;
using SolrNet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;


namespace SolisSearch.Repositories
{
    public class StatisticsRepository
    {
        public Statistics GetStatistics()
        {
            Statistics statistics = new Statistics();
            SolrQueryResults<CmsSearchResultItem> source = ServiceLocator.Current.GetInstance<ISolrOperations<CmsSearchResultItem>>().Query((ISolrQuery)new SolrQuery("*:*"));
            statistics.NumDocs = source.NumFound;
            foreach (string str in source.SelectMany<CmsSearchResultItem, string>((Func<CmsSearchResultItem, IEnumerable<string>>)(i => (IEnumerable<string>)i.ContentType)).Distinct<string>())
            {
                string contenType = str;
                int num = source.Count<CmsSearchResultItem>((Func<CmsSearchResultItem, bool>)(i =>
                {
                    if (i.ContentType != null)
                        return i.ContentType.Contains(contenType);
                    return false;
                }));
                statistics.RichTextDocuments.Add(contenType, num.ToString((IFormatProvider)CultureInfo.InvariantCulture));
            }
            return statistics;
        }
    }
}
