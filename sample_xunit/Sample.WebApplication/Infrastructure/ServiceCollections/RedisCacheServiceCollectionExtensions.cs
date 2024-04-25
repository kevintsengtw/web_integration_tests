using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using Sample.Domain.Settings;

namespace Sample.WebApplication.Infrastructure.ServiceCollections;

/// <summary>
/// Class RedisCacheServiceCollectionExtensions
/// </summary>
public static class RedisCacheServiceCollectionExtensions
{
    /// <summary>
    /// Add the AddRedisConfigurationOptions.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddRedisConfigurationOptions(this IServiceCollection services)
    {
        services.AddOptions<RedisConfigurationOptions>()
                .Configure<IServiceScopeFactory>((options, serviceScopeFactory) =>
                {
                    var serviceProvider = serviceScopeFactory.CreateScope().ServiceProvider;
                    var configuration = serviceProvider.GetRequiredService<IConfiguration>();

                    var redisConnectinosSection = configuration.GetSection(RedisConfigurationOptions.SectionName);
                    var redisConfigurationOptions = redisConnectinosSection.Get<RedisConfigurationOptions>();

                    options.InstanceName = redisConfigurationOptions.InstanceName;
                    options.RedisConfiguration = redisConfigurationOptions.RedisConfiguration;
                });

        services.AddRedisCache();

        return services;
    }

    /// <summary>
    /// Add the RedisCache (Microsoft.Extensions.Caching.StackExchangeRedis)
    /// </summary>
    /// <param name="services">The services.</param>
    /// <returns>IServiceCollection.</returns>
    private static IServiceCollection AddRedisCache(this IServiceCollection services)
    {
        services.AddOptions<RedisCacheOptions>()
                .Configure<IServiceProvider>((options, serviceProvider) =>
                {
                    var redisConfigurationOptions = serviceProvider.GetRequiredService<IOptions<RedisConfigurationOptions>>();
                    var redisConfiguration = redisConfigurationOptions.Value;

                    options.InstanceName = redisConfiguration.InstanceName;
                    options.Configuration = redisConfiguration.RedisConfiguration.Configuration;
                });

        return services;
    }
}