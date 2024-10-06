using AutoFixture.Xunit2;

namespace Sample.ServiceTests.AutoFixtureConfigurations;

/// <summary>
/// class InlineAutoDataWithCustomizationAttribute
/// </summary>
/// <seealso cref="InlineAutoDataAttribute"/>
public class InlineAutoDataWithCustomizationAttribute : InlineAutoDataAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InlineAutoDataWithCustomizationAttribute"/> class
    /// </summary>
    /// <param name="values">The values</param>
    public InlineAutoDataWithCustomizationAttribute(params object[] values)
        : base(new AutoDataWithCustomizationAttribute(), values)
    {
    }
}