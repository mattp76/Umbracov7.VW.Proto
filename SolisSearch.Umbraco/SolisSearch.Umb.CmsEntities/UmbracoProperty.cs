using SolisSearch.Interfaces;

namespace SolisSearch.Umb.CmsEntities
{
    public class UmbracoProperty : ICmsProperty
    {
        public string Name { get; set; }

        public object Value { get; set; }

        public object NativePropertyObject { get; set; }
    }
}
