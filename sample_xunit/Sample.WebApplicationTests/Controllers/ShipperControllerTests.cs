using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions.AspNetCore.Mvc;
using MapsterMapper;
using NSubstitute;
using Sample.Domain.Misc;
using Sample.Service.Dto;
using Sample.Service.Interface;
using Sample.TestResource.AutoFixture;
using Sample.WebApplication.Controllers;
using Sample.WebApplication.Models.InputParameters;
using Sample.WebApplication.Models.OutputModels;

namespace Sample.WebApplicationTests.Controllers;

public class ShipperControllerTests : IClassFixture<ShipperControllerClassFixture>
{
    private readonly ShipperControllerClassFixture _classFixture;
    
    private readonly StubShipperController _stub;

    public ShipperControllerTests(ShipperControllerClassFixture classFixture)
    {
        this._classFixture = classFixture;
        this._stub = classFixture.Stub;
    }

    //---------------------------------------------------------------------------------------------
    // GetAllAsync

    [Fact]
    public async Task GetAllAsync_從service取得的資料為空集合_應回傳OkObjectResult及空的資料集合()
    {
        // arrange
        this._stub.ShipperService.GetAllAsync().Returns(Enumerable.Empty<ShipperDto>());

        // act
        var actual = await this._stub.SystemUnderTest.GetAllAsync();

        // assert
        actual.Should().BeOkObjectResult()
              .WithStatusCode(200)
              .WithValueMatch<IEnumerable<ShipperOutputModel>>(x => !x.Any());
    }

    [Theory]
    [AutoData]
    public async Task GetAllAsync_從service可取得資料_應回傳OkObjectResult及資料集合(
        [CollectionSize(10)] IEnumerable<ShipperDto> shipperDtos)
    {
        // arrange
        this._stub.ShipperService.GetAllAsync().Returns(shipperDtos);

        // act
        var actual = await this._stub.SystemUnderTest.GetAllAsync();

        // assert
        actual.Should().BeOkObjectResult()
              .WithStatusCode(200)
              .WithValueMatch<IEnumerable<ShipperOutputModel>>(
                  x => x.Any() && x.Count() == 10);
    }

    //---------------------------------------------------------------------------------------------
    // GetCollectionAsync

    [Fact]
    public async Task GetCollectionAsync_資料表裡無資料_應回傳OkObjectResult及空集合()
    {
        // arrange
        var parameter = new ShipperPageParameter { From = 1, Size = 10 };

        this._stub.ShipperService.GetTotalCountAsync().Returns(0);

        // act
        var actual = await this._stub.SystemUnderTest.GetCollectionAsync(parameter);

        // assert
        actual.Should().BeOkObjectResult()
              .WithStatusCode(200)
              .WithValueMatch<IEnumerable<ShipperOutputModel>>(x => !x.Any());
    }

    [Theory]
    [AutoData]
    public async Task GetCollectionAsync_from輸入1_size輸入10_有符合條件的資料_應回傳OkObjectResult及集合(
        [CollectionSize(10)] IEnumerable<ShipperDto> shipperDtos)
    {
        // arrange
        var parameter = new ShipperPageParameter { From = 1, Size = 10 };

        this._stub.ShipperService.GetTotalCountAsync().Returns(10);

        this._stub.ShipperService
            .GetCollectionAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(shipperDtos);

        // act
        var actual = await this._stub.SystemUnderTest.GetCollectionAsync(parameter);

        // assert
        actual.Should().BeOkObjectResult()
              .WithStatusCode(200)
              .WithValueMatch<IEnumerable<ShipperOutputModel>>(x => x.Count() == 10);
    }

    //---------------------------------------------------------------------------------------------
    // SearchAsync

    [Fact]
    public async Task SearchAsync_CompanyName有值_Phone無值_沒有符合條件的資料_應回傳OkObjectResult及空集合()
    {
        // arrange
        var parameter = new ShipperSearchParameter { CompanyName = "test", Phone = "" };

        this._stub.ShipperService
            .SearchAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Enumerable.Empty<ShipperDto>());
        
        // act
        var actual = await this._stub.SystemUnderTest.SearchAsync(parameter);
        
        // assert
        actual.Should().BeOkObjectResult()
              .WithStatusCode(200)
              .WithValueMatch<IEnumerable<ShipperOutputModel>>(x => !x.Any());
    }

    [Fact]
    public async Task SearchAsync_CompanyName無值_Phone有值_沒有符合條件的資料_應回傳OkObjectResult及空集合()
    {
        // arrange
        var parameter = new ShipperSearchParameter { CompanyName = "", Phone = "021234" };

        this._stub.ShipperService
            .SearchAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Enumerable.Empty<ShipperDto>());
        
        // act
        var actual = await this._stub.SystemUnderTest.SearchAsync(parameter);
        
        // assert
        actual.Should().BeOkObjectResult()
              .WithStatusCode(200)
              .WithValueMatch<IEnumerable<ShipperOutputModel>>(x => !x.Any());
    }    
    
    [Fact]
    public async Task SearchAsync_CompanyName有值_Phone無值_有符合條件的資料_應回傳OkObjectResult及集合()
    {
        // arrange
        var parameter = new ShipperSearchParameter { CompanyName = "test", Phone = "" };

        var shippers = this._classFixture.Fixture.Build<ShipperDto>()
                           .With(x => x.CompanyName, "test1")
                           .CreateMany(1);
        
        this._stub.ShipperService
            .SearchAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(shippers);
        
        // act
        var actual = await this._stub.SystemUnderTest.SearchAsync(parameter);
        
        // assert
        actual.Should().BeOkObjectResult()
              .WithStatusCode(200)
              .WithValueMatch<IEnumerable<ShipperOutputModel>>(
                  x => x.Count() == 1 && 
                       x.All(x => x.CompanyName.StartsWith("test")));
    }
    
    //---------------------------------------------------------------------------------------------
    // GetAsync

    [Theory]
    [AutoData]
    public async Task GetAsync_輸入ShipperId_資料不存在_應回傳BadRequestObjectResult及訊息(ShipperIdParameter parameter)
    {
        // arrange
        this._stub.ShipperService
            .IsExistsAsync(Arg.Any<int>())
            .Returns(false);

        const string expectedMessage = "shipper not exists";

        // act
        var actual = await this._stub.SystemUnderTest.GetAsync(parameter);

        // assert
        actual.Should().BeBadRequestObjectResult()
              .WithStatusCode(400)
              .WithValueMatch<ResponseMessageOutputModel>(x => x.Message.Equals(expectedMessage));
    }

    [Theory]
    [AutoData]
    public async Task GetAsync_輸入ShipperId_資料存在並可取得資料_應回傳OkObjectResult及Shipper資料(
        ShipperIdParameter parameter, ShipperDto shipperDto)
    {
        // arrange
        shipperDto.ShipperId = parameter.ShipperId;

        this._stub.ShipperService.IsExistsAsync(Arg.Any<int>()).Returns(true);
        this._stub.ShipperService.GetAsync(Arg.Any<int>()).Returns(shipperDto);

        // act
        var actual = await this._stub.SystemUnderTest.GetAsync(parameter);

        // assert
        actual.Should().BeOkObjectResult()
              .WithStatusCode(200)
              .WithValueMatch<ShipperOutputModel>(x => x.ShipperId == parameter.ShipperId);
    }

    //---------------------------------------------------------------------------------------------
    // PostAsync

    [Theory]
    [AutoData]
    public async Task PostAsync_資料建立出現錯誤_應回傳BadRequestObjectResult及訊息(ShipperParameter parameter)
    {
        // arrange
        this._stub.ShipperService
            .CreateAsync(Arg.Any<ShipperDto>())
            .Returns(new Result(false) { Message = "資料新增錯誤" });

        // act
        var actual = await this._stub.SystemUnderTest.PostAsync(parameter);

        // assert
        actual.Should().BeBadRequestObjectResult()
              .WithStatusCode(400)
              .WithValueMatch<ResponseMessageOutputModel>(x => x.Message == "create failure");
    }

    [Theory]
    [AutoData]
    public async Task PostAsync_資料建立完成_應回傳OkObjectResult及訊息(ShipperParameter parameter)
    {
        // arrange
        this._stub.ShipperService
            .CreateAsync(Arg.Any<ShipperDto>())
            .Returns(new Result(true) { AffectRows = 1 });

        // act
        var actual = await this._stub.SystemUnderTest.PostAsync(parameter);

        // assert
        actual.Should().BeOkObjectResult()
              .WithStatusCode(200)
              .WithValueMatch<ResponseMessageOutputModel>(x => x.Message == "create success");
    }

    //---------------------------------------------------------------------------------------------
    // PutAsync

    [Theory]
    [AutoData]
    public async Task PutAsync_要修改的資料並不存在_應回傳BadRequestObjectResult及訊息(ShipperUpdateParameter parameter)
    {
        // arrange
        this._stub.ShipperService.IsExistsAsync(Arg.Any<int>()).Returns(false);

        // act
        var actual = await this._stub.SystemUnderTest.PutAsync(parameter);

        // assert
        actual.Should().BeBadRequestObjectResult()
              .WithStatusCode(400)
              .WithValueMatch<ResponseMessageOutputModel>(x => x.Message == "shipper not exists");
    }

    [Theory]
    [AutoData]
    public async Task PutAsync_要修改的資料為存在_修改資料失敗_應回傳BadRequestObjectResult及訊息(
        ShipperUpdateParameter parameter, ShipperDto shipperDto)
    {
        // arrange
        this._stub.ShipperService.IsExistsAsync(Arg.Any<int>()).Returns(true);

        shipperDto.ShipperId = parameter.ShipperId;

        this._stub.ShipperService.GetAsync(Arg.Any<int>()).Returns(shipperDto);

        this._stub.ShipperService
            .UpdateAsync(Arg.Any<ShipperDto>())
            .Returns(new Result(false) { Message = "資料更新錯誤" });

        // act
        var actual = await this._stub.SystemUnderTest.PutAsync(parameter);

        // assert
        actual.Should().BeBadRequestObjectResult()
              .WithStatusCode(400)
              .WithValueMatch<ResponseMessageOutputModel>(x => x.Message == "update failure");
    }

    [Theory]
    [AutoData]
    public async Task PutAsync_要修改的資料為存在_修改資料完成_應回傳OkObjectResult及訊息(
        ShipperUpdateParameter parameter, ShipperDto shipperDto)
    {
        // arrange
        this._stub.ShipperService.IsExistsAsync(Arg.Any<int>()).Returns(true);

        shipperDto.ShipperId = parameter.ShipperId;

        this._stub.ShipperService.GetAsync(Arg.Any<int>()).Returns(shipperDto);

        this._stub.ShipperService
            .UpdateAsync(Arg.Any<ShipperDto>())
            .Returns(new Result(true) { AffectRows = 1 });

        // act
        var actual = await this._stub.SystemUnderTest.PutAsync(parameter);

        // assert
        actual.Should().BeOkObjectResult()
              .WithStatusCode(200)
              .WithValueMatch<ResponseMessageOutputModel>(x => x.Message == "update success");
    }

    //---------------------------------------------------------------------------------------------
    // DeleteAsync

    [Theory]
    [AutoData]
    public async Task DeleteAsync_指定刪除的資料並不存在_應回傳BadRequestObjectResult及訊息(ShipperIdParameter parameter)
    {
        // arrange
        this._stub.ShipperService.IsExistsAsync(Arg.Any<int>()).Returns(false);

        // act
        var actual = await this._stub.SystemUnderTest.DeleteAsync(parameter);

        // assert
        actual.Should().BeBadRequestObjectResult()
              .WithStatusCode(400)
              .WithValueMatch<ResponseMessageOutputModel>(x => x.Message == "shipper not exists");
    }

    [Theory]
    [AutoData]
    public async Task DeleteAsync_指定刪除的資料存在_執行刪除失敗_應回傳BadRequestObjectResult及訊息(ShipperIdParameter parameter)
    {
        // arrange
        this._stub.ShipperService.IsExistsAsync(Arg.Any<int>()).Returns(true);

        this._stub.ShipperService
            .DeleteAsync(Arg.Any<int>())
            .Returns(new Result(false) { Message = "資料刪除失敗" });

        // act
        var actual = await this._stub.SystemUnderTest.DeleteAsync(parameter);

        // assert
        actual.Should().BeBadRequestObjectResult()
              .WithStatusCode(400)
              .WithValueMatch<ResponseMessageOutputModel>(x => x.Message == "delete failure");
    }

    [Theory]
    [AutoData]
    public async Task DeleteAsync_指定刪除的資料存在_執行刪除完成_應回傳OkObjectResult及訊息(ShipperIdParameter parameter)
    {
        // arrange
        this._stub.ShipperService.IsExistsAsync(Arg.Any<int>()).Returns(true);

        this._stub.ShipperService
            .DeleteAsync(Arg.Any<int>())
            .Returns(new Result(true) { AffectRows = 1 });

        // act
        var actual = await this._stub.SystemUnderTest.DeleteAsync(parameter);

        // assert
        actual.Should().BeOkObjectResult()
              .WithStatusCode(200)
              .WithValueMatch<ResponseMessageOutputModel>(x => x.Message == "delete success");
    }
}

public class ShipperControllerClassFixture : WebApplicationFixture
{
    public StubShipperController Stub => this.Fixture.Create<StubShipperController>();
}

public class StubShipperController
{
    public IMapper Mapper { get; set; }

    public IShipperService ShipperService { get; set; }

    public ShipperController SystemUnderTest => new(this.Mapper, this.ShipperService);
}