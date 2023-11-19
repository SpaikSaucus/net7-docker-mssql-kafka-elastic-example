using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using Microsoft.AspNetCore.Builder;

namespace UserPermission.Infrastructure.Bootstrap.Extensions.ServiceCollection
{
    public static class CompressionServiceCollectionExtensions
    {
        public static void AddResponseCompressionExtension(this IServiceCollection services)
        {
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
            });
        }
    }
}
