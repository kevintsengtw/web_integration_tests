using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;

namespace Sample.RepositoryTests.AutoFixtureConfigurations;

/// <summary>
/// class AutoDataWithCustomizationAttribute
/// </summary>
/// <remarks>
/// https://github.com/AutoFixture/AutoFixture/discussions/1191
/// https://github.com/keghub/dotnet-utils/blob/master/tests/Tests.ServiceModel/CustomAutoDataAttribute.cs
/// https://stackoverflow.com/questions/54261179/when-setting-up-a-custom-autodataattribute-for-auto-mocking-whats-the-proper-s
/// https://stackoverflow.com/questions/31136031/can-i-customise-a-fixture-in-an-xunit-constructor-for-use-with-theory-and-autoda
/// </remarks>
public class AutoDataWithCustomizationAttribute : AutoDataAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutoDataWithCustomizationAttribute"/> class
    /// </summary>
    public AutoDataWithCustomizationAttribute()
        : base(CreateFixture)
    {
    }

    public AutoDataWithCustomizationAttribute(params Type[] customizationTypes)
        : base(() => CreateFixtureWithCustomizationTypes(customizationTypes))
    {
    }

    private static IFixture CreateFixture()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        return fixture;
    }

    private static IFixture CreateFixtureWithCustomizationTypes(params Type[] customizationTypes)
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
    }
}