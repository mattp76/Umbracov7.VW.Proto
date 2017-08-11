using System;
using System.Collections.Specialized;

namespace SolisSearch.Entities
{
    public class Statistics
    {
        private NameValueCollection richTextDocuments;

        public int NumDocs { get; set; }

        public NameValueCollection RichTextDocuments
        {
            get
            {
                return this.richTextDocuments ?? (this.richTextDocuments = new NameValueCollection());
            }
            set
            {
                this.richTextDocuments = value;
            }
        }
    }
}
