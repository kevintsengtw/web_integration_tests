using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions.AspNetCore.Mvc;
using NSubstitute;
using Sample.Domain.Misc;
using Sample.Service.Dto;
using Sample.Service.Interface;
using Sample.TestResource.AutoFixture;
using Sample.WebApplication.Controllers;
using Sample.WebApplication.Models.InputParameters;
using Sample.WebApplication.Models.OutputModels;
using Sample.WebApplicationTests.AutoFixtureConfigurations;

// ReSharper disable PossibleMultipleEnumeration

namespace Sample.WebApplicationTests.Controllers;

public class ShipperControllerTests
{
    //---------------------------------------------------------------------------------------------
    // GetAllAsync

    [Theory]
    [AutoDataWithCustomization]
    public async Task GetAllAsync_從service取得的資料為空集合_應回傳OkObjectResult及空的資料集合(
        [Frozen] IShipperService shipperService,
        ShipperController sut)
    {
        // arrange
        shipperService.GetAllAsync().Returns([]);

        // act
        var actual = await sut.GetAllAsync();

        // assert
        actual.Should().BeOkObjectResult()
              .WithStatusCode(200)
              .WithValueMatch<IEnumerable<ShipperOutputModel>>(x => !x.Any());
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task GetAllAsync_從service可取得資料_應回傳OkObjectResult及資料集合(
        [CollectionSize(10)] IEnumerable<ShipperDto> shipperDtos,
        [Frozen] IShipperService shipperService,
        ShipperController sut)
    {
        // arrange
        shipperService.GetAllAsync().Returns(shipperDtos);

        // act
        var actual = await sut.GetAllAsync();

        // assert
        actual.Should().BeOkObjectResult()
              .WithStatusCode(200)
              .WithValueMatch<IEnumerable<ShipperOutputModel>>(
                  x => x.Any() && x.Count() == 10);
    }

    //---------------------------------------------------------------------------------------------
    // GetCollectionAsync

    [Theory]
    [AutoDataWithCustomization]
    public async Task GetCollectionAsync_資料表裡無資料_應回傳OkObjectResult及空集合(
        [Frozen] IShipperService shipperService,
        ShipperController sut)
    {
        // arrange
        var parameter = new ShipperPageParameter { From = 1, Size = 10 };

        shipperService.GetTotalCountAsync().Returns(0);

        // act
        var actual = await sut.GetCollectionAsync(parameter);

        // assert
        actual.Should().BeOkObjectResult()
              .WithStatusCode(200)
              .WithValueMatch<IEnumerable<ShipperOutputModel>>(x => !x.Any());
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task GetCollectionAsync_from輸入1_size輸入10_有符合條件的資料_應回傳OkObjectResult及集合(
        [CollectionSize(10)] IEnumerable<ShipperDto> shipperDtos,
        [Frozen] IShipperService shipperService,
        ShipperController sut)
    {
        // arrange
        var parameter = new ShipperPageParameter { From = 1, Size = 10 };

        shipperService.GetTotalCountAsync().Returns(10);

        shipperService.GetCollectionAsync(Arg.Any<int>(), Arg.Any<int>())
                      .Returns(shipperDtos);

        // act
        var actual = await sut.GetCollectionAsync(parameter);

        // assert
        actual.Should().BeOkObjectResult()
              .WithStatusCode(200)
              .WithValueMatch<IEnumerable<ShipperOutputModel>>(x => x.Count() == 10);
    }

    //---------------------------------------------------------------------------------------------
    // SearchAsync

    [Theory]
    [AutoDataWithCustomization]
    public async Task SearchAsync_CompanyName有值_Phone無值_沒有符合條件的資料_應回傳OkObjectResult及空集合(
        [Frozen] IShipperService shipperService,
        ShipperController sut)
    {
        // arrange
        var parameter = new ShipperSearchParameter { CompanyName = "test", Phone = "" };

        shipperService.SearchAsync(Arg.Any<string>(), Arg.Any<string>())
                      .Returns([]);

        // act
        var actual = await sut.SearchAsync(parameter);

        // assert
        actual.Should().BeOkObjectResult()
              .WithStatusCode(200)
              .WithValueMatch<IEnumerable<ShipperOutputModel>>(x => !x.Any());
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task SearchAsync_CompanyName無值_Phone有值_沒有符合條件的資料_應回傳OkObjectResult及空集合(
        [Frozen] IShipperService shipperService,
        ShipperController sut)
    {
        // arrange
        var parameter = new ShipperSearchParameter { CompanyName = "", Phone = "021234" };

        shipperService.SearchAsync(Arg.Any<string>(), Arg.Any<string>())
                      .Returns([]);

        // act
        var actual = await sut.SearchAsync(parameter);

        // assert
        actual.Should().BeOkObjectResult()
              .WithStatusCode(200)
              .WithValueMatch<IEnumerable<ShipperOutputModel>>(x => !x.Any());
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task SearchAsync_CompanyName有值_Phone無值_有符合條件的資料_應回傳OkObjectResult及集合(
        IFixture fixture,
        [Frozen] IShipperService shipperService,
        ShipperController sut)
    {
        // arrange
        var parameter = new ShipperSearchParameter { CompanyName = "test", Phone = "" };

        var shippers = fixture.Build<ShipperDto>()
                              .With(x => x.CompanyName, "test1")
                              .CreateMany(1);

        shipperService.SearchAsync(Arg.Any<string>(), Arg.Any<string>())
                      .Returns(shippers);

        // act
        var actual = await sut.SearchAsync(parameter);

        // assert
        actual.Should().BeOkObjectResult()
              .WithStatusCode(200)
              .WithValueMatch<IEnumerable<ShipperOutputModel>>(
                  x => x.Count() == 1
                       && x.All(y => y.CompanyName.StartsWith("test")));
    }

    //---------------------------------------------------------------------------------------------
    // GetAsync

    [Theory]
    [AutoDataWithCustomization]
    public async Task GetAsync_輸入ShipperId_資料不存在_應回傳BadRequestObjectResult及訊息(
        [Frozen] IShipperService shipperService,
        ShipperController sut,
        ShipperIdParameter parameter)
    {
        // arrange
        shipperService.IsExistsAsync(Arg.Any<int>())
                      .Returns(false);

        const string expectedMessage = "shipper not exists";

        // act
        var actual = await sut.GetAsync(parameter);

        // assert
        actual.Should().BeBadRequestObjectResult()
              .WithStatusCode(400)
              .WithValueMatch<ResponseMessageOutputModel>(x => x.Message.Equals(expectedMessage));
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task GetAsync_輸入ShipperId_資料存在並可取得資料_應回傳OkObjectResult及Shipper資料(
        [Frozen] IShipperService shipperService,
        ShipperController sut,
        ShipperIdParameter parameter,
        ShipperDto shipperDto)
    {
        // arrange
        shipperDto.ShipperId = parameter.ShipperId;

        shipperService.IsExistsAsync(Arg.Any<int>()).Returns(true);
        shipperService.GetAsync(Arg.Any<int>()).Returns(shipperDto);

        // act
        var actual = await sut.GetAsync(parameter);

        // assert
        actual.Should().BeOkObjectResult()
              .WithStatusCode(200)
              .WithValueMatch<ShipperOutputModel>(x => x.ShipperId == parameter.ShipperId);
    }

    //---------------------------------------------------------------------------------------------
    // PostAsync

    [Theory]
    [AutoDataWithCustomization]
    public async Task PostAsync_資料建立出現錯誤_應回傳BadRequestObjectResult及訊息(
        [Frozen] IShipperService shipperService,
        ShipperController sut,
        ShipperParameter parameter)
    {
        // arrange
        shipperService.CreateAsync(Arg.Any<ShipperDto>())
                      .Returns(new Result(false) { Message = "資料新增錯誤" });

        // act
        var actual = await sut.PostAsync(parameter);

        // assert
        actual.Should().BeBadRequestObjectResult()
              .WithStatusCode(400)
              .WithValueMatch<ResponseMessageOutputModel>(x => x.Message == "create failure");
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task PostAsync_資料建立完成_應回傳OkObjectResult及訊息(
        [Frozen] IShipperService shipperService,
        ShipperController sut,
        ShipperParameter parameter)
    {
        // arrange
        shipperService.CreateAsync(Arg.Any<ShipperDto>())
                      .Returns(new Result(true) { AffectRows = 1 });

        // act
        var actual = await sut.PostAsync(parameter);

        // assert
        actual.Should().BeOkObjectResult()
              .WithStatusCode(200)
              .WithValueMatch<ResponseMessageOutputModel>(x => x.Message == "create success");
    }

    //---------------------------------------------------------------------------------------------
    // PutAsync

    [Theory]
    [AutoDataWithCustomization]
    public async Task PutAsync_要修改的資料並不存在_應回傳BadRequestObjectResult及訊息(
        [Frozen] IShipperService shipperService,
        ShipperController sut,
        ShipperUpdateParameter parameter)
    {
        // arrange
        shipperService.IsExistsAsync(Arg.Any<int>()).Returns(false);

        // act
        var actual = await sut.PutAsync(parameter);

        // assert
        actual.Should().BeBadRequestObjectResult()
              .WithStatusCode(400)
              .WithValueMatch<ResponseMessageOutputModel>(x => x.Message == "shipper not exists");
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task PutAsync_要修改的資料為存在_修改資料失敗_應回傳BadRequestObjectResult及訊息(
        [Frozen] IShipperService shipperService,
        ShipperController sut,
        ShipperUpdateParameter parameter,
        ShipperDto shipperDto)
    {
        // arrange
        shipperService.IsExistsAsync(Arg.Any<int>()).Returns(true);

        shipperDto.ShipperId = parameter.ShipperId;

        shipperService.GetAsync(Arg.Any<int>()).Returns(shipperDto);

        shipperService.UpdateAsync(Arg.Any<ShipperDto>())
                      .Returns(new Result(false) { Message = "資料更新錯誤" });

        // act
        var actual = await sut.PutAsync(parameter);

        // assert
        actual.Should().BeBadRequestObjectResult()
              .WithStatusCode(400)
              .WithValueMatch<ResponseMessageOutputModel>(x => x.Message == "update failure");
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task PutAsync_要修改的資料為存在_修改資料完成_應回傳OkObjectResult及訊息(
        [Frozen] IShipperService shipperService,
        ShipperController sut,
        ShipperUpdateParameter parameter,
        ShipperDto shipperDto)
    {
        // arrange
        shipperService.IsExistsAsync(Arg.Any<int>()).Returns(true);

        shipperDto.ShipperId = parameter.ShipperId;

        shipperService.GetAsync(Arg.Any<int>()).Returns(shipperDto);

        shipperService.UpdateAsync(Arg.Any<ShipperDto>())
                      .Returns(new Result(true) { AffectRows = 1 });

        // act
        var actual = await sut.PutAsync(parameter);

        // assert
        actual.Should().BeOkObjectResult()
              .WithStatusCode(200)
              .WithValueMatch<ResponseMessageOutputModel>(x => x.Message == "update success");
    }

    //---------------------------------------------------------------------------------------------
    // DeleteAsync

    [Theory]
    [AutoDataWithCustomization]
    public async Task DeleteAsync_指定刪除的資料並不存在_應回傳BadRequestObjectResult及訊息(
        [Frozen] IShipperService shipperService,
        ShipperController sut,
        ShipperIdParameter parameter)
    {
        // arrange
        shipperService.IsExistsAsync(Arg.Any<int>()).Returns(false);

        // act
        var actual = await sut.DeleteAsync(parameter);

        // assert
        actual.Should().BeBadRequestObjectResult()
              .WithStatusCode(400)
              .WithValueMatch<ResponseMessageOutputModel>(x => x.Message == "shipper not exists");
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task DeleteAsync_指定刪除的資料存在_執行刪除失敗_應回傳BadRequestObjectResult及訊息(
        [Frozen] IShipperService shipperService,
        ShipperController sut,
        ShipperIdParameter parameter)
    {
        // arrange
        shipperService.IsExistsAsync(Arg.Any<int>()).Returns(true);

        shipperService.DeleteAsync(Arg.Any<int>())
                      .Returns(new Result(false) { Message = "資料刪除失敗" });

        // act
        var actual = await sut.DeleteAsync(parameter);

        // assert
        actual.Should().BeBadRequestObjectResult()
              .WithStatusCode(400)
              .WithValueMatch<ResponseMessageOutputModel>(x => x.Message == "delete failure");
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task DeleteAsync_指定刪除的資料存在_執行刪除完成_應回傳OkObjectResult及訊息(
        [Frozen] IShipperService shipperService,
        ShipperController sut,
        ShipperIdParameter parameter)
    {
        // arrange
        shipperService.IsExistsAsync(Arg.Any<int>()).Returns(true);

        shipperService.DeleteAsync(Arg.Any<int>())
                      .Returns(new Result(true) { AffectRows = 1 });

        // act
        var actual = await sut.DeleteAsync(parameter);

        // assert
        actual.Should().BeOkObjectResult()
              .WithStatusCode(200)
              .WithValueMatch<ResponseMessageOutputModel>(x => x.Message == "delete success");
    }
}