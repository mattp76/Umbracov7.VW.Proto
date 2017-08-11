using Microsoft.Practices.ServiceLocation;
using SolisSearch.Configuration;
using SolisSearch.Configuration.ConfigurationElements;
using SolisSearch.Entities;
using SolisSearch.Extensions;
using SolisSearch.Helpers;
using SolisSearch.Log;
using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Xml;


namespace SolisSearch.Repositories
{
    public class SearchRepository
    {
        private readonly ILogFacade log;

        public SearchRepository(ILogFacade logger)
        {
            this.log = logger;
        }

        public SolrQueryResults<CmsSearchResultItem> SearchIndex(string query, string[] searchfilters, int page, int pageSize, string sortOrder, string language = null)
        {
            StandardQuery standardQuery = this.CreateStandardQuery(query, searchfilters, page, pageSize, sortOrder, language);
            return this.ExecuteQuery(standardQuery.LocalParams + (ISolrQuery)standardQuery.Query, standardQuery.QueryOptions);
        }

        public SolrQueryResults<CmsSearchResultItem> ExecuteQuery(ISolrQuery query, QueryOptions queryOptions)
        {
            return ServiceLocator.Current.GetInstance<ISolrOperations<CmsSearchResultItem>>().Query(query, queryOptions);
        }

