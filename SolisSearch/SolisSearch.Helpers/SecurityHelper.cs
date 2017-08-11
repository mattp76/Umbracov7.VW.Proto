using SolrNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

namespace SolisSearch.Helpers
{
    public class SecurityHelper
    {
        internal static SolrQuery GetUserSecurityQuery()
        {
            StringBuilder stringBuilder = new StringBuilder("acl:Everyone");
            if (HttpContext.Current != null && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                string name = HttpContext.Current.User.Identity.Name;
                try
                {
                    string[] rolesForUser = Roles.GetRolesForUser(name);
                    if (((IEnumerable<string>)rolesForUser).Any<string>())
                    {
                        foreach (string str in rolesForUser)
                        {
                            if (!(str == "Everyone"))
                                stringBuilder.Append(" OR acl:\"" + str + "\"");
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return new SolrQuery("(" + (object)stringBuilder + ")");
        }
    }
}
