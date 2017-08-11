using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolisSearch.Interfaces
{
    public interface ICmsEntityFactory
    {
        ICmsContent CreateCmsContent(object cmsNode);

        ICmsMedia CreateCmsMedia(object cmsMedia);

        ICmsProperty CreateCmsProperty(object cmsProperty);

        string ActualId(string customId);
    }
}
