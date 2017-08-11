using SolisSearch.Interfaces;
using System;


namespace SolisSearch.Umb.CmsEntities
{
    internal class UmbracoNode : ICmsContent
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string ContentType { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime StartPublish { get; set; }

        public DateTime EndPublish { get; set; }

        public DateTime UpdateDate { get; set; }

        public object NativeNodeObject { get; set; }

        public string Path { get; set; }

        public int ParentId { get; set; }
    }
}
