using System.Runtime.Serialization;

namespace SolisSearch.Umb.Parsers
{
    [DataContract]
    internal class Row
    {
        [DataMember(IsRequired = false)]
        public string name { get; set; }

        [DataMember(IsRequired = false)]
        public Area[] areas { get; set; }

        [DataMember(IsRequired = false)]
        public string id { get; set; }
    }
}
