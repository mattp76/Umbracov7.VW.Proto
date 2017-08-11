using System.Collections.Generic;
using System.IO;

namespace SolisSearch.Interfaces
{
    public interface ICmsIndexer
    {
        string CmsRootId { get; }

        ICmsEntityFactory CmsEntityFactory { get; }

        ICmsProperty GetProperty(ICmsContent cmsNode, string propertyName);

        object GetPropertyValue(ICmsContent cmsNode, string propertyName);

        T GetPropertyValue<T>(ICmsContent cmsNode, string propertyName);

        ICollection<string> GetNodeAcl(ICmsContent cmsNode);

        string GetContentUrl(ICmsContent cmsNode);

        ICollection<string> GetBreadcrumbs(ICmsContent cmsNode);

        ICmsMedia ResolveMedia(string mediaUrl);

        ICmsContent GetContentById(int id);

        IList<ICmsContent> GetIndexingRoot(object rootIndex);

        IEnumerable<ICmsContent> GetDescendants(ICmsContent node);

        ICollection<string> GetNodeLanguages(ICmsContent node);

        ICollection<string> GetContentTypes(ICmsContent node);

        ICmsContent GetParent(ICmsContent node);

        bool IsFileToIndex(string filename, string friendlyFilename);

        string GetMediaFriendlyUrl(string input);

        Stream GetFile(string path);
    }
}
