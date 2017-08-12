using SolrNet.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SolisSearch.Entities
{
    public class CmsSearchResultItem
    {
        private ICollection<string> acl;
        private ICollection<string> alphaindex;
        private ICollection<string> breadcrumbs;
        private ICollection<string> content;
        private ICollection<string> contentType;
        private ICollection<string> docTypes;
        private List<string> documents;
        private ICollection<string> languages;
        private ICollection<string> title;

        [SolrUniqueKey("id")]
        public string Id { get; set; }

        [SolrField("name")]
        public string Name { get; set; }

        [SolrField("url")]
        public string LinkUrl { get; set; }

        [SolrField("breadcrumbs")]
        public ICollection<string> Breadcrumbs
        {
            get
            {
                return this.breadcrumbs ?? (this.breadcrumbs = (ICollection<string>)new Collection<string>());
            }
            set
            {
                this.breadcrumbs = value;
            }
        }

        [SolrField("doctypes")]
        public ICollection<string> DocTypes
        {
            get
            {
                return this.docTypes ?? (this.docTypes = (ICollection<string>)new Collection<string>());
            }
            set
            {
                this.docTypes = value;
            }
        }

        [SolrField("content")]
        public ICollection<string> Content
        {
            get
            {
                return this.content ?? (this.content = (ICollection<string>)new Collection<string>());
            }
            set
            {
                this.content = value;
            }
        }

        [SolrField("created")]
        public DateTime Created { get; set; }

        [SolrField("last_modified")]
        public DateTime LastModified { get; set; }

        [SolrField("start_publish")]
        public DateTime StartPublish { get; set; }

        [SolrField("end_publish")]
        public DateTime EndPublish { get; set; }

        [SolrField("content_type")]
        public ICollection<string> ContentType
        {
            get
            {
                return this.contentType ?? (this.contentType = (ICollection<string>)new Collection<string>());
            }
            set
            {
                this.contentType = value;
            }
        }

        [SolrField("acl")]
        public ICollection<string> Acl
        {
            get
            {
                return this.acl ?? (this.acl = (ICollection<string>)new Collection<string>());
            }
            set
            {
                this.acl = value;
            }
        }

        [SolrField("title")]
        public ICollection<string> DocumentTitle
        {
            get
            {
                return this.title ?? (this.title = (ICollection<string>)new Collection<string>());
            }
            set
            {
                this.title = value;
            }
        }

        [SolrField("alphaindex")]
        public ICollection<string> AlphaIndex
        {
            get
            {
                return this.alphaindex ?? (this.alphaindex = (ICollection<string>)new Collection<string>());
            }
            set
            {
                this.alphaindex = value;
            }
        }

        [SolrField("resourcename")]
        public string ResourceName { get; set; }

        [SolrField("lang")]
        public ICollection<string> Languages
        {
            get
            {
                return this.languages ?? (this.languages = (ICollection<string>)new Collection<string>());
            }
            set
            {
                this.languages = value;
            }
        }

        [SolrField("*")]
        public IDictionary<string, object> CmsProperties { get; set; }

        public List<string> Documents
        {
            get
            {
                return this.documents ?? (this.documents = new List<string>());
            }
            set
            {
                this.documents = value;
            }
        }

        public CmsSearchResultItem()
        {
            this.CmsProperties = (IDictionary<string, object>)new Dictionary<string, object>();
        }

        public CmsSearchResultItem Clone()
        {
            return (CmsSearchResultItem)MemberwiseClone();
        }
    }
}
