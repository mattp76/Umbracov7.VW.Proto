using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolrNet;
using SolrNet.Commands.Parameters;

namespace SolisSearch.Entities
{
    public class StandardQuery
    {
        public LocalParams LocalParams { get; set; }

        public AbstractSolrQuery Query { get; set; }

        public QueryOptions QueryOptions { get; set; }
    }
}
