using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;

namespace Sample.ServiceTests.Utilities;

/// <summary>
/// class AutoTestingDataAttribute
/// </summary>
/// <seealso cref="AutoDataAttribute"/>
public class AutoTestingDataAttribute : AutoDataAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutoTestingDataAttribute"/> class
    /// </summary>
    public AutoTestingDataAttribute() : base(() =>
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