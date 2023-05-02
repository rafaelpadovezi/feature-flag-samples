using System.Data.Common;
using LaunchDarkly.Sdk.Server;
using LaunchDarkly.Sdk.Server.Integrations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;

namespace Flaggy.Tests.Support;

public class TestWebApplicationFactory :  WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables().Build();
        builder.ConfigureServices(services =>
        {
            // Override ApplicationContext
            services.RemoveAll<ApplicationContext>();
            services.RemoveAll<DbContextOptions<ApplicationContext>>();

            var builder = new SqlConnectionStringBuilder(configuration.GetConnectionString("ApplicationContext"));
            builder.InitialCatalog = "TestDb";

            services.AddDbContext<ApplicationContext>(
                options => options.UseSqlServer(builder.ConnectionString), ServiceLifetime.Singleton);
            
            // Override LauchDarkly SDK
            var td = TestData.DataSource();
            // You can set any initial flag states here with td.Update
            var config = Configuration.Builder("TEST-SDK")
                .DataSource(td)
                .Build();
            var client = new LdClient(config);
            services.AddSingleton(_ => client);
            services.AddSingleton(_ => td);
        });
        builder.UseEnvironment("Development");
    }
}
