using System;
using Microsoft.Extensions.Configuration;

namespace Sample.WebApplicationIntegrationTests.Utilities;

/// <summary>
/// class TestSettingProvider
/// </summary>
public static class TestSettingProvider
{
    public static string SettingFile { get; set; } = "TestSettings.json";
    
    /// <summary>
    /// Get Environment Name
    /// </summary>
    /// <returns></returns>
    public static string GetEnvironmentName(Type typeOfTarget)
    {
        var configuration = GetConfiguration();

        var environmentName = string.IsNullOrWhiteSpace(configuration["EnvironmentName"])
            ? typeOfTarget.Assembly.GetName().Name.ToLower().Replace(".", "-")
            : configuration["EnvironmentName"].ToLower();

        return environmentName;
    }

    /// <summary>
    /// Get Environment Variables Settings
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="section"></param>
    /// <returns></returns>
    public static Dictionary<string, string> GetEnvironmentSettings(IConfigurationRoot configuration, string section)
    {
        var environments = new Dictionary<string, string>();

        var children = configuration.GetSection(section)?.GetChildren().ToArray();
        if (children is null)
        {
            return environments;
        }

        foreach (var item in children.Select(child => child.Value))
        {
            var value = item.Split("=");
            environments.Add(value[0], value[1]);
        }

        return environments;
    }

    /// <summary>
    /// 取得建立測試資料庫指定使用的 docker 設定資料
    /// </summary>
    /// <returns>System.String.</returns>
    public static Mssql GetDatabaseSettings()
    {
        var configuration = GetConfiguration();

        var databaseSettings = new Mssql
        {
            Image = configuration["Mssql:Image"],
            Tag = configuration["Mssql:Tag"],
            SaPassword = configuration["Mssql:SaPassword"],
            ContainerName = configuration["Mssql:ContainerName"],
            ContainerReadyMessage = configuration["Mssql:ContainerReadyMessage"],
            EnvironmentSettings = GetEnvironmentSettings(configuration, "Mssql:EnvironmentSettings"),
            HostPort = string.IsNullOrWhiteSpace(configuration["Mssql:HostPort"])
                ? GetRandomPort()
                : configuration["Mssql:HostPort"].Equals("0")
                    ? GetRandomPort()
                    : ushort.TryParse(configuration["Mssql:HostPort"], out var databasePort)
                        ? databasePort
                        : GetRandomPort(),
            ContainerPort = string.IsNullOrWhiteSpace(configuration["Mssql:ContainerPort"])
                ? (ushort)1433
                : ushort.TryParse(configuration["Mssql:ContainerPort"], out var containerPort)
                    ? containerPort
                    : (ushort)1433
        };

        return databaseSettings;
    }

    /// <summary>
    /// 取得建立測試用 Redis 指定使用的 docker 設定資料
    /// </summary>
    /// <returns></returns>
    public static Redis GetRedisSetting()
    {
        var configuration = GetConfiguration();

        var redisSetting = new Redis
        {
            Image = configuration["Redis:Image"],
            Tag = configuration["Redis:Tag"],
            ContainerName = configuration["Redis:ContainerName"],
            HostPort = string.IsNullOrWhiteSpace(configuration["Redis:HostPort"])
                ? GetRandomPort()
                : configuration["Redis:HostPort"].Equals("0")
                    ? GetRandomPort()
                    : ushort.TryParse(configuration["Redis:HostPort"], out var redisPort)
                        ? redisPort
                        : GetRandomPort(),
            ContainerPort = string.IsNullOrWhiteSpace(configuration["Redis:ContainerPort"])
                ? (ushort)6379
                : ushort.TryParse(configuration["Redis:ContainerPort"], out var containerPort)
                    ? containerPort
                    : (ushort)6379
        };

        return redisSetting;
    }

    /// <summary>
    /// Get Random Port
    /// </summary>
    /// <returns></returns>
    public static ushort GetRandomPort()
    {
        var result = Random.Shared.Next(49152, 65535);
        return (ushort)result;
    }

    /// <summary>
    /// Get ConfigurationRoot
    /// </summary>
    /// <returns></returns>
    private static IConfigurationRoot GetConfiguration()
    {
        var configuration = new ConfigurationBuilder().AddJsonFile(SettingFile).Build();
        return configuration;
    }
}