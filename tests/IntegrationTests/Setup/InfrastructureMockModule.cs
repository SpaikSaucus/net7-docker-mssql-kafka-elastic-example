using Autofac;
using Microsoft.EntityFrameworkCore;
using UserPermission.Domain.Core;
using UserPermission.Domain.Permission.Models;
using UserPermission.Infrastructure.Core;
using UserPermission.Infrastructure.Services;

namespace IntegrationTests.Setup
{
	public class InfrastructureMockModule : Module
	{
		public InfrastructureMockModule()
		{
		}

		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<UserPermissionMockDbContext>()
			 .As<DbContext>()
			 .InstancePerLifetimeScope();

			builder.RegisterType<UnitOfWork>()
			 .As<IUnitOfWork>()
			 .InstancePerLifetimeScope();

			builder.RegisterType<ElasticsearchCRUD>()
			 .As<IElasticsearchCRUD<Permission>>()
			 .InstancePerLifetimeScope();
		}
	}
}
