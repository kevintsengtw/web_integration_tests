using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace Sample.WebApplicationIntegrationTests.Utilities;

/// <summary>
/// Class RedisFixture
/// </summary>
public sealed class RedisFixture
{
    /// <summary>
    /// Create Redis Container.
    /// </summary>
    /// <param name="redisSettings">redisSettings</param>
    /// <param name="typeOfTarget">typeOfTarget</param>
    /// <returns></returns>
    public static (IContainer ContainerService, int RedisPort) CreateContainer(Redis redisSettings, Type typeOfTarget)
    {
        var redisPort = redisSettings.HostPort;

        var environmentName = TestSettingProvider.GetEnvironmentName(typeOfTarget);
        var containerName = redisSettings.ContainerName;

        var container = new ContainerBuilder()
                        .WithImage($"{redisSettings.Image}:{redisSettings.Tag}")
                        .WithPortBinding(redisSettings.HostPort, redisSettings.ContainerPort)
                        .WithName($"{environmentName}-{containerName}")
                        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(redisSettings.ContainerPort))
                        .WithAutoRemove(true)
                        .Build();

        return (container, redisPort);
    }
}