using SolisSearch.Interfaces;

namespace SolisSearch.Umb.CmsEntities
{
    public class UmbracoMedia : ICmsMedia
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public object NativeMediaObject { get; set; }
    }
}
