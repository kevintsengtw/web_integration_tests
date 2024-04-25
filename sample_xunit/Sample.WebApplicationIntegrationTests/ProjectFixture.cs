using System.IO.Abstractions;
using DotNet.Testcontainers.Containers;
using FluentAssertions;
using Sample.WebApplicationIntegrationTests.DatabaseUtilities;
using Sample.WebApplicationIntegrationTests.TestData;
using Sample.WebApplicationIntegrationTests.Utilities;

namespace Sample.WebApplicationIntegrationTests;

[CollectionDefinition(nameof(ProjectCollectionFixture))]
public class ProjectCollectionFixture : ICollectionFixture<ProjectFixture>
{
}

public class ProjectFixture : IAsyncLifetime
{
    private static IFileSystem FileSystem => new FileSystem();

    private static string SettingFile => FileSystem.Path.Combine("Settings", "TestSettings.json");

    private static string DatabaseIp { get; set; }

    private static string DatabaseSaPassword { get; set; }

    private static string DatabaseName => "Sample";

    private static string MasterConnectionString =>
        string.Format(TestDbConnection.Connections.Master, DatabaseIp, DatabaseSaPassword);

    internal static string SampleDbConnectionString =>
        string.Format(TestDbConnection.Connections.Database, DatabaseIp, DatabaseName, DatabaseSaPassword);


    internal static int RedisPort { get; private set; } = 0;

    private static IContainer _mssqlContainer;

    private static IContainer _redisContainer;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectFixture"/> class
    /// </summary>
    public ProjectFixture()
    {
        TestSettingProvider.SettingFile = SettingFile;

        //-- Create Redis Container
        var redisSettings = TestSettingProvider.GetRedisSetting();
        var redisResult = RedisFixture.CreateContainer(redisSettings, typeof(ProjectFixture));
        _redisContainer = redisResult.ContainerService;
        RedisPort = redisResult.RedisPort;

        // Create Mssql Server
        var databaseSettings = TestSettingProvider.GetDatabaseSettings();
        DatabaseIp = $"127.0.0.1,{databaseSettings.HostPort}";
        DatabaseSaPassword = databaseSettings.SaPassword;
        _mssqlContainer = MssqlFixture.CreateContainer(databaseSettings, typeof(ProjectFixture));
    }

    /// <summary>
    /// Initializes this instance
    /// </summary>
    public async Task InitializeAsync()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        await _redisContainer.StartAsync(cts.Token);
        await _mssqlContainer.StartAsync(cts.Token);

        DatabaseCommand.PrintMssqlVersion(MasterConnectionString);

        // 在 container 裡的 SQL Server 建立測試用 Database
        DatabaseHelper.CreateDatabase(MasterConnectionString, DatabaseName);

        //-- Create Tables & Insert Data to Database
        DatabaseHelper.CreateTables(SampleDbConnectionString, TableNames.TableNameCollection);

        SetupDateAssertions();
    }

    /// <summary>
    /// Disposes this instance
    /// </summary>
    public async Task DisposeAsync()
    {
        await _redisContainer.DisposeAsync();
        await _mssqlContainer.DisposeAsync();
    }

    //---------------------------------------------------------------------------------------------

    /// <summary>
    /// FluentAssertions - Setup DateTime AssertionOptions
    /// </summary>
    private static void SetupDateAssertions()
    {
        // FluentAssertions 設定 : 日期時間使用接近比對的方式，而非完全一致的比對
        AssertionOptions.AssertEquivalencyUsing(options =>
        {
            options.Using<DateTime>(
                       ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(1000)))
                   .WhenTypeIs<DateTime>();

            options.Using<DateTimeOffset>(
                       ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(1000)))
                   .WhenTypeIs<DateTimeOffset>();

            return options;
        });
    }
}