using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using NSubstitute;
using Sample.Domain.Entities;
using Sample.Domain.Misc;
using Sample.Service.Dto;
using Sample.TestResource.AutoFixture;

namespace Sample.ServiceTests.Implements;

public class ShipperServiceTests : IClassFixture<ShipperServiceClassFixture>
{
    private readonly ShipperServiceClassFixture _classFixture;

    private readonly StubShipperService _stub;

    public ShipperServiceTests(ShipperServiceClassFixture classFixture)
    {
        this._classFixture = classFixture;
        this._stub = classFixture.Stub;
    }

    //---------------------------------------------------------------------------------------------
    // IsExistsAsync

    [Fact]
    public async Task IsExistsAsync_輸入的ShipperId為0時_應拋出ArgumentOutOfRangeException()
    {
        // arrange
        var shipperId = 0;

        // act
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => this._stub.SystemUnderTest.IsExistsAsync(shipperId));

        // assert
        exception.Message.Should().Contain(nameof(shipperId));
    }

    [Fact]
    public async Task IsExistsAsync_輸入的ShipperId為負1時_應拋出ArgumentOutOfRangeException()
    {
        // arrange
        var shipperId = -1;

        // act
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => this._stub.SystemUnderTest.IsExistsAsync(shipperId));

        // assert
        exception.Message.Should().Contain(nameof(shipperId));
    }

    [Fact]
    public async Task IsExistsAsync_輸入的ShipperId_資料不存在_應回傳false()
    {
        // arrange
        var shipperId = 99;

        this._stub.ShipperRepository.IsExistsAsync(Arg.Any<int>()).Returns(false);

        // act
        var actual = await this._stub.SystemUnderTest.IsExistsAsync(shipperId);

        // assert
        actual.Should().BeFalse();
    }

    [Fact]
    public async Task IsExistsAsync_輸入的ShipperId_資料有存在_應回傳True()
    {
        // arrange
        var shipperId = 99;

        this._stub.ShipperRepository.IsExistsAsync(Arg.Any<int>()).Returns(true);

        // act
        var actual = await this._stub.SystemUnderTest.IsExistsAsync(shipperId);

        // assert
        actual.Should().BeTrue();
    }

    //---------------------------------------------------------------------------------------------
    // GetAsync

    [Fact]
    public async Task GetAsync_輸入的ShipperId為0時_應拋出ArgumentOutOfRangeException()
    {
        // arrange
        var shipperId = 0;

        // act
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => this._stub.SystemUnderTest.GetAsync(shipperId));

        // assert
        exception.Message.Should().Contain(nameof(shipperId));
    }

    [Fact]
    public async Task GetAsync_輸入的ShipperId為負1時_應拋出ArgumentOutOfRangeException()
    {
        // arrange
        var shipperId = -1;

        // act
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => this._stub.SystemUnderTest.GetAsync(shipperId));

        // assert
        exception.Message.Should().Contain(nameof(shipperId));
    }

    [Fact]
    public async Task GetAsync_輸入的ShipperId_資料不存在_應回傳null()
    {
        // arrange
        var shipperId = 99;

        this._stub.ShipperRepository.IsExistsAsync(Arg.Any<int>()).Returns(false);

        // act
        var actual = await this._stub.SystemUnderTest.GetAsync(shipperId);

        // assert
        actual.Should().BeNull();
    }

    [Theory]
    [AutoData]
    public async Task GetAsync_輸入的ShipperId_資料有存在_應回傳model(ShipperModel model)
    {
        // arrange
        var shipperId = model.ShipperId;

        this._stub.ShipperRepository.IsExistsAsync(Arg.Any<int>()).Returns(true);
        this._stub.ShipperRepository.GetAsync(Arg.Any<int>()).Returns(model);

        // act
        var actual = await this._stub.SystemUnderTest.GetAsync(shipperId);

        // assert
        actual.Should().NotBeNull();
        actual.ShipperId.Should().Be(shipperId);
    }

    //---------------------------------------------------------------------------------------------
    // GetTotalCountAsync

    [Fact]
    public async Task GetTotalCountAsync_資料表裡無資料_應回傳0()
    {
        // arrange
        var expected = 0;

        // act
        var actual = await this._stub.SystemUnderTest.GetTotalCountAsync();

        // assert
        actual.Should().Be(expected);
    }

    [Fact]
    public async Task GetTotalCountAsync_資料表裡有10筆資料_應回傳10()
    {
        // arrange
        var expected = 10;

        this._stub.ShipperRepository.GetTotalCountAsync().Returns(10);

        // act
        var actual = await this._stub.SystemUnderTest.GetTotalCountAsync();

        // assert
        actual.Should().Be(expected);
    }

    //---------------------------------------------------------------------------------------------
    // GetAllAsync

    [Fact]
    public async Task GetAllAsync_資料表裡無資料_應回傳空集合()
    {
        // arrange

        // act
        var actual = await this._stub.SystemUnderTest.GetAllAsync();

        // assert
        actual.Should().BeEmpty();
    }

    [Theory]
    [AutoData]
    public async Task GetAllAsync_資料表裡有10筆資料_回傳的集合裡有10筆([CollectionSize(10)] IEnumerable<ShipperModel> models)
    {
        // arrange
        this._stub.ShipperRepository.GetAllAsync().Returns(models);

        // act
        var actual = await this._stub.SystemUnderTest.GetAllAsync();

        // assert
        actual.Should().NotBeEmpty();
        actual.Should().HaveCount(10);
    }

    //---------------------------------------------------------------------------------------------
    // GetCollectionAsync

    [Theory]
    [InlineData(0, 10, nameof(from))]
    [InlineData(-1, 10, nameof(from))]
    [InlineData(1, 0, nameof(size))]
    [InlineData(1, -1, nameof(size))]
    public async Task GetCollectionAsync_from與size輸入不合規格內容_應拋出ArgumentOutOfRangeException(
        int from, int size, string parameterName)
    {
        // act
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => this._stub.SystemUnderTest.GetCollectionAsync(from, size));

        // assert
        exception.Message.Should().Contain(parameterName);
    }

    [Fact]
    public async Task GetCollectionAsync_from為1_size為10_資料表裡無資料_應回傳空集合()
    {
        // arrange
        const int from = 1;
        const int size = 10;

        this._stub.ShipperRepository.GetTotalCountAsync().Returns(0);

        // act
        var actual = await this._stub.SystemUnderTest.GetCollectionAsync(from, size);

        // assert
        actual.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCollectionAsync_from為20_size為10_資料表裡只有10筆資料_from超過總數量_應回傳空集合()
    {
        // arrange
        const int from = 20;
        const int size = 10;

        this._stub.ShipperRepository.GetTotalCountAsync().Returns(10);

        // act
        var actual = await this._stub.SystemUnderTest.GetCollectionAsync(from, size);

        // assert
        actual.Should().BeEmpty();
    }

    [Theory]
    [AutoData]
    public async Task GetCollectionAsync_from為1_size為10_資料表裡有5筆資料_回傳集合應有5筆(
        [CollectionSize(5)] IEnumerable<ShipperModel> models)
    {
        // arrange
        const int from = 1;
        const int size = 10;

        this._stub.ShipperRepository.GetTotalCountAsync().Returns(5);

        this._stub.ShipperRepository
            .GetCollectionAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(models);

        // act
        var actual = await this._stub.SystemUnderTest.GetCollectionAsync(from, size);

        // assert
        actual.Should().NotBeEmpty();
        actual.Should().HaveCount(5);
    }

    [Theory]
    [AutoData]
    public async Task GetCollectionAsync_from為6_size為10_資料表裡有10筆資料_回傳集合應有10筆(
        [CollectionSize(20)] IEnumerable<ShipperModel> models)
    {
        // arrange
        const int from = 6;
        const int size = 10;

        this._stub.ShipperRepository.GetTotalCountAsync().Returns(10);

        this._stub.ShipperRepository
            .GetCollectionAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(models.Skip(5).Take(10));

        // act
        var actual = await this._stub.SystemUnderTest.GetCollectionAsync(from, size);

        // assert
        actual.Should().NotBeEmpty();
        actual.Should().HaveCount(10);
    }

    [Theory]
    [AutoData]
    public async Task GetCollectionAsync_from為11_size為10_資料表裡有30筆資料_回傳集合應有10筆(
        [CollectionSize(30)] IEnumerable<ShipperModel> models)
    {
        // arrange
        const int from = 11;
        const int size = 10;

        this._stub.ShipperRepository.GetTotalCountAsync().Returns(30);

        this._stub.ShipperRepository
            .GetCollectionAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(models.Skip(10).Take(10));

        // act
        var actual = await this._stub.SystemUnderTest.GetCollectionAsync(from, size);

        // assert
        actual.Should().NotBeEmpty();
        actual.Should().HaveCount(10);
    }

    //---------------------------------------------------------------------------------------------
    // SearchAsync

    [Theory]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData(null, "")]
    [InlineData(null, null)]
    public async Task SearchAsync_companyName與phone輸入不合規格的內容_應拋出ArgumentException(string companyName, string phone)
    {
        // arrange
        const string exceptionMessage = "companyName 與 phone 不可都為空白";

        // act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => this._stub.SystemUnderTest.SearchAsync(companyName, phone));

        // assert
        exception.Message.Should().Be(exceptionMessage);
    }

    [Fact]
    public async Task SearchAsync_資料表裡無資料_應回傳空集合()
    {
        // arrange
        const string companyName = "test";
        const string phone = "02123456789";

        this._stub.ShipperRepository.GetTotalCountAsync().Returns(0);

        // act
        var actual = await this._stub.SystemUnderTest.SearchAsync(companyName, phone);

        // assert
        actual.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchAsync_companyName輸入資料_沒有符合條件的資料_應回傳空集合()
    {
        // arrange
        const string companyName = "test";
        const string phone = "";

        this._stub.ShipperRepository.GetTotalCountAsync().Returns(10);

        this._stub.ShipperRepository
            .SearchAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Enumerable.Empty<ShipperModel>());

        // act
        var actual = await this._stub.SystemUnderTest.SearchAsync(companyName, phone);

        // assert
        actual.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchAsync_companyName輸入資料_phone無輸入_有符合條件的資料_回傳集合應包含符合條件的資料()
    {
        // arrange
        var models = this._classFixture.Fixture.Build<ShipperModel>()
                         .With(x => x.CompanyName, "test")
                         .CreateMany(1);

        this._stub.ShipperRepository.GetTotalCountAsync().Returns(10);

        this._stub.ShipperRepository
            .SearchAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(models);

        const string companyName = "test";
        const string phone = "";

        // act
        var actual = await this._stub.SystemUnderTest.SearchAsync(companyName, phone);

        // assert
        actual.Should().NotBeEmpty();
        actual.Should().HaveCount(1);
        actual.Any(x => x.CompanyName == companyName).Should().BeTrue();
    }

    [Fact]
    public async Task SearchAsync_companyName無輸入_phone輸入資料_有符合條件的資料_回傳集合應包含符合條件的資料()
    {
        // arrange
        var models = this._classFixture.Fixture.Build<ShipperModel>()
                         .With(x => x.Phone, "02123456789")
                         .CreateMany(1);

        this._stub.ShipperRepository.GetTotalCountAsync().Returns(10);

        this._stub.ShipperRepository
            .SearchAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(models);

        const string companyName = "";
        const string phone = "02123456789";

        // act
        var actual = await this._stub.SystemUnderTest.SearchAsync(companyName, phone);

        // assert
        actual.Should().NotBeEmpty();
        actual.Should().HaveCount(1);
        actual.Any(x => x.Phone == phone).Should().BeTrue();
    }

    [Fact]
    public async Task SearchAsync_companyName輸入資料_phone輸入資料_有符合條件的資料_回傳集合應包含符合條件的資料()
    {
        // arrange
        var models = this._classFixture.Fixture.Build<ShipperModel>()
                         .With(x => x.CompanyName, "demo")
                         .With(x => x.Phone, "03123456789")
                         .CreateMany(1);

        this._stub.ShipperRepository.GetTotalCountAsync().Returns(11);

        this._stub.ShipperRepository
            .SearchAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(models);

        const string companyName = "demo";
        const string phone = "03123456789";

        // act
        var actual = await this._stub.SystemUnderTest.SearchAsync(companyName, phone);

        // assert
        actual.Should().NotBeEmpty();
        actual.Should().HaveCount(1);
        actual.Any(x => x.CompanyName == companyName && x.Phone == phone).Should().BeTrue();
    }

    [Fact]
    public async Task SearchAsync_companyName輸入資料_phone輸入資料_沒有符合條件的資料_應回傳空集合()
    {
        // arrange
        this._stub.ShipperRepository.GetTotalCountAsync().Returns(10);

        this._stub.ShipperRepository
            .SearchAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Enumerable.Empty<ShipperModel>());

        const string companyName = "try";
        const string phone = "04123456789";

        // act
        var actual = await this._stub.SystemUnderTest.SearchAsync(companyName, phone);

        // assert
        actual.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchAsync_companyName輸入資料_phone無輸入_有2筆符合條件的資料_回傳集合應有兩筆()
    {
        // arrange
        var model1 = this._classFixture.Fixture.Build<ShipperModel>()
                         .With(x => x.CompanyName, "note")
                         .Create();

        var model2 = this._classFixture.Fixture.Build<ShipperModel>()
                         .With(x => x.CompanyName, "node")
                         .Create();

        var models = new List<ShipperModel>
        {
            model1,
            model2
        };
        
        this._stub.ShipperRepository.GetTotalCountAsync().Returns(10);

        this._stub.ShipperRepository
            .SearchAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(models);
        
        const string companyName = "no";
        const string phone = "";

        // act
        var actual = await this._stub.SystemUnderTest.SearchAsync(companyName, phone);

        // assert
        actual.Should().NotBeEmpty();
        actual.Should().HaveCount(2);
        actual.All(x => x.CompanyName.StartsWith("no")).Should().BeTrue();
    }

    //---------------------------------------------------------------------------------------------
    // CreateAsync

    [Fact]
    public async Task CreateAsync_輸入的model為null時_應拋出ArgumentNullException()
    {
        // arrange
        ShipperDto shipperDto = null;

        // act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => this._stub.SystemUnderTest.CreateAsync(shipperDto));

        // assert
        exception.Message.Should().Contain("shipper");
    }

    [Theory]
    [AutoData]
    public async Task CreateAsync_輸入一個有資料的model_新增完成_回傳Result的Success應為true(ShipperDto shipperDto)
    {
        // arrange
        this._stub.ShipperRepository
            .CreateAsync(Arg.Any<ShipperModel>())
            .Returns(new Result { Success = true, AffectRows = 1 });

        // act
        var actual = await this._stub.SystemUnderTest.CreateAsync(shipperDto);

        // assert
        actual.Success.Should().BeTrue();
        actual.AffectRows.Should().Be(1);
    }

    //---------------------------------------------------------------------------------------------
    // UpdateAsync

    [Fact]
    public async Task UpdateAsync_輸入的model為null時_應拋出ArgumentNullException()
    {
        // arrange
        ShipperDto shipperDto = null;

        // act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => this._stub.SystemUnderTest.UpdateAsync(shipperDto));

        // assert
        exception.Message.Should().Contain("shipper");
    }

    [Theory]
    [AutoData]
    public async Task UpdateAsync_輸入model_要修改的資料並不存在_更新錯誤_回傳Result的Success應為false(ShipperDto shipperDto)
    {
        // arrange
        this._stub.ShipperRepository.IsExistsAsync(Arg.Any<int>()).Returns(false);

        // act
        var actual = await this._stub.SystemUnderTest.UpdateAsync(shipperDto);

        // assert
        actual.Success.Should().BeFalse();
        actual.Message.Should().Be("shipper not exists");
    }

    [Theory]
    [AutoData]
    public async Task UpdateAsync_輸入model_要修改的資料存在_更新完成_回傳Result的Success應為true(ShipperDto shipperDto)
    {
        // arrange
        shipperDto.CompanyName = "update";

        this._stub.ShipperRepository.IsExistsAsync(Arg.Any<int>()).Returns(true);

        this._stub.ShipperRepository
            .UpdateAsync(Arg.Any<ShipperModel>())
            .Returns(new Result { Success = true, AffectRows = 1 });

        // act
        var actual = await this._stub.SystemUnderTest.UpdateAsync(shipperDto);

        // assert
        actual.Success.Should().BeTrue();
    }

    //---------------------------------------------------------------------------------------------
    // DeleteAsync

    [Fact]
    public async Task DeleteAsync_輸入的ShipperId為0時_應拋出ArgumentOutOfRangeException()
    {
        // arrange
        var shipperId = 0;

        // act
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => this._stub.SystemUnderTest.DeleteAsync(shipperId));

        // assert
        exception.Message.Should().Contain(nameof(shipperId));
    }

    [Fact]
    public async Task DeleteAsync_輸入的ShipperId為負1時_應拋出ArgumentOutOfRangeException()
    {
        // arrange
        var shipperId = -1;

        // act
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => this._stub.SystemUnderTest.DeleteAsync(shipperId));

        // assert
        exception.Message.Should().Contain(nameof(shipperId));
    }

    [Fact]
    public async Task DeleteAsync_輸入ShipperId_要刪除的資料並不存在_刪除錯誤_回傳Result的Success應為false()
    {
        // arrange
        var shipperId = 999;

        this._stub.ShipperRepository.IsExistsAsync(Arg.Any<int>()).Returns(false);

        // act
        var actual = await this._stub.SystemUnderTest.DeleteAsync(shipperId);

        // assert
        actual.Success.Should().BeFalse();
        actual.Message.Should().Be("shipper not exists");
    }

    [Theory]
    [AutoData]
    public async Task DeleteAsync_輸入model_要刪除的資料存在_刪除完成_回傳Result的Success應為true(ShipperDto shipperDto)
    {
        // arrange
        this._stub.ShipperRepository.IsExistsAsync(Arg.Any<int>()).Returns(true);

        this._stub.ShipperRepository
            .DeleteAsync(Arg.Any<int>())
            .Returns(new Result { Success = true, AffectRows = 1 });

        // act
        var actual = await this._stub.SystemUnderTest.DeleteAsync(shipperDto.ShipperId);

        // assert
        actual.Success.Should().BeTrue();
    }
}