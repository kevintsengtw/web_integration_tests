using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;

namespace Sample.ServiceTests.AutoFixtureConfigurations;

/// <summary>
/// class AutoDateWithCustomizationTypeAttribute
/// </summary>
public class AutoDateWithCustomizationTypeAttribute : AutoDataAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutoDateWithCustomizationTypeAttribute"/> class
    /// </summary>
    /// <param name="customizationType">The customization type</param>
    public AutoDateWithCustomizationTypeAttribute(Type customizationType) : base(() =>
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization())
                                   .Customize(new MapsterMapperCustomization())
                                   .Customize(new FakeTimeProviderCustomization());

        var customization = Activator.CreateInstance(customizationType) as ICustomization;

        fixture.Customize(customization);

        return fixture;
    })
    {
    }
}