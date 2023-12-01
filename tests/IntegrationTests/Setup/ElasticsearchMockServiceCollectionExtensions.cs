using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System;
using UserPermission.Domain.Permission.Models;

namespace IntegrationTests.Setup
{
	public static class ElasticsearchMockServiceCollectionExtensions
	{
		public static void AddElasticsearchMockExtension(this IServiceCollection services)
		{
			var baseUrl = "http://foo.test";
			var indexName = "default_index";

			var connectionPool = new SingleNodeConnectionPool(new Uri(baseUrl));
			var settings = new ConnectionSettings(connectionPool, new InMemoryConnection());
			settings.DisableDirectStreaming();
			settings.DefaultIndex(indexName);

			var client = new ElasticClient(settings);
			services.AddSingleton<IElasticClient>(client);

			client.Indices.Create(indexName, index => index.Map<Permission>(x => x.AutoMap()));
		}
	}
}
