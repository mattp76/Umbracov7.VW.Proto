using System;

namespace SolisSearch.Interfaces
{
    public interface ICmsContent
    {
        string Id { get; set; }

        string Name { get; set; }

        string ContentType { get; set; }

        DateTime CreateDate { get; set; }

        DateTime StartPublish { get; set; }

        DateTime EndPublish { get; set; }

        DateTime UpdateDate { get; set; }

        object NativeNodeObject { get; set; }

        string Path { get; set; }

        int ParentId { get; set; }
    }
}
