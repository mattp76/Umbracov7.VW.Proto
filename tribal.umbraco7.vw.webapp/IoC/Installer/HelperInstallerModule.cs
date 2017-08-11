using System;
using Autofac;
using tribal.umbraco7.vw.webapp.Interfaces;
using tribal.umbraco7.vw.webapp.Services;
using tribal.umbraco7.vw.webapp.Helpers;

namespace tribal.umbraco7.vw.webapp.IoC.Installer
{
    public class HelperInstallerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var domainAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            builder.RegisterType<VWHelper>().As<IVWHelper>().SingleInstance();

        }
    }
}