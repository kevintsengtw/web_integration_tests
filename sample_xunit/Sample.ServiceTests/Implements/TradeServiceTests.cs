using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using Sample.Domain.Repositories;
using Sample.Service.Implements;
using Sample.ServiceTests.AutoFixtureConfigurations;

namespace Sample.ServiceTests.Implements;

public class TradeServiceTests
{
    private static DateTime[] Holidays
    {
        get
        {
            var holidays = new[]
            {
                new DateTime(2024, 9, 1, 0, 0, 0, DateTimeKind.Local), new DateTime(2024, 9, 2, 0, 0, 0, DateTimeKind.Local),
                new DateTime(2024, 9, 7, 0, 0, 0, DateTimeKind.Local), new DateTime(2024, 9, 8, 0, 0, 0, DateTimeKind.Local),
                new DateTime(2024, 9, 14, 0, 0, 0, DateTimeKind.Local), new DateTime(2024, 9, 15, 0, 0, 0, DateTimeKind.Local),
                new DateTime(2024, 9, 21, 0, 0, 0, DateTimeKind.Local), new DateTime(2024, 9, 22, 0, 0, 0, DateTimeKind.Local),
                new DateTime(2024, 9, 28, 0, 0, 0, DateTimeKind.Local), new DateTime(2024, 9, 29, 0, 0, 0, DateTimeKind.Local)
            };
            return holidays;
        }
    }
    
    [Theory]
    [AutoDataWithCustomization]
    public void IsTradeNow_取得的目前時間為假日_應回傳false(
        [Frozen(Matching.DirectBaseType)] FakeTimeProvider fakeTimeProvider,
        [Frozen] IHolidayRepository holidayRepository,
        TradeService sut)
    {
        // arrange
        var currentTime = new DateTime(2024, 9, 28, 0, 0, 0, DateTimeKind.Local);
        fakeTimeProvider.SetLocalTimeZone(TimeZoneInfo.Local);
        fakeTimeProvider.SetUtcNow(TimeZoneInfo.ConvertTimeToUtc(currentTime));
        
        holidayRepository.GetHolidays(Arg.Any<int>(), Arg.Any<int>()).Returns(Holidays);
        
        // act
        var actual = sut.IsTradeNow();
        
        // assert
        actual.Should().BeFalse();
    }

    [Theory]
    [AutoDataWithCustomization]
    public void IsTradeNow_取得的目前時間不是假日_且時間沒有超過下午三點半_應回傳true(
        [Frozen(Matching.DirectBaseType)] FakeTimeProvider fakeTimeProvider,
        [Frozen] IHolidayRepository holidayRepository,
        TradeService sut)
    {
        // arrange
        var currentTime = new DateTime(2024, 9, 24, 14, 0, 0, DateTimeKind.Local);
        fakeTimeProvider.SetLocalTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Asia/Taipei"));
        fakeTimeProvider.SetUtcNow(TimeZoneInfo.ConvertTimeToUtc(currentTime));
        
        holidayRepository.GetHolidays(Arg.Any<int>(), Arg.Any<int>()).Returns(Holidays);
        
        // act
        var actual = sut.IsTradeNow();
        
        // assert
        actual.Should().BeTrue();
    }
    
    [Theory]
    [AutoDataWithCustomization]
    public void IsTradeNow_取得的目前時間不是假日_但時間已經超過下午三點半_應回傳false(
        [Frozen(Matching.DirectBaseType)] FakeTimeProvider fakeTimeProvider,
        [Frozen] IHolidayRepository holidayRepository,
        TradeService sut)
    {
        // arrange
        var currentTime = new DateTime(2024, 9, 24, 16, 0, 0, DateTimeKind.Local);
        fakeTimeProvider.SetLocalTimeZone(TimeZoneInfo.Local);
        fakeTimeProvider.SetUtcNow(TimeZoneInfo.ConvertTimeToUtc(currentTime));
        
        holidayRepository.GetHolidays(Arg.Any<int>(), Arg.Any<int>()).Returns(Holidays);
        
        // act
        var actual = sut.IsTradeNow();
        
        // assert
        actual.Should().BeFalse();
    }
}