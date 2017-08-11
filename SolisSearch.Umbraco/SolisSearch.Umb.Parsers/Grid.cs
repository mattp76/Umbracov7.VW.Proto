using System.Runtime.Serialization;

namespace SolisSearch.Umb.Parsers
{
    [DataContract]
    internal class Grid
    {
        [DataMember(IsRequired = false)]
        public string name { get; set; }

        [DataMember(IsRequired = false)]
        public Section[] sections { get; set; }
    }

}
