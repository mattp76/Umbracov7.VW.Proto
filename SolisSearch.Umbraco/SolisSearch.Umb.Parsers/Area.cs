using System.Runtime.Serialization;

namespace SolisSearch.Umb.Parsers
{
    [DataContract]
    internal class Area
    {
        [DataMember(IsRequired = false)]
        public int grid { get; set; }

        [DataMember(IsRequired = false)]
        public bool allowAll { get; set; }

        [DataMember(IsRequired = false)]
        public string[] allowed { get; set; }

        [DataMember(IsRequired = false)]
        public Control[] controls { get; set; }
    }
}
