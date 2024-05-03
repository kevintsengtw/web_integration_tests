using AutoFixture;
using AutoFixture.AutoNSubstitute;

namespace Sample.ServiceTests.Utilities;

/// <summary>
/// class AutoFixtureHelper
/// </summary>
public static class AutoFixtureHelper
{
    /// <summary>
    /// Create the fixture instance.
    /// </summary>
    /// <returns>The fixture</returns>
    public static IFixture Create() 
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization())
                                   .Customize(new MapsterMapperCustomization());

        return fixture;
    }
}