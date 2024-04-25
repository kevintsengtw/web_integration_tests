using System;
using System.IO.Abstractions;
using Sample.RepositoryTests.TestData;

namespace Sample.RepositoryTests.Utilities.Database;

/// <summary>
/// Class DatabaseUtilities
/// </summary>
public static class DatabaseUtilities
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
    /// <param name="databaseConnectionString">The databaseConnectionString</param>
    public static void CreateTables(string databaseConnectionString)
    {
        //-- Create Tables
        foreach (var tableName in TableNames.TableNameCollection)
        {
            var createTableScript = FileSystem.Path.Combine("TestData", "TableSchemas", $"Sample_{tableName}_Create.sql");
            DatabaseCommand.ExecuteDbScript(databaseConnectionString, createTableScript);
        }

        Thread.Sleep(2000);
    }
}