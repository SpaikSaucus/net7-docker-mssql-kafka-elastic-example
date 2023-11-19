using Microsoft.Extensions.DependencyInjection;

namespace UserPermission.Infrastructure.Bootstrap.Extensions.ServiceCollection
{
    public static class HealthChecksServiceCollectionExtensions
    {
        public static void AddHealthChecksExtension(this IServiceCollection services)
        {
            services.AddHealthChecks();
        }
    }
}
