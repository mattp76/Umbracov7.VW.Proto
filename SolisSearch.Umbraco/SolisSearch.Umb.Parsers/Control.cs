using System.Runtime.Serialization;

namespace SolisSearch.Umb.Parsers
{
    [DataContract]
    internal class Control
    {
        [DataMember(IsRequired = false)]
        public object value { get; set; }

        [DataMember(IsRequired = false)]
        public Editor editor { get; set; }
    }
}
