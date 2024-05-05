using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sample.Domain.Settings;

namespace Sample.WebApplicationIntegrationTests;

/// <summary>
/// Class TestWebApplicationFactory
/// </summary>
/// <typeparam name="TProgram"></typeparam>
public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : Program
{
    /// <summary>
    /// Gives a fixture an opportunity to configure the application before it gets built.
    /// </summary>
    /// <param name="builder">builder</param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            var projectDirectory = Directory.GetCurrentDirectory();

            // appSettings
            configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var appSettingsTestFile = Path.Combine(projectDirectory, "Settings", "appsettings.Test.json");
            configurationBuilder.AddJsonFile(appSettingsTestFile, optional: true, reloadOnChange: true);
            Console.WriteLine($"{TimeProvider.System.GetLocalNow().DateTime:yyyy-MM-dd HH:mm:ss} AddJsonFile {appSettingsTestFile}");
        });

        builder.ConfigureTestServices(services =>
        {
            // DatabaseConnectionOptions
            var databaseConnectionOptions = services.SingleOrDefault(x => x.ServiceType == typeof(IOptions<DatabaseConnectionOptions>));
            services.Remove(databaseConnectionOptions);
            services.AddSingleton(_ => Options.Create(new DatabaseConnectionOptions
            {
                ConnectionString = ProjectFixture.SampleDbConnectionString
            }));

            // RedisConfigurationOptions
            var redisConfigurationOprions = services.SingleOrDefault(x => x.ServiceType == typeof(RedisConfigurationOptions));
            services.Remove(redisConfigurationOprions);
            services.AddSingleton(_ =>
            {
                var options = new RedisConfigurationOptions
                {
                    InstanceName = "Sample.WebApplication",
                    RedisConfiguration = new RedisConfiguration
                    {
                        Hosts = ["127.0.0.1"],
                        Ports = [ProjectFixture.RedisPort]
                    }
                };
                return options;
            });
        });
    }
}