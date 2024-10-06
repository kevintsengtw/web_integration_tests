using AutoFixture;
using Microsoft.Extensions.Time.Testing;
using Sample.Domain.Repositories;
using Sample.Service.Implements;

namespace Sample.ServiceTests.AutoFixtureConfigurations;

/// <summary>
/// class TradeServiceCustomization
/// </summary>
public class TradeServiceCustomization : ICustomization
{
    /// <summary>
    /// Customizes the fixture
    /// </summary>
    /// <param name="fixture">The fixture</param>
    public void Customize(IFixture fixture)
    {
        fixture.Register(() =>
        {
            var fakeTimeProvider = fixture.Create<FakeTimeProvider>();
            var holidayRepository = fixture.Create<IHolidayRepository>();
            var sut = new TradeService(fakeTimeProvider, holidayRepository);
            return sut;
        });
    }
}