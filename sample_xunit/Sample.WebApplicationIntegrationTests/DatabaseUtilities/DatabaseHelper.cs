using System.IO.Abstractions;

namespace Sample.WebApplicationIntegrationTests.DatabaseUtilities;

/// <summary>
/// Class DatabaseHelper
/// </summary>
public class DatabaseHelper
{
    /// <summary>
    /// Gets the value of the file system
    /// </summary>
    private static IFileSystem FileSystem => new FileSystem();

    /// <summary>
    /// Creaate Database.
    /// </summary>
    /// <param name="masterConnectionString">The masterConnectionString</param>
    /// <param name="databaseName">The databaseName</param>
    public static void CreateDatabase(string masterConnectionString, string databaseName)
    {
        DatabaseCommand.CreateDatabase(masterConnectionString, databaseName);
        Thread.Sleep(2000);
    }

    /// <summary>
    /// Create Tables (and Insert Channel, Platform default data).
    /// </summary>
    /// <param name="commonDbConnectionString">The commonDbConnectionString</param>
    /// <param name="tableNames">The tableNames</param>
    public static void CreateTables(string commonDbConnectionString, IEnumerable<string> tableNames)
    {
        //-- Create Tables
        foreach (var tableName in tableNames)
        {
            var createTableScript = FileSystem.Path.Combine("TestData", "TableSchemas", $"Sample_{tableName}_Create.sql");
            DatabaseCommand.ExecuteDbScript(commonDbConnectionString, createTableScript);
        }

        Thread.Sleep(2000);
    }
}