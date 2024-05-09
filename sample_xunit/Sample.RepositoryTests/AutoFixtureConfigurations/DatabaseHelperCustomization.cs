using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.Extensions.Options;
using Sample.Domain.Settings;
using Sample.Repository.Helpers;

namespace Sample.RepositoryTests.AutoFixtureConfigurations;

/// <summary>
/// class DatabaseHelperCustomization
/// </summary>
public class DatabaseHelperCustomization : ICustomization
{
    private static string ConnectionString => ProjectFixture.SampleDbConnectionString;

    private DatabaseHelper DatabaseHelper
    {
        get
        {
            var databaseConnectionOptions = new DatabaseConnectionOptions { ConnectionString = ConnectionString };
            var options = Options.Create(databaseConnectionOptions);
            var databaseHelper = new DatabaseHelper(options);
            return databaseHelper;
        }
    }

    public void Customize(IFixture fixture)
    {
        fixture.Customizations.Add(new TypeRelay(typeof(IDatabaseHelper), typeof(DatabaseHelper)));
        fixture.Register(() => DatabaseHelper);
    }
}