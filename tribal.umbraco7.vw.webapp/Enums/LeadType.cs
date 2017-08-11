using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace tribal.umbraco7.vw.webapp.Enums
{
    public enum LeadType
    {
        [Description("BROCHURE")]
        Brocure = 0,
        [Description("TESTDRIVE")]
        TestDrive = 1,
        [Description("CALLBACK")]
        Callback = 2,
        [Description("REGISTERINTEREST")]
        RegisterInterest = 3,
        [Description("COMPETITION")]
        Competition = 4
    }
}