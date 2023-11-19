using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System;
using UserPermission.Domain.Permission.Models;

namespace UserPermission.Infrastructure.Bootstrap.Extensions.ServiceCollection
{
    public static class ElasticsearchServiceCollectionExtensions
    {
        public static void AddElasticsearchExtension(this IServiceCollection services, 
            IConfiguration configuration, IWebHostEnvironment env)
        {
            var baseUrl = configuration.GetConnectionString("Elasticsearch");
            var indexName = configuration.GetValue<string>("AppSettings:ElasticsearchIndex");
            var username = configuration.GetValue<string>("AppSettings:ElasticsearchUsername");
            var password = configuration.GetValue<string>("AppSettings:ElasticsearchPassword");

            var settings = new ConnectionSettings(new Uri(baseUrl ?? ""))
                .BasicAuthentication(username, password)
                .EnableApiVersioningHeader()
                .EnableHttpCompression()
                .DisableDirectStreaming()
                .RequestTimeout(TimeSpan.FromMinutes(2))
                .DefaultIndex(indexName);

            if (env.EnvironmentName == "Local")
                settings.ServerCertificateValidationCallback((sender, certificate, chain, sslPolicyErrors) => true);

            var client = new ElasticClient(settings);
            services.AddSingleton<IElasticClient>(client);

            client.Indices.Create(indexName, index => index.Map<Permission>(x => x.AutoMap()));
        }
    }
}
