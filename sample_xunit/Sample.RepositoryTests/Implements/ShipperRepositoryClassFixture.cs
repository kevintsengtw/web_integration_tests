﻿using Sample.Domain.Entities;
using Sample.RepositoryTests.TestData;
using Sample.RepositoryTests.Utilities.Database;

namespace Sample.RepositoryTests.Implements;

/// <summary>
/// class ShipperRepositoryClassFixture
/// </summary>
public class ShipperRepositoryClassFixture : IDisposable
{
    public ShipperRepositoryClassFixture()
    {
        CreateTable();
    }

    public void Dispose()
    {
        DropTable();
    }

    private void CreateTable()
    {
        TableCommands.Drop(ProjectFixture.SampleDbConnectionString, TableNames.Shippers);

        var filePath = Path.Combine("TestData", "TableSchemas", "Sample_Shippers_Create.sql");
        var script = File.ReadAllText(filePath);
        TableCommands.Create(ProjectFixture.SampleDbConnectionString, script);
    }

    private void DropTable()
    {
        TableCommands.Drop(ProjectFixture.SampleDbConnectionString, TableNames.Shippers);
    }

    public static void InsertData(ShipperModel model)
    {
        const string sqlCommand =
            """
            SET IDENTITY_INSERT dbo.Shippers ON
            Insert into Shippers (ShipperID, CompanyName, Phone)
            Values (@ShipperID, @CompanyName, @Phone);
            SET IDENTITY_INSERT dbo.Shippers OFF
            """;

        DatabaseCommand.ExecuteSqlCommand(ProjectFixture.SampleDbConnectionString, sqlCommand, model);
    }
}