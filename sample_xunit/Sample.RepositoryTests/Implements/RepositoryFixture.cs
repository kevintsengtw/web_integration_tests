using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Sample.Repository.Helpers;
using Sample.RepositoryTests.Utilities.Database;

namespace Sample.RepositoryTests.Implements;

/// <summary>
/// Class RepositoryFixture
/// </summary>
public class RepositoryFixture
{
    internal static IFixture Fixture => new Fixture().Customize(new AutoNSubstituteCustomization())
                                                     .Customize(new DatabaseHelperCustomization());

    internal static IDatabaseHelper DatabaseHelper => Fixture.Create<DatabaseHelper>();
}