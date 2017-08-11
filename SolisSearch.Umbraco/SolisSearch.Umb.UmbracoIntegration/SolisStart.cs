using Microsoft.Web.Infrastructure.DynamicModuleHelper;

namespace SolisSearch.Umb.UmbracoIntegration
{
    public class SolisStart
    {
        private static bool startWasCalled;

        public static void Start()
        {
            if (SolisStart.startWasCalled)
                return;
            SolisStart.startWasCalled = true;
            DynamicModuleUtility.RegisterModule(typeof(SolisHttpModule));
        }
    }
}