        public StandardQuery CreateStandardQuery(string query, string[] searchfilters, int page, int pageSize, string sortOrder, string language = null)
        {
            string defaultField = CurrentConfiguration.SearchSettings.DefaultField;
            AbstractSolrQuery abstractSolrQuery1 = (AbstractSolrQuery)null;
            AbstractSolrQuery abstractSolrQuery2 = (AbstractSolrQuery)null;
            DateTime dateTime;
            if (!string.IsNullOrEmpty(query))
            {
                if (!this.IsCustomQuery(query))
                {
                    string query1 = query;
                    this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("Query contains no field information, inserting default field \"{0}\" to query", (object)defaultField), (Exception)null);
                    string str1 = string.Empty;
                    if (CurrentConfiguration.SearchSettings.EnableLanguageSupport)
                    {
                        if (string.IsNullOrEmpty(language))
                            language = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
                        if (((IEnumerable<string>)CurrentConfiguration.SearchSettings.EnabledLanguages.Split(new string[1]
                        {
              ","
                        }, StringSplitOptions.RemoveEmptyEntries)).Contains<string>(language))
                        {
                            str1 = "_" + language;
                            this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, "Language support enabled and language active on default query, appending language to text field: " + str1, (Exception)null);
                        }
                        string str2 = "lang:" + language;
                        if (searchfilters == null)
                            searchfilters = new string[1] { str2 };
                        else
                            searchfilters = new List<string>((IEnumerable<string>)searchfilters)
              {
                str2
              }.ToArray();
                    }
                    string format1 = "start_publish: [* TO {0}]";
                    dateTime = DateTime.Now.ToUniversalTime();
                    string str3 = dateTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
                    string str4 = string.Format(format1, (object)str3);
                    this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, "Adding start publish filter " + str4, (Exception)null);
                    if (searchfilters == null)
                        searchfilters = new string[1] { str4 };
                    else
                        searchfilters = new List<string>((IEnumerable<string>)searchfilters)
            {
              str4
            }.ToArray();
                    string format2 = "end_publish: [{0} TO *]";
                    dateTime = DateTime.Now;
                    dateTime = dateTime.ToUniversalTime();
                    string str5 = dateTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
                    string str6 = string.Format(format2, (object)str5);
                    this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, "Adding end publish filter " + str6, (Exception)null);
                    searchfilters = new List<string>((IEnumerable<string>)searchfilters)
          {
            str6
          }.ToArray();
                    query = this.ParseQuery(defaultField + str1, query);
                    abstractSolrQuery2 = this.GetBoostQuery(query1);
                }
                this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, "Query is " + query, (Exception)null);
                abstractSolrQuery1 = (AbstractSolrQuery)new SolrQuery(query);
                if (abstractSolrQuery2 != null)
                    abstractSolrQuery1 = abstractSolrQuery1 || abstractSolrQuery2;
            }
            List<ISolrQuery> solrQueryList = (List<ISolrQuery>)null;
            if (searchfilters != null && ((IEnumerable<string>)searchfilters).Any<string>())
            {
                solrQueryList = new List<ISolrQuery>();
                foreach (string searchfilter in searchfilters)
                {
                    string field = searchfilter.Substring(0, searchfilter.IndexOf(":", StringComparison.InvariantCulture));
                    string str = searchfilter.Substring(searchfilter.IndexOf(":", StringComparison.InvariantCulture) + 1);
                    if (str.Contains(" TO "))
                    {
                        ParsedRange range = RangeFormatHelper.ParseRange(str);
                        if (RangeFormatHelper.IsDateRange(str))
                        {
                            DateTime? from = new DateTime?();
                            if (range.start.ToString() != "*")
                                from = new DateTime?(Convert.ToDateTime(range.start));
                            DateTime? to = new DateTime?();
                            if (range.end.ToString() != "*")
                                to = new DateTime?(Convert.ToDateTime(range.end));
                            solrQueryList.Add((ISolrQuery)new SolrQueryByRange<DateTime?>(field, from, to, range.startinclusive, range.endinclusive));
                        }
                        else
                        {
                            Facet facet = CurrentConfiguration.Facets.Cast<Facet>().FirstOrDefault<Facet>((Func<Facet, bool>)(f => f.Field == field));
                            if (facet != null)
                            {
                                switch (facet.Type)
                                {
                                    case "double":
                                        solrQueryList.Add((ISolrQuery)new SolrQueryByRange<double>(field, Convert.ToDouble(range.start), Convert.ToDouble(range.end), range.startinclusive, range.endinclusive));
                                        continue;
                                    case "decimal":
                                        solrQueryList.Add((ISolrQuery)new SolrQueryByRange<Decimal>(field, Convert.ToDecimal(range.start), Convert.ToDecimal(range.end), range.startinclusive, range.endinclusive));
                                        continue;
                                    default:
                                        solrQueryList.Add((ISolrQuery)new SolrQueryByRange<int>(field, Convert.ToInt32(range.start), Convert.ToInt32(range.end), range.startinclusive, range.endinclusive));
                                        continue;
                                }
                            }
                            else
                                solrQueryList.Add((ISolrQuery)new SolrQueryByRange<string>(field, range.start.ToString(), range.end.ToString(), range.startinclusive, range.endinclusive));
                        }
                    }
                    else
                        solrQueryList.Add((ISolrQuery)new SolrQueryByField(field, str));
                }
            }
            QueryOptions queryOptions = new QueryOptions();
            if (solrQueryList != null)
                queryOptions.FilterQueries = (ICollection<ISolrQuery>)solrQueryList;
            if (!string.IsNullOrEmpty(sortOrder))
                queryOptions.OrderBy = (ICollection<SortOrder>)new Collection<SortOrder>()
        {
          SortOrder.Parse(sortOrder)
        };
            if (CurrentConfiguration.SearchSettings.SpellCheck)
                queryOptions.SpellCheck = new SpellCheckingParameters()
                {
                    Collate = new bool?(true),
                    Query = query.StripSolrFields()
                };
            Facets facets = CurrentConfiguration.Facets;
            if (facets != null && facets.Count > 0)
            {
                FacetParameters facetParameters = new FacetParameters();
                List<ISolrFacetQuery> solrFacetQueryList = new List<ISolrFacetQuery>();
                foreach (Facet facet in facets.Cast<Facet>())
                {
                    string facetField = FieldNameHelper.GetFacetField(facet.Field);
                    switch (facet.Type)
                    {
                        case "date":
                            using (IEnumerator<FacetRange> enumerator = facet.Ranges.Cast<FacetRange>().GetEnumerator())
                            {
                                while (enumerator.MoveNext())
                                {
                                    FacetRange current = enumerator.Current;
                                    DateTime start = DateTime.Parse(current.Start);
                                    DateTime end = DateTime.Parse(current.End);
                                    SolrFacetDateQuery solrFacetDateQuery = new SolrFacetDateQuery(facetField, start, end, "+" + current.Gap + "DAY");
                                    solrFacetQueryList.Add((ISolrFacetQuery)solrFacetDateQuery);
                                }
                                continue;
                            }
                        case "range":
                            using (IEnumerator<FacetRange> enumerator = facet.Ranges.Cast<FacetRange>().GetEnumerator())
                            {
                                while (enumerator.MoveNext())
                                {
                                    FacetRange current = enumerator.Current;
                                    string dataType = current.DataType;
                                    string s1 = string.Empty;
                                    if (!string.IsNullOrEmpty(current.Start))
                                        s1 = current.Start;
                                    string s2 = string.Empty;
                                    if (!string.IsNullOrEmpty(current.End))
                                        s2 = current.End;
                                    string str = string.Empty;
                                    if (!string.IsNullOrEmpty(current.Gap))
                                        str = current.Gap;
                                    switch (dataType)
                                    {
                                        case "date":
                                            DateTime from1;
                                            ISolrQuery q;
                                            if (!string.IsNullOrEmpty(current.Dynamic))
                                            {
                                                switch (current.Dynamic)
                                                {
                                                    case "thisday":
                                                        from1 = DateTime.Today;
                                                        DateTime to1 = from1.AddDays(1.0);
                                                        q = (ISolrQuery)new SolrQueryByRange<DateTime>(facetField, from1, to1, true, false);
                                                        break;
                                                    case "thisweek":
                                                        from1 = this.GetFirstDayOfWeek(DateTime.Today);
                                                        DateTime to2 = from1.AddDays(7.0);
                                                        q = (ISolrQuery)new SolrQueryByRange<DateTime>(facetField, from1, to2, true, false);
                                                        break;
                                                    case "thismonth":
                                                        // ISSUE: explicit reference operation
                                                        // ISSUE: variable of a reference type
                                                        from1 = DateTime.Today;
                                                        DateTime local1 = from1;
                                                        dateTime = DateTime.Now;
                                                        int year1 = dateTime.Year;
                                                        dateTime = DateTime.Now;
                                                        int month1 = dateTime.Month;
                                                        int day1 = 1;
                                                        // ISSUE: explicit reference operation
                                                        local1 = new DateTime(year1, month1, day1);
                                                        DateTime to3 = from1.AddMonths(1);
                                                        q = (ISolrQuery)new SolrQueryByRange<DateTime>(facetField, from1, to3, true, false);
                                                        break;
                                                    case "thisyear":
                                                        // ISSUE: explicit reference operation
                                                        // ISSUE: variable of a reference type
                                                        from1 = DateTime.Today;
                                                        DateTime local2 = from1;
                                                        dateTime = DateTime.Now;
                                                        int year2 = dateTime.Year;
                                                        int month2 = 1;
                                                        int day2 = 1;
                                                        // ISSUE: explicit reference operation
                                                        local2 = new DateTime(year2, month2, day2);
                                                        DateTime to4 = from1.AddYears(1);
                                                        q = (ISolrQuery)new SolrQueryByRange<DateTime>(facetField, from1, to4, true, false);
                                                        break;
                                                    case "last":
                                                        int num1 = Convert.ToInt32(str) * -1;
                                                        DateTime today = DateTime.Today;
                                                        from1 = today.AddDays((double)num1);
                                                        q = (ISolrQuery)new SolrQueryByRange<DateTime>(facetField, from1, today, true, true);
                                                        break;
                                                    default:
                                                        throw new ArgumentException("No valid dynamic range resolved");
                                                }
                                            }
                                            else
                                            {
                                                from1 = DateTime.Parse(s1);
                                                DateTime to5 = DateTime.Parse(s2);
                                                q = (ISolrQuery)new SolrQueryByRange<DateTime>(facetField, from1, to5, true);
                                            }
                                            SolrFacetQuery solrFacetQuery1 = new SolrFacetQuery(q);
                                            solrFacetQueryList.Add((ISolrFacetQuery)solrFacetQuery1);
                                            continue;
                                        case "double":
                                            double num2 = Convert.ToDouble(s1);
                                            double num3 = Convert.ToDouble(s2);
                                            double num4 = string.IsNullOrEmpty(str) ? num3 - num2 : Convert.ToDouble(str);
                                            double from2 = num2;
                                            while (from2 < num3)
                                            {
                                                SolrFacetQuery solrFacetQuery2 = new SolrFacetQuery((ISolrQuery)new SolrQueryByRange<double>(facetField, from2, from2 + num4, true, false));
                                                solrFacetQueryList.Add((ISolrFacetQuery)solrFacetQuery2);
                                                from2 += num4;
                                            }
                                            continue;
                                        case "decimal":
                                            Decimal num5 = Convert.ToDecimal(s1);
                                            Decimal num6 = Convert.ToDecimal(s2);
                                            Decimal num7 = string.IsNullOrEmpty(str) ? num6 - num5 : Convert.ToDecimal(str);
                                            Decimal from3 = num5;
                                            while (from3 < num6)
                                            {
                                                SolrFacetQuery solrFacetQuery2 = new SolrFacetQuery((ISolrQuery)new SolrQueryByRange<Decimal>(facetField, from3, from3 + num7, true, false));
                                                solrFacetQueryList.Add((ISolrFacetQuery)solrFacetQuery2);
                                                from3 += num7;
                                            }
                                            continue;
                                        default:
                                            int int32_1 = Convert.ToInt32(s1);
                                            int int32_2 = Convert.ToInt32(s2);
                                            int num8 = string.IsNullOrEmpty(str) ? int32_2 - int32_1 : Convert.ToInt32(str);
                                            int from4 = int32_1;
                                            while (from4 < int32_2)
                                            {
                                                SolrFacetQuery solrFacetQuery2 = new SolrFacetQuery((ISolrQuery)new SolrQueryByRange<int>(facetField, from4, from4 + num8, true, false));
                                                solrFacetQueryList.Add((ISolrFacetQuery)solrFacetQuery2);
                                                from4 += num8;
                                            }
                                            continue;
                                    }
                                }
                                continue;
                            }
                        default:
                            SolrFacetFieldQuery solrFacetFieldQuery = new SolrFacetFieldQuery(facetField)
                            {
                                MinCount = new int?(facet.MinCount),
                                Limit = new int?(facet.Limit),
                                Sort = facet.Sort
                            };
                            solrFacetQueryList.Add((ISolrFacetQuery)solrFacetFieldQuery);
                            continue;
                    }
                }
                facetParameters.Queries = (ICollection<ISolrFacetQuery>)solrFacetQueryList;
                queryOptions.Facet = facetParameters;
            }
            if (CurrentConfiguration.SearchSettings.Highlight)
            {
                string[] strArray = CurrentConfiguration.SearchSettings.HighlightFields.Split(new string[1]
                {
          ","
                }, StringSplitOptions.RemoveEmptyEntries);
                string fragmenter = CurrentConfiguration.SearchSettings.Fragmenter;
                queryOptions.Highlight = new HighlightingParameters()
                {
                    Fields = (ICollection<string>)strArray,
                    Fragmenter = new SolrHighlightFragmenter?(fragmenter.ToLower() == "regex" ? SolrHighlightFragmenter.Regex : SolrHighlightFragmenter.Gap),
                    Fragsize = new int?(CurrentConfiguration.SearchSettings.Fragsize)
                };
            }
            if (pageSize > 0)
            {
                int num = pageSize * (page - 1);
                queryOptions.Start = new int?(num);
                queryOptions.Rows = new int?(pageSize);
            }
            LocalParams localParams = (LocalParams)null;
            if (!string.IsNullOrEmpty(CurrentConfiguration.SearchSettings.DefaultOperator))
            {
                localParams = new LocalParams();
                localParams.Add("q.op", CurrentConfiguration.SearchSettings.DefaultOperator.ToUpper());
            }
            SolrQuery userSecurityQuery = SecurityHelper.GetUserSecurityQuery();
            AbstractSolrQuery abstractSolrQuery3 = abstractSolrQuery1;
            if (abstractSolrQuery3)
            {
                SolrQuery solrQuery = userSecurityQuery;
                abstractSolrQuery3 &= (AbstractSolrQuery)solrQuery;
            }
            AbstractSolrQuery abstractSolrQuery4 = abstractSolrQuery3;
            return new StandardQuery()
            {
                LocalParams = localParams,
                Query = abstractSolrQuery4,
                QueryOptions = queryOptions
            };
        }

        private AbstractSolrQuery GetBoostQuery(string query)
        {
            AbstractSolrQuery abstractSolrQuery = (AbstractSolrQuery)null;
            List<KeyValuePair<string, double>> source = new List<KeyValuePair<string, double>>();
            foreach (DocType docType in CurrentConfiguration.DocTypes.Cast<DocType>())
            {
                foreach (Property property in docType.DocTypeProperties.Cast<Property>().Where<Property>((Func<Property, bool>)(p =>
                {
                    if (p.Content)
                        return p.Boost > 0.0;
                    return false;
                })).ToList<Property>())
                {
                    string fieldname = FieldNameHelper.GetDynamicFieldName(property.Name, (string)null);
                    if (source.All<KeyValuePair<string, double>>((Func<KeyValuePair<string, double>, bool>)(k => k.Key != fieldname)))
                        source.Add(new KeyValuePair<string, double>(fieldname, property.Boost));
                }
            }
            string empty = string.Empty;
            foreach (KeyValuePair<string, double> keyValuePair in source)
                empty += string.Format("({0}:\"{1}\")^{2} OR ", (object)keyValuePair.Key, (object)query, (object)keyValuePair.Value);
            if (!string.IsNullOrEmpty(empty))
                abstractSolrQuery = (AbstractSolrQuery)new SolrQuery(empty.Substring(0, empty.Length - 4));
            return abstractSolrQuery;
        }

        private bool IsCustomQuery(string query)
        {
            if (!query.Contains(":"))
                return false;
            char[] charArray = query.ToCharArray();
            string empty = string.Empty;
            for (int index = 0; index < charArray.Length; ++index)
            {
                char ch = charArray[index];
                if ((int)ch == 58 && empty != "\\")
                    return true;
                empty = ch.ToString((IFormatProvider)CultureInfo.InvariantCulture);
            }
            return false;
        }

        public List<string> AutoComplete(string query)
        {
            string xml = new SolrConnection(((SolisSearchConfigurationSection)ConfigurationManager.GetSection("SolisSearch")).SolrServer.Address).Get("/suggest", (IEnumerable<KeyValuePair<string, string>>)new Dictionary<string, string>()
      {
        {
          "q",
          query
        },
        {
          "wt",
          "xml"
        }
      });
            List<string> source1 = new List<string>();
            if (!string.IsNullOrEmpty(xml))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xml);
                XmlNodeList source2 = xmlDocument.SelectNodes("//str[@name='collation']");
                if (source2 != null && source2.Count > 0)
                {
                    foreach (XmlNode xmlNode in source2.Cast<XmlNode>())
                        source1.Add(xmlNode.InnerText);
                }
                XmlNodeList source3 = xmlDocument.SelectNodes("//arr[@name='suggestion']/str");
                if (source3 != null && source3.Count > 0)
                {
                    foreach (XmlNode xmlNode in source3.Cast<XmlNode>())
                        source1.Add(xmlNode.InnerText);
                }
            }
            return source1.Distinct<string>().ToList<string>();
        }

        private string ParseQuery(string field, string query)
        {
            if (!query.Contains(" ") || query.Contains(" AND ") || (query.Contains(" OR ") || query.Contains(" NOT ")) || query.Contains("\""))
                return field + ":" + query;
            string[] strArray = query.Split(new string[1] { " " }, StringSplitOptions.RemoveEmptyEntries);
            string str1 = string.Empty;
            foreach (string str2 in strArray)
                str1 = str1 + field + ":" + str2 + " ";
            return str1;
        }

        private DateTime GetFirstDayOfWeek(DateTime dateTime)
        {
            if (dateTime.DayOfWeek == DayOfWeek.Monday)
                return dateTime;
            return this.GetFirstDayOfWeek(dateTime.AddDays(-1.0));
        }
    }
}
