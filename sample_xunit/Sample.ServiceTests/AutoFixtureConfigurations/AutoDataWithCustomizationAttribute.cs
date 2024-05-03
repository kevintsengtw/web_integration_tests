using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;

namespace Sample.ServiceTests.AutoFixtureConfigurations;

/// <summary>
/// class AutoDataWithCustomizationAttribute
/// </summary>
/// <seealso cref="AutoDataAttribute"/>
public class AutoDataWithCustomizationAttribute : AutoDataAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutoDataWithCustomizationAttribute"/> class
    /// </summary>
    public AutoDataWithCustomizationAttribute() : base(() =>
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization())
                                   .Customize(new MapsterMapperCustomization());

        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
               .ForEach(x => fixture.Behaviors.Remove(x));

        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        return fixture;
    })
    {
    }
}