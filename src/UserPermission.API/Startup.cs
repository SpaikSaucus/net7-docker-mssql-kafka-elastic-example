using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserPermission.Infrastructure.Bootstrap;

namespace UserPermission.API
{
    public class Startup
    {
        private readonly ApplicationStartup applicationStartup;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.applicationStartup = new ApplicationStartup(configuration, env);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            this.applicationStartup.ConfigureServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            this.applicationStartup.Configure(app);
            if (env.EnvironmentName == Environments.Development || env.EnvironmentName == "Local")
            {
                app.UseDeveloperExceptionPage();
            }
        }

        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you by the factory.
        public void ConfigureContainer(ContainerBuilder builder)
        {
            this.applicationStartup.ConfigureContainer(builder);
        }
    }
}
