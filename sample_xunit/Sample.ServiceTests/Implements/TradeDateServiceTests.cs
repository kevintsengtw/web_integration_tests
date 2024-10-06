using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using Sample.Domain.Entities;
using Sample.Domain.Repositories;
using Sample.Service.Implements;
using Sample.ServiceTests.AutoFixtureConfigurations;

namespace Sample.ServiceTests.Implements;

public class TradeDateServiceTests
{
    [Theory]
    [AutoDataWithCustomization]
    public async Task GetAsync_test1(
        [Frozen(Matching.DirectBaseType)] FakeTimeProvider fakeTimeProvider,
        [Frozen] ITradeDateApiRepository tradeDateApiRepository,
        TradeDateService sut)
    {
        // arrange
        var tradeDate = new DateTime(2024, 9, 22);
        var count = 2;

        fakeTimeProvider.SetUtcNow(new DateTime(2024, 9, 22, 14, 00, 00, DateTimeKind.Local));

        tradeDateApiRepository.GetAsync(Arg.Any<DateTime>(), Arg.Any<int>())
                              .Returns(new TradeDateModel
                              {
                                  TradeDate = new List<int>
                                  {
                                      20240923,
                                      20240924
                                  }
                              });

        // act
        var actual = await sut.GetAsync(tradeDate, count);

        // assert
        actual.Should().Be("2024-09-23");
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task GetAsync_test2(
        [Frozen(Matching.DirectBaseType)] FakeTimeProvider fakeTimeProvider,
        [Frozen] ITradeDateApiRepository tradeDateApiRepository,
        TradeDateService sut)
    {
        // arrange
        var tradeDate = new DateTime(2024, 9, 23);
        var count = 2;

        fakeTimeProvider.SetUtcNow(new DateTime(2024, 9, 22, 14, 00, 00, DateTimeKind.Local));

        tradeDateApiRepository.GetAsync(Arg.Any<DateTime>(), Arg.Any<int>())
                              .Returns(new TradeDateModel
                              {
                                  TradeDate = new List<int>
                                  {
                                      20240923,
                                      20240924
                                  }
                              });

        // act
        var actual = await sut.GetAsync(tradeDate, count);

        // assert
        actual.Should().Be("2024-09-23");
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task GetAsync_test3(
        [Frozen(Matching.DirectBaseType)] FakeTimeProvider fakeTimeProvider,
        [Frozen] ITradeDateApiRepository tradeDateApiRepository,
        TradeDateService sut)
    {
        // arrange
        var tradeDate = new DateTime(2024, 9, 23);
        var count = 2;

        fakeTimeProvider.SetUtcNow(new DateTime(2024, 9, 22, 16, 00, 00, DateTimeKind.Local));

        tradeDateApiRepository.GetAsync(Arg.Any<DateTime>(), Arg.Any<int>())
                              .Returns(new TradeDateModel
                              {
                                  TradeDate = new List<int>
                                  {
                                      20240923,
                                      20240924
                                  }
                              });

        // act
        var actual = await sut.GetAsync(tradeDate, count);

        // assert
        actual.Should().Be("2024-09-24");
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task GetAsync_test4(
        [Frozen(Matching.DirectBaseType)] FakeTimeProvider fakeTimeProvider,
        [Frozen] ITradeDateApiRepository tradeDateApiRepository,
        TradeDateService sut)
    {
        // arrange
        var tradeDate = new DateTime(2024, 9, 22);
        var count = 2;

        fakeTimeProvider.SetUtcNow(new DateTime(2024, 9, 22, 14, 00, 00, DateTimeKind.Local));

        tradeDateApiRepository.GetAsync(Arg.Any<DateTime>(), Arg.Any<int>())
                              .Returns(new TradeDateModel
                              {
                                  TradeDate = new List<int>
                                  {
                                      20240923,
                                      20240924
                                  }
                              });

        // act
        var actual = await sut.GetAsync(tradeDate, count);

        // assert
        actual.Should().Be("2024-09-23");
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task GetAsync_test5(
        [Frozen(Matching.DirectBaseType)] FakeTimeProvider fakeTimeProvider,
        [Frozen] ITradeDateApiRepository tradeDateApiRepository,
        TradeDateService sut)
    {
        // arrange
        var tradeDate = new DateTime(2024, 9, 23);
        var count = 2;

        fakeTimeProvider.SetUtcNow(new DateTime(2024, 9, 22, 14, 00, 00, DateTimeKind.Local));

        tradeDateApiRepository.GetAsync(Arg.Any<DateTime>(), Arg.Any<int>())
                              .Returns(new TradeDateModel
                              {
                                  TradeDate = new List<int>
                                  {
                                      20240923,
                                      20240924
                                  }
                              });

        // act
        var actual = await sut.GetAsync(tradeDate, count);

        // assert
        actual.Should().Be("2024-09-23");
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task GetAsync_test6(
        [Frozen(Matching.DirectBaseType)] FakeTimeProvider fakeTimeProvider,
        [Frozen] ITradeDateApiRepository tradeDateApiRepository,
        TradeDateService sut)
    {
        // arrange
        var tradeDate = new DateTime(2024, 9, 23);
        var count = 2;

        fakeTimeProvider.SetUtcNow(new DateTime(2024, 9, 22, 16, 00, 00, DateTimeKind.Local));

        tradeDateApiRepository.GetAsync(Arg.Any<DateTime>(), Arg.Any<int>())
                              .Returns(new TradeDateModel
                              {
                                  TradeDate = new List<int>
                                  {
                                      20240923,
                                      20240924
                                  }
                              });

        // act
        var actual = await sut.GetAsync(tradeDate, count);

        // assert
        actual.Should().Be("2024-09-24");
    }
}