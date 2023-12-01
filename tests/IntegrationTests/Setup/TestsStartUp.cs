using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using UserPermission.Application.Behaviors;
using UserPermission.Domain.Core;
using UserPermission.Infrastructure.Bootstrap.AutofacModules;
using UserPermission.Infrastructure.Bootstrap.Extensions.ApplicationBuilder;
using UserPermission.Infrastructure.Bootstrap.Extensions.ServiceCollection;
using UserPermission.Infrastructure.Core;

namespace IntegrationTests.Setup
{
	public class TestsStartup
    {
		private readonly IConfiguration configuration;
		private readonly IWebHostEnvironment env;

		public TestsStartup(IConfiguration configuration, IWebHostEnvironment env)
		{
			this.configuration = configuration;
			this.env = env;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddCorsExtension();
			services.AddHealthChecksExtension();
			services.AddSwaggerGenExtension();
			services.AddResponseCompressionExtension();
			services.AddHttpContextAccessor();

			services.AddDbContext<UserPermissionMockDbContext>(opt =>
			{
				opt.UseInMemoryDatabase(databaseName: "InMemoryDatabase");
			});

			services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ValidatorBehavior<,>).Assembly));
			services.AddControllers(o =>
			{
				o.Filters.Add(new ProducesResponseTypeAttribute(400));
				o.Filters.Add(new ProducesResponseTypeAttribute(500));
			}).AddJsonOptions(o =>
			{
				o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
			});

			services.AddTransient<IUnitOfWork, UnitOfWork>();
			services.AddElasticsearchMockExtension();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseCorsExtension();
			app.UseRouting();
			app.UseHealthChecksExtension();
			app.UseResponseCompression();
			app.UseSwaggerExtension();

			app.UseExceptionHandler(errorPipeline =>
			{
				errorPipeline.UseExceptionHandlerMiddleware(this.configuration.GetValue("AppSettings:IncludeErrorDetailInResponse", false));
			});

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapDefaultControllerRoute();
				endpoints.MapControllers();
			});
		}

		public void ConfigureContainer(ContainerBuilder builder)
		{
			builder.RegisterModule(new InfrastructureMockModule());
			builder.RegisterModule(new MediatorModule(configuration.GetValue("AppSettings:CommandLoggingEnabled", false)));
		}
	}
}
