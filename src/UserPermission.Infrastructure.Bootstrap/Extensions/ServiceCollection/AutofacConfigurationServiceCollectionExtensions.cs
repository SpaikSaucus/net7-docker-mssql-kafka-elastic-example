using Autofac;
using Microsoft.Extensions.Configuration;
using UserPermission.Infrastructure.Bootstrap.AutofacModules;

namespace UserPermission.Infrastructure.Bootstrap.Extensions.ServiceCollection
{
    public static class AutofacConfigurationServiceCollectionExtensions
    {
        public static void AddAutofacExtension(this ContainerBuilder builder, IConfiguration configuration)
        {
            builder.RegisterModule(new InfrastructureModule());
            builder.RegisterModule(new MediatorModule(configuration.GetValue("AppSettings:CommandLoggingEnabled", false)));
        }
    }
}
