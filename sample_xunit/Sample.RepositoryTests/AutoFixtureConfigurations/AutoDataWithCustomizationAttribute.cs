using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;

namespace Sample.RepositoryTests.AutoFixtureConfigurations;

/// <summary>
/// class AutoDataWithCustomizationAttribute
/// </summary>
public class AutoDataWithCustomizationAttribute : AutoDataAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutoDataWithCustomizationAttribute"/> class
    /// </summary>
    public AutoDataWithCustomizationAttribute() : base(() =>
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization())
                                   .Customize(new DatabaseHelperCustomization());

        return fixture;
    })
    {
    }
}