using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace UserPermission.Infrastructure.Bootstrap.Extensions.ServiceCollection
{
    public static class SwaggerServiceCollectionExtensions
    {
        private const string BearerSecurityScheme = "Bearer";
        
        public static void AddSwaggerGenExtension(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{GetApplicationName()}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });
        }

        private static string GetApplicationName()
        {
            return (System.Reflection.Assembly.GetEntryAssembly() ?? System.Reflection.Assembly.GetExecutingAssembly()).GetName().Name;
        }
    }
}
