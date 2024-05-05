using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace Sample.WebApplicationIntegrationTests.Utilities;

/// <summary>
/// Class MssqlFixture
/// </summary>
public sealed class MssqlFixture
{
    /// <summary>
    /// 使用 Testcontainers-dotnet 建立 Database Container.
    /// </summary>
    /// <param name="databaseSetting"></param>
    /// <param name="typeOfTarget"></param>
    /// <returns></returns>
    public static IContainer CreateContainer(Mssql databaseSetting, Type typeOfTarget)
    {
        var environmentName = TestSettingProvider.GetEnvironmentName(typeOfTarget);
        var containerName = databaseSetting.ContainerName;

        var container = new ContainerBuilder()
                        .WithImage($"{databaseSetting.Image}:{databaseSetting.Tag}")
                        .WithEnvironment(databaseSetting.EnvironmentSettings)
                        .WithName($"{environmentName}-{containerName}")
                        .WithPortBinding(databaseSetting.HostPort, databaseSetting.ContainerPort)
                        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(databaseSetting.ContainerPort))
                        .WithAutoRemove(true)
                        .Build();

        return container;
    }
}