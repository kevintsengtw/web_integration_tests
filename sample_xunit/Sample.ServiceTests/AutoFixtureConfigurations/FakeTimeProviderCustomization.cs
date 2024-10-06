using AutoFixture;
using Microsoft.Extensions.Time.Testing;

namespace Sample.ServiceTests.AutoFixtureConfigurations;

/// <summary>
/// class FakeTimeProviderCustomization
/// </summary>
public class FakeTimeProviderCustomization : ICustomization
{
    /// <summary>
    /// Customizes the fixture
    /// </summary>
    /// <param name="fixture">The fixture</param>
    public void Customize(IFixture fixture)
    {
        fixture.Register(() => new FakeTimeProvider());
    }
}