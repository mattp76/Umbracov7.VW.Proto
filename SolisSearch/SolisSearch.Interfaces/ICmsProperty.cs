
namespace SolisSearch.Interfaces
{
    public interface ICmsProperty
    {
        string Name { get; set; }

        object Value { get; set; }

        object NativePropertyObject { get; set; }
    }
}
