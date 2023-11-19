using Autofac;
using UserPermission.Domain.Core;
using UserPermission.Domain.Permission.Models;
using UserPermission.Infrastructure.Core;
using UserPermission.Infrastructure.Services;

namespace UserPermission.Infrastructure.Bootstrap.AutofacModules
{
    public class InfrastructureModule : Module
    {
        public InfrastructureModule()
        {
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UnitOfWork>()
             .As<IUnitOfWork>()
             .InstancePerLifetimeScope();

            builder.RegisterType<ElasticsearchCRUD>()
             .As<IElasticsearchCRUD<Permission>>()
             .InstancePerLifetimeScope();

            builder.RegisterType<KafkaProducer>()
             .As<IKafkaProducer>()
             .InstancePerLifetimeScope();
        }
    }
}
