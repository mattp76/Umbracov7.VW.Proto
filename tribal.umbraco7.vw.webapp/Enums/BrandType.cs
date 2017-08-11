using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace tribal.umbraco7.vw.webapp.Enums
{
    public enum BrandType
    {
        [Description("vw passenger")]
        Passenger = 0,
        [Description("vw commercial")]
        Commercial = 1,
    }
}