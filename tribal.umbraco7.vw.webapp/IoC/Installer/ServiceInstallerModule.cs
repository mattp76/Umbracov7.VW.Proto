using System;
using Autofac;
using tribal.umbraco7.vw.webapp.Interfaces;
using tribal.umbraco7.vw.webapp.Services;

namespace tribal.umbraco7.vw.webapp.IoC.Installer
{
    public class ServiceInstallerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var domainAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            builder.RegisterType<LeadService>().As<ILeadService>().SingleInstance();
            builder.RegisterType<PricingService>().As<IPricingService>().SingleInstance();

        }
    }
}