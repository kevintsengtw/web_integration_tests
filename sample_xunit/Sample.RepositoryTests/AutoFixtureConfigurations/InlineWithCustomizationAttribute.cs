using AutoFixture.Xunit2;

namespace Sample.RepositoryTests.AutoFixtureConfigurations;

/// <summary>
/// class InlineWithCustomizationAttribute
/// </summary>
/// <seealso cref="InlineAutoDataAttribute"/>
public class InlineWithCustomizationAttribute : InlineAutoDataAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InlineWithCustomizationAttribute"/> class
    /// </summary>
    /// <param name="values">The values</param>
    public InlineWithCustomizationAttribute(params object[] values)
        : base(new AutoDataWithCustomizationAttribute(), values)
    {
    }
}