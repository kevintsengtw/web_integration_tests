using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using MoreLinq.Extensions;
using Sample.Domain.Entities;
using Sample.Repository.Implements;
using Sample.RepositoryTests.TestData;
using Sample.RepositoryTests.Utilities.Database;
using Sample.TestResource.AutoFixture;

namespace Sample.RepositoryTests.Implements;

[Collection(nameof(ProjectCollectionFixture))]
public sealed class ShipperRepositoryTests : IClassFixture<ShipperRepositoryClassFixture>, IDisposable
{
    private readonly ShipperRepository _systemUnderTest;

    public ShipperRepositoryTests(ShipperRepositoryClassFixture classFixture)
    {
        this._systemUnderTest = classFixture.SystemUnderTest;
    }

    public void Dispose()
    {
        TableCommands.Truncate(ProjectFixture.SampleDbConnectionString, TableNames.Shippers);
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
            () => this._systemUnderTest.IsExistsAsync(shipperId));

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
            () => this._systemUnderTest.IsExistsAsync(shipperId));

        // assert
        exception.Message.Should().Contain(nameof(shipperId));
    }

    [Fact]
    public async Task IsExistsAsync_輸入的ShipperId_資料不存在_應回傳false()
    {
        // arrange
        var shipperId = 99;

        // act
        var actual = await this._systemUnderTest.IsExistsAsync(shipperId);

        // assert
        actual.Should().BeFalse();
    }

    [Theory]
    [AutoData]
    public async Task IsExistsAsync_輸入的ShipperId_資料有存在_應回傳True(ShipperModel model)
    {
        // arrange
        var shipperId = model.ShipperId;
        ShipperRepositoryClassFixture.InsertData(model);

        // act
        var actual = await this._systemUnderTest.IsExistsAsync(shipperId);

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
            () => this._systemUnderTest.GetAsync(shipperId));

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
            () => this._systemUnderTest.GetAsync(shipperId));

        // assert
        exception.Message.Should().Contain(nameof(shipperId));
    }

    [Fact]
    public async Task GetAsync_輸入的ShipperId_資料不存在_應回傳null()
    {
        // arrange
        var shipperId = 99;

        // act
        var actual = await this._systemUnderTest.GetAsync(shipperId);

        // assert
        actual.Should().BeNull();
    }

    [Theory]
    [AutoData]
    public async Task GetAsync_輸入的ShipperId_資料有存在_應回傳model(ShipperModel model)
    {
        // arrange
        var shipperId = model.ShipperId;
        ShipperRepositoryClassFixture.InsertData(model);

        // act
        var actual = await this._systemUnderTest.GetAsync(shipperId);

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
        var actual = await this._systemUnderTest.GetTotalCountAsync();

        // assert
        actual.Should().Be(expected);
    }

    [Fact]
    public async Task GetTotalCountAsync_資料表裡有10筆資料_應回傳10()
    {
        // arrange
        var expected = 10;

        var models = RepositoryFixture.Fixture.CreateMany<ShipperModel>(10);

        foreach (var model in models)
        {
            ShipperRepositoryClassFixture.InsertData(model);
        }

        // act
        var actual = await this._systemUnderTest.GetTotalCountAsync();

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
        var actual = await this._systemUnderTest.GetAllAsync();

        // assert
        actual.Should().BeEmpty();
    }

    [Theory]
    [AutoData]
    public async Task GetAllAsync_資料表裡有10筆資料_回傳的集合裡有10筆([CollectionSize(10)] IEnumerable<ShipperModel> models)
    {
        // arrange
        models.ForEach(ShipperRepositoryClassFixture.InsertData);

        // act
        var actual = await this._systemUnderTest.GetAllAsync();

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
            () => this._systemUnderTest.GetCollectionAsync(from, size));
        
        // assert
        exception.Message.Should().Contain(parameterName);
    }
    
    [Fact]
    public async Task GetCollectionAsync_from為1_size為10_資料表裡無資料_應回傳空集合()
    {
        // arrange
        const int from = 1;
        const int size = 10;
        
        // act
        var actual = await this._systemUnderTest.GetCollectionAsync(from, size);
        
        // assert
        actual.Should().BeEmpty();
    }
    
    [Theory]
    [AutoData]
    public async Task GetCollectionAsync_from為1_size為10_資料表裡有5筆資料_回傳集合應有5筆(
        [CollectionSize(5)] IEnumerable<ShipperModel> models)
    {
        // arrange
        models.ForEach(ShipperRepositoryClassFixture.InsertData);
        
        const int from = 1;
        const int size = 10;
        
        // act
        var actual = await this._systemUnderTest.GetCollectionAsync(from, size);
        
        // assert
        actual.Should().NotBeEmpty();
        actual.Should().HaveCount(5);
    }
    
    [Theory]
    [AutoData]
    public async Task GetCollectionAsync_from為20_size為10_資料表裡只有10筆資料_from超過總數量_應回傳空集合(
        [CollectionSize(10)] IEnumerable<ShipperModel> models)
    {
        // arrange
        models.ForEach(ShipperRepositoryClassFixture.InsertData);        
        
        const int from = 20;
        const int size = 10;
        
        // act
        var actual = await this._systemUnderTest.GetCollectionAsync(from, size);
        
        // assert
        actual.Should().BeEmpty();
    }
    
    [Theory]
    [AutoData]
    public async Task GetCollectionAsync_from為6_size為10_資料表裡有10筆資料_回傳集合應有10筆(
        [CollectionSize(20)] IEnumerable<ShipperModel> models)
    {
        // arrange
        models.ForEach(ShipperRepositoryClassFixture.InsertData);
        
        const int from = 6;
        const int size = 10;
        
        // act
        var actual = await this._systemUnderTest.GetCollectionAsync(from, size);
        
        // assert
        actual.Should().NotBeEmpty();
        actual.Should().HaveCount(10);
    }
    
    [Theory]
    [AutoData]
    public async Task GetCollectionAsync_from為11_size為10_資料表裡有10筆資料_應回傳空集合(
        [CollectionSize(10)] IEnumerable<ShipperModel> models)
    {
        // arrange
        models.ForEach(ShipperRepositoryClassFixture.InsertData);
        
        const int from = 11;
        const int size = 10;
        
        // act
        var actual = await this._systemUnderTest.GetCollectionAsync(from, size);
        
        // assert
        actual.Should().BeEmpty();
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
            ()=> this._systemUnderTest.SearchAsync(companyName, phone));
        
        // assert
        exception.Message.Should().Be(exceptionMessage);
    }

    [Fact]
    public async Task SearchAsync_資料表裡無資料_應回傳空集合()
    {
        // arrange
        const string companyName = "test";
        const string phone = "02123456789";
        
        // act
        var actual = await this._systemUnderTest.SearchAsync(companyName, phone);
        
        // assert
        actual.Should().BeEmpty();
    }
    
    [Theory]
    [AutoData]
    public async Task SearchAsync_companyName輸入資料_沒有符合條件的資料_應回傳空集合([CollectionSize(10)] IEnumerable<ShipperModel> models)
    {
        // arrange
        models.ForEach(ShipperRepositoryClassFixture.InsertData);
        
        const string companyName = "test";
        const string phone = "";
        
        // act
        var actual = await this._systemUnderTest.SearchAsync(companyName, phone);
        
        // assert
        actual.Should().BeEmpty();
    }
    
    [Theory]
    [AutoData]
    public async Task SearchAsync_companyName輸入資料_phone無輸入_有符合條件的資料_回傳集合應包含符合條件的資料([CollectionSize(10)] List<ShipperModel> models)
    {
        // arrange
        var model = RepositoryFixture.Fixture.Build<ShipperModel>()
                                     .With(x => x.CompanyName, "test")
                                     .Create();

        models.Add(model);
        
        models.ForEach(ShipperRepositoryClassFixture.InsertData);
        
        const string companyName = "test";
        const string phone = "";
        
        // act
        var actual = await this._systemUnderTest.SearchAsync(companyName, phone);
        
        // assert
        actual.Should().NotBeEmpty();
        actual.Should().HaveCount(1);
        actual.Any(x => x.CompanyName == companyName).Should().BeTrue();
    }    
    
    [Theory]
    [AutoData]
    public async Task SearchAsync_companyName無輸入_phone輸入資料_有符合條件的資料_回傳集合應包含符合條件的資料([CollectionSize(10)] List<ShipperModel> models)
    {
        // arrange
        var model = RepositoryFixture.Fixture.Build<ShipperModel>()
                                     .With(x => x.Phone, "02123456789")
                                     .Create();

        models.Add(model);
        
        models.ForEach(ShipperRepositoryClassFixture.InsertData);
        
        const string companyName = "";
        const string phone = "02123456789";
        
        // act
        var actual = await this._systemUnderTest.SearchAsync(companyName, phone);
        
        // assert
        actual.Should().NotBeEmpty();
        actual.Should().HaveCount(1);
        actual.Any(x => x.Phone == phone).Should().BeTrue();
    }
    
    [Theory]
    [AutoData]
    public async Task SearchAsync_companyName輸入資料_phone輸入資料_有符合條件的資料_回傳集合應包含符合條件的資料([CollectionSize(10)] List<ShipperModel> models)
    {
        // arrange
        var model = RepositoryFixture.Fixture.Build<ShipperModel>()
                                     .With(x=>x.CompanyName, "demo")
                                     .With(x => x.Phone, "03123456789")
                                     .Create();

        models.Add(model);
        
        models.ForEach(ShipperRepositoryClassFixture.InsertData);
        
        const string companyName = "demo";
        const string phone = "03123456789";
        
        // act
        var actual = await this._systemUnderTest.SearchAsync(companyName, phone);
        
        // assert
        actual.Should().NotBeEmpty();
        actual.Should().HaveCount(1);
        actual.Any(x => x.CompanyName == companyName && x.Phone == phone).Should().BeTrue();
    }
    
    [Theory]
    [AutoData]
    public async Task SearchAsync_companyName輸入資料_phone輸入資料_沒有符合條件的資料_應回傳空集合([CollectionSize(10)] List<ShipperModel> models)
    {
        // arrange
        models.ForEach(ShipperRepositoryClassFixture.InsertData);
        
        const string companyName = "try";
        const string phone = "04123456789";
        
        // act
        var actual = await this._systemUnderTest.SearchAsync(companyName, phone);
        
        // assert
        actual.Should().BeEmpty();
    }
    
    [Fact]
    public async Task SearchAsync_companyName輸入資料_phone無輸入_有2筆符合條件的資料_回傳集合應有兩筆()
    {
        // arrange
        var model1 = RepositoryFixture.Fixture.Build<ShipperModel>()
                                      .With(x => x.CompanyName, "note")
                                      .Create();
        
        var model2 = RepositoryFixture.Fixture.Build<ShipperModel>()
                                      .With(x => x.CompanyName, "node")
                                      .Create();
        
        ShipperRepositoryClassFixture.InsertData(model1);
        ShipperRepositoryClassFixture.InsertData(model2);
        
        const string companyName = "no";
        const string phone = "";
        
        // act
        var actual = await this._systemUnderTest.SearchAsync(companyName, phone);
        
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
        ShipperModel model = null;

        // act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => this._systemUnderTest.CreateAsync(model));

        // assert
        exception.Message.Should().Contain(nameof(model));
    }

    [Theory]
    [AutoData]
    public async Task CreateAsync_輸入一個有資料的model_新增完成_回傳Result的Success應為true(ShipperModel model)
    {
        // arrange

        // act
        var actual = await this._systemUnderTest.CreateAsync(model);

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
        ShipperModel model = null;

        // act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => this._systemUnderTest.UpdateAsync(model));

        // assert
        exception.Message.Should().Contain(nameof(model));
    }
    
    [Theory]
    [AutoData]
    public async Task UpdateAsync_輸入model_要修改的資料並不存在_更新錯誤_回傳Result的Success應為false(ShipperModel model)
    {
        // arrange

        // act
        var actual = await this._systemUnderTest.UpdateAsync(model);

        // assert
        actual.Success.Should().BeFalse();
        actual.Message.Should().Be("資料更新錯誤");
    }

    [Theory]
    [AutoData]
    public async Task UpdateAsync_輸入model_要修改的資料存在_更新完成_回傳Result的Success應為true(ShipperModel model)
    {
        // arrange
        ShipperRepositoryClassFixture.InsertData(model);

        model.CompanyName = "update";

        // act
        var actual = await this._systemUnderTest.UpdateAsync(model);

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
            () => this._systemUnderTest.DeleteAsync(shipperId));

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
            () => this._systemUnderTest.DeleteAsync(shipperId));

        // assert
        exception.Message.Should().Contain(nameof(shipperId));
    }

    [Fact]
    public async Task DeleteAsync_輸入ShipperId_要刪除的資料並不存在_刪除錯誤_回傳Result的Success應為false()
    {
        // arrange
        var shipperId = 999;

        // act
        var actual = await this._systemUnderTest.DeleteAsync(shipperId);

        // assert
        actual.Success.Should().BeFalse();
        actual.Message.Should().Be("資料刪除錯誤");
    }

    [Theory]
    [AutoData]
    public async Task DeleteAsync_輸入model_要刪除的資料存在_刪除完成_回傳Result的Success應為true(ShipperModel model)
    {
        // arrange
        ShipperRepositoryClassFixture.InsertData(model);

        // act
        var actual = await this._systemUnderTest.DeleteAsync(model.ShipperId);

        // assert
        actual.Success.Should().BeTrue();
    }
}