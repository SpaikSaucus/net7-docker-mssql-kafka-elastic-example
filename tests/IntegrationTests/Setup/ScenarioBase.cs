using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;

namespace IntegrationTests.Setup
{
	public class ScenarioBase
	{
		public TestServer CreateServer()
		{
			var path = Assembly.GetAssembly(typeof(ScenarioBase)).Location;
			var host = Host.CreateDefaultBuilder()
				.ConfigureAppConfiguration((hostingContext, config) =>
				{
					config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
						.AddJsonFile($"appsettings.Local.json", optional: true, reloadOnChange: true);
					config.AddEnvironmentVariables();
				})
				.UseServiceProviderFactory(new AutofacServiceProviderFactory())
				.ConfigureWebHostDefaults(webHostBuilder =>
				{
					webHostBuilder
						.UseTestServer()
						.UseContentRoot(Path.GetDirectoryName(path))
						.UseStartup<TestsStartup>();
				})
				.Build();

			host.Start();
			return host.GetTestServer();
		}

		public static class Get
		{
			public const string Permissions = "api/permissions";
		}

		public static class Post
		{
			public const string Permissions = "api/permissions";
		}

		public static class Patch
		{
			public const string Permissions = "api/permissions";
		}
	}
}
