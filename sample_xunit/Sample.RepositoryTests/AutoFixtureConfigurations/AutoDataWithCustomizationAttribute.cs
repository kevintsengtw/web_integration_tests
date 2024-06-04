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

    public AutoDataWithCustomizationAttribute(params Type[] customizationTypes) : base(() =>
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

        foreach (var type in customizationTypes)
        {
            if (typeof(ICustomization).IsAssignableFrom(type) && Activator.CreateInstance(type) is ICustomization customization)
            {
                fixture.Customize(customization);
            }
            else
            {
                throw new ArgumentException($"Type {type.Name} does not implement ICustomization.");
            }
        }

        return fixture;
    })
    {
    }
}