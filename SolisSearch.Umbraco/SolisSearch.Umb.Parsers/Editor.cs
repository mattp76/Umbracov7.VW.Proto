using System.Runtime.Serialization;

namespace SolisSearch.Umb.Parsers
{
    [DataContract]
    internal class Editor
    {
        [DataMember(IsRequired = false)]
        public string name { get; set; }

        [DataMember(IsRequired = false)]
        public string alias { get; set; }

        [DataMember(IsRequired = false)]
        public string view { get; set; }

        [DataMember(IsRequired = false)]
        public object render { get; set; }

        [DataMember(IsRequired = false)]
        public string icon { get; set; }
    }
}
