using System.Runtime.Serialization;

namespace SolisSearch.Umb.Parsers
{
    [DataContract]
    internal class Section
    {
        [DataMember(IsRequired = false)]
        public Row[] rows { get; set; }
    }
}
