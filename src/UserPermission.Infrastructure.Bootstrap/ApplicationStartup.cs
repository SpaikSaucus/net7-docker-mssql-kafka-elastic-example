using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using UserPermission.Domain.Core;
using UserPermission.Infrastructure.Bootstrap.Extensions.ApplicationBuilder;
using UserPermission.Infrastructure.Bootstrap.Extensions.ServiceCollection;
using UserPermission.Infrastructure.Core;
using UserPermission.Infrastructure.EF;

namespace UserPermission.Infrastructure.Bootstrap
{
    public class ApplicationStartup
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment env;

        public ApplicationStartup(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.configuration = configuration;    
            this.env = env;
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.AddAutofacExtension(this.configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCorsExtension();
            services.AddHealthChecksExtension();
            services.AddSwaggerGenExtension();
            services.AddResponseCompressionExtension();
            services.AddHttpContextAccessor();
           
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Application.Behaviors.ValidatorBehavior<,>).Assembly));
            services.AddControllers(o =>
                {
                    o.Filters.Add(new ProducesResponseTypeAttribute(400));
                    o.Filters.Add(new ProducesResponseTypeAttribute(500));
                }).AddJsonOptions(o =>
                {
                    o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });

            services.AddDbContext<UserPermissionDbContext>(opt =>
            {
                opt.UseSqlServer(this.configuration.GetConnectionString("DataBase"));
            });

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddElasticsearchExtension(this.configuration, this.env);
            services.AddKafkaExtension(this.configuration);
        }

        public void Configure(IApplicationBuilder app)
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

            app.UseKafkaHandlerMiddleware(this.configuration.GetValue("AppSettings:KafkaTopic",string.Empty));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
            });
        }
    }
}
