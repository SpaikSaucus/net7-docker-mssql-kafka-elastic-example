using Microsoft.AspNetCore.Builder;

namespace UserPermission.Infrastructure.Bootstrap.Extensions.ApplicationBuilder
{
    public static class HealthChecksApplicationBuilderExtensions
    {
        public static void UseHealthChecksExtension(this IApplicationBuilder app)
        {
            app.UseHealthChecks("/health");
        }
    }
}
