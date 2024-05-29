using System.Text.Json;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Flurl;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MoreLinq;
using NSubstitute;
using Sample.Domain.Misc;
using Sample.Service.Dto;
using Sample.Service.Interface;
using Sample.TestResource.AutoFixture;
using Sample.WebApplication.Infrastructure.Wrapper.Models;
using Sample.WebApplication.Models.InputParameters;
using Sample.WebApplication.Models.OutputModels;
using Sample.WebApplicationIntegrationTests.DatabaseUtilities;
using Sample.WebApplicationIntegrationTests.Utilities;
using Xunit.Abstractions;

namespace Sample.WebApplicationIntegrationTests.Tests;

/// <summary>
/// Class ShipperApiTests
/// </summary>
[Collection(nameof(ProjectCollectionFixture))]
public sealed class ShipperApiTests : IClassFixture<ApiTestClassFixture>, IDisposable
{
    private const string BaseRequestUrl = "api/shipper";

    private readonly ApiTestClassFixture _classFixture;

    private readonly ITestOutputHelper _testOutputHelper;

    private HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShipperApiTests"/> class
    /// </summary>
    /// <param name="testOutputHelper">testOutputHelper</param>
    /// <param name="classFixture">classFixture</param>
    public ShipperApiTests(ITestOutputHelper testOutputHelper, ApiTestClassFixture classFixture)
    {
        _testOutputHelper = testOutputHelper;
        this._classFixture = classFixture;
        this._httpClient = classFixture.TestWebApplicationFactory.CreateDefaultClient(new HttpClientLogger(testOutputHelper));
    }

    ~ShipperApiTests()
    {
        this.Dispose(false);
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        TableCommands.Truncate(ProjectFixture.SampleDbConnectionString, "Shippers");
        this._httpClient?.Dispose();
    }

    //---------------------------------------------------------------------------------------------
    // Get (api/shipper.GET)

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper.GET")]
    public async Task Get_RequestUri未帶入ShipperId_輸入驗證錯誤_應回傳BadRequestResult_StatusCode為400()
    {
        // arrange

        // act
        var response = await this._httpClient.GetAsync($"{BaseRequestUrl}");

        // assert
        response.Should().Be400BadRequest()
                .And
                .Satisfy<FailureResultOutputModel>(model =>
                {
                    model.Status.Should().Be("ValidationError");
                    model.Method.Should().Be("/api/shipper.GET");

                    var errors = JsonSerializer.Deserialize<List<FailureInformation>>(JsonSerializer.Serialize(model.Errors));
                    errors[0].Message.Should().Be("ShipperId error");
                    errors[0].Description.Should().Be("ShipperId 必須大於 0");
                });
    }

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper.GET")]
    public async Task Get_ShipperId輸入0_不符合規格_ValidationError_應回傳BadRequestResult_StatusCode為400()
    {
        // arrange
        var parameter = new ShipperIdParameter { ShipperId = 0 };

        var requestUri = BaseRequestUrl.SetQueryParams(parameter).ToUri();

        // act
        var response = await this._httpClient.GetAsync(requestUri);

        // assert
        response.Should().Be400BadRequest()
                .And
                .Satisfy<FailureResultOutputModel>(model =>
                {
                    model.Status.Should().Be("ValidationError");
                    model.Method.Should().Be("/api/shipper.GET");

                    var errors = JsonSerializer.Deserialize<List<FailureInformation>>(JsonSerializer.Serialize(model.Errors));
                    errors[0].Message.Should().Be("ShipperId error");
                    errors[0].Description.Should().Be("ShipperId 必須大於 0");
                });
    }

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper.GET")]
    public async Task Get_ShipperId輸入負1_不符合規格_ValidationError_應回傳BadRequestResult_StatusCode為400()
    {
        // arrange
        var parameter = new ShipperIdParameter { ShipperId = -1 };

        var requestUri = BaseRequestUrl.SetQueryParams(parameter).ToUri();

        // act
        var response = await this._httpClient.GetAsync(requestUri);

        // assert
        response.Should().Be400BadRequest()
                .And
                .Satisfy<FailureResultOutputModel>(model =>
                {
                    model.Status.Should().Be("ValidationError");
                    model.Method.Should().Be("/api/shipper.GET");

                    var errors = JsonSerializer.Deserialize<List<FailureInformation>>(JsonSerializer.Serialize(model.Errors));
                    errors[0].Message.Should().Be("ShipperId error");
                    errors[0].Description.Should().Be("ShipperId 必須大於 0");
                });
    }

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper.GET")]
    public async Task Get_輸入正確格式的ShipperID_資料不存在_StatusCode應為400_回傳FailureResultOutputModel並包含預期訊息()
    {
        // arrange
        var parameter = new ShipperIdParameter { ShipperId = 999 };

        const string expectedMessage = "shipper not exists";

        var requestUri = BaseRequestUrl.SetQueryParams(parameter).ToUri();

        // act
        var response = await this._httpClient.GetAsync(requestUri);

        // assert
        response.Should().Be400BadRequest()
                .And
                .Satisfy<FailureResultOutputModel<ResponseMessageOutputModel>>(model =>
                {
                    model.Status.Should().Be("Error");
                    model.Method.Should().Be("/api/shipper.GET");

                    var responseMessage = JsonSerializer.Deserialize<ResponseMessageOutputModel>(JsonSerializer.Serialize(model.Errors));
                    responseMessage.Message.Should().Be(expectedMessage);
                });
    }

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper.GET")]
    public async Task Get_輸入正確格式的ShipperID_資料存在_StatusCode應為200_並回傳ShipperOutputModel()
    {
        // arrange
        var data = new { CompanyName = "test", Phone = "1234" };
        InsertShipperData(data);

        var parameter = new ShipperIdParameter { ShipperId = 1 };

        var requestUri = BaseRequestUrl.SetQueryParams(parameter).ToUri();

        var expectedModel = new ShipperOutputModel
        {
            ShipperId = 1, CompanyName = "test", Phone = "1234"
        };

        // act
        var response = await this._httpClient.GetAsync(requestUri);

        // assert
        response.Should().Be200Ok()
                .And
                .Satisfy<SuccessResultOutputModel<ShipperOutputModel>>(model =>
                {
                    model.Status.Should().Be("Success");
                    model.Method.Should().Be("/api/shipper.GET");
                    model.Data.Should().BeEquivalentTo(expectedModel);
                });
    }

    //---------------------------------------------------------------------------------------------
    // api/shipper.POST

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper.POST")]
    public async Task Post_Request的Content未帶入ShipperParameter_應回傳UnsupportedMediaType_StatusCode為415()
    {
        // arrange

        // act
        var response = await this._httpClient.PostAsync($"{BaseRequestUrl}", null);

        // assert
        response.Should().Be415UnsupportedMediaType();
    }

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper.POST")]
    public async Task Post_parameter的CompanyName為null_資料驗證失敗_StatusCode應回傳400以及驗證錯誤訊息()
    {
        // arrange
        var parameter = new ShipperParameter { CompanyName = null, Phone = "12345678" };

        // act
        var response = await this._httpClient.PostAsJsonAsync($"{BaseRequestUrl}", parameter);

        // assert
        response.Should().Be400BadRequest()
                .And
                .Satisfy<FailureResultOutputModel>(model =>
                {
                    model.Status.Should().Be("ValidationError");
                    model.Method.Should().Be("/api/shipper.POST");

                    var errors = JsonSerializer.Deserialize<List<FailureInformation>>(JsonSerializer.Serialize(model.Errors));
                    errors[0].Message.Should().Be("CompanyName error");
                    errors[0].Description.Should().Be("CompanyName 不可為 null");
                });
    }

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper.POST")]
    public async Task Post_parameter的CompanyName為空白字串_資料驗證失敗_StatusCode應回傳400以及驗證錯誤訊息()
    {
        // arrange
        var parameter = new ShipperParameter { CompanyName = "", Phone = "12345678" };

        // act
        var response = await this._httpClient.PostAsJsonAsync($"{BaseRequestUrl}", parameter);

        // assert
        response.Should().Be400BadRequest()
                .And
                .Satisfy<FailureResultOutputModel>(model =>
                {
                    model.Status.Should().Be("ValidationError");
                    model.Method.Should().Be("/api/shipper.POST");

                    var errors = JsonSerializer.Deserialize<List<FailureInformation>>(JsonSerializer.Serialize(model.Errors));
                    errors[0].Message.Should().Be("CompanyName error");
                    errors[0].Description.Should().Be("未輸入 CompanyName");
                });
    }

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper.POST")]
    public async Task Post_parameter的CompanyName字串長度超過40_資料驗證失敗_StatusCode應回傳400以及驗證錯誤訊息()
    {
        // arrange
        var fixture = new Fixture();
        var companyName = string.Join("", fixture.CreateMany<IEnumerable<string>>(2));
        var parameter = new ShipperParameter { CompanyName = companyName, Phone = "12345678" };

        // act
        var response = await this._httpClient.PostAsJsonAsync($"{BaseRequestUrl}", parameter);

        // assert
        response.Should().Be400BadRequest()
                .And
                .Satisfy<FailureResultOutputModel>(model =>
                {
                    model.Status.Should().Be("ValidationError");
                    model.Method.Should().Be("/api/shipper.POST");

                    var errors = JsonSerializer.Deserialize<List<FailureInformation>>(JsonSerializer.Serialize(model.Errors));
                    errors[0].Message.Should().Be("CompanyName error");
                    errors[0].Description.Should().Be("CompanyName 長度不可超過 40");
                });
    }

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper.POST")]
    public async Task Post_parameter的Phone為null_資料驗證失敗_StatusCode應回傳400以及驗證錯誤訊息()
    {
        // arrange
        var parameter = new ShipperParameter { CompanyName = "test", Phone = null };

        // act
        var response = await this._httpClient.PostAsJsonAsync($"{BaseRequestUrl}", parameter);

        // assert
        response.Should().Be400BadRequest()
                .And
                .Satisfy<FailureResultOutputModel>(model =>
                {
                    model.Status.Should().Be("ValidationError");
                    model.Method.Should().Be("/api/shipper.POST");

                    var errors = JsonSerializer.Deserialize<List<FailureInformation>>(JsonSerializer.Serialize(model.Errors));
                    errors[0].Message.Should().Be("Phone error");
                    errors[0].Description.Should().Be("Phone 不可為 null");
                });
    }

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper.POST")]
    public async Task Post_parameter的Phone為空白字串_資料驗證失敗_StatusCode應回傳400以及驗證錯誤訊息()
    {
        // arrange
        var parameter = new ShipperParameter { CompanyName = "test", Phone = "" };

        // act
        var response = await this._httpClient.PostAsJsonAsync($"{BaseRequestUrl}", parameter);

        // assert
        response.Should().Be400BadRequest()
                .And
                .Satisfy<FailureResultOutputModel>(model =>
                {
                    model.Status.Should().Be("ValidationError");
                    model.Method.Should().Be("/api/shipper.POST");

                    var errors = JsonSerializer.Deserialize<List<FailureInformation>>(JsonSerializer.Serialize(model.Errors));
                    errors[0].Message.Should().Be("Phone error");
                    errors[0].Description.Should().Be("未輸入 Phone");
                });
    }

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper.POST")]
    public async Task Post_parameter的Phone字串長度超過24_資料驗證失敗_StatusCode應回傳400以及驗證錯誤訊息()
    {
        // arrange
        var fixture = new Fixture();
        var parameter = new ShipperParameter { CompanyName = "test", Phone = fixture.Create<string>() };

        // act
        var response = await this._httpClient.PostAsJsonAsync($"{BaseRequestUrl}", parameter);

        // assert
        response.Should().Be400BadRequest()
                .And
                .Satisfy<FailureResultOutputModel>(model =>
                {
                    model.Status.Should().Be("ValidationError");
                    model.Method.Should().Be("/api/shipper.POST");

                    var errors = JsonSerializer.Deserialize<List<FailureInformation>>(JsonSerializer.Serialize(model.Errors));
                    errors[0].Message.Should().Be("Phone error");
                    errors[0].Description.Should().Be("Phone 長度不可超過 24");
                });
    }

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper.POST")]
    public async Task Post_ShipperParameter的屬性輸入正確的資料_資料建立成功_StatusCode應回傳200以及成功訊息()
    {
        // arrange
        var parameter = new ShipperParameter { CompanyName = "test", Phone = "12345678" };

        // act
        var response = await this._httpClient.PostAsJsonAsync($"{BaseRequestUrl}", parameter);

        // assert
        response.Should().Be200Ok()
                .And
                .Satisfy<SuccessResultOutputModel<ResponseMessageOutputModel>>(model =>
                {
                    model.Status.Should().Be("Success");
                    model.Method.Should().Be("/api/shipper.POST");
                    model.Data.Message.Should().Be("create success");
                });
    }

    //---------------------------------------------------------------------------------------------
    // Put (api/shipper.PUT)

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper.PUT")]
    public async Task Put_沒有帶入Parameter_應回傳UnsupportedMediaType_StatusCode為415()
    {
        // arrange

        // act
        var response = await this._httpClient.PutAsync($"{BaseRequestUrl}", null);

        // assert
        response.Should().Be415UnsupportedMediaType();
    }

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper.PUT")]
    public async Task Put_parameter沒有帶入ShipperId_應回傳BadRequestResult()
    {
        // arrange
        var parameter = new ShipperUpdateParameter { CompanyName = "test", Phone = "12345678" };

        // act
        var response = await this._httpClient.PutAsJsonAsync($"{BaseRequestUrl}", parameter);

        // assert
        response.Should().Be400BadRequest()
                .And
                .Satisfy<FailureResultOutputModel>(model =>
                {
                    model.Status.Should().Be("ValidationError");
                    model.Method.Should().Be("/api/shipper.PUT");

                    var errors = JsonSerializer.Deserialize<List<FailureInformation>>(JsonSerializer.Serialize(model.Errors));
                    errors[0].Message.Should().Be("ShipperId error");
                    errors[0].Description.Should().Be("ShipperId 必須大於 0");
                });
    }

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper.PUT")]
    public async Task Put_parameter沒有帶入CompanyName_應回傳BadRequestResult()
    {
        // arrange
        var parameter = new ShipperUpdateParameter { ShipperId = 1, Phone = "12345678" };

        // act
        var response = await this._httpClient.PutAsJsonAsync($"{BaseRequestUrl}", parameter);

        // assert
        response.Should().Be400BadRequest()
                .And
                .Satisfy<FailureResultOutputModel>(model =>
                {
                    model.Status.Should().Be("ValidationError");
                    model.Method.Should().Be("/api/shipper.PUT");

                    var errors = JsonSerializer.Deserialize<List<FailureInformation>>(JsonSerializer.Serialize(model.Errors));
                    errors[0].Message.Should().Be("CompanyName error");
                    errors[0].Description.Should().Be("CompanyName 不可為 null");
                });
    }

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper.PUT")]
    public async Task Put_parameter的CompanyName為null_應回傳BadRequestResult()
    {
        // arrange
        var parameter = new ShipperUpdateParameter { ShipperId = 1, CompanyName = null, Phone = "12345678" };

        // act
        var response = await this._httpClient.PutAsJsonAsync($"{BaseRequestUrl}", parameter);

        // assert
        response.Should().Be400BadRequest()
                .And
                .Satisfy<FailureResultOutputModel>(model =>
                {
                    model.Status.Should().Be("ValidationError");
                    model.Method.Should().Be("/api/shipper.PUT");

                    var errors = JsonSerializer.Deserialize<List<FailureInformation>>(JsonSerializer.Serialize(model.Errors));
                    errors[0].Message.Should().Be("CompanyName error");
                    errors[0].Description.Should().Be("CompanyName 不可為 null");
                });
    }

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper.PUT")]
    public async Task Put_parameter的CompanyName為空白字串_資料驗證失敗_StatusCode應回傳400以及驗證錯誤訊息()
    {
        // arrange
        var parameter = new ShipperUpdateParameter { ShipperId = 1, CompanyName = "", Phone = "12345678" };

        // act
        var response = await this._httpClient.PutAsJsonAsync($"{BaseRequestUrl}", parameter);

        // assert
        response.Should().Be400BadRequest()
                .And
                .Satisfy<FailureResultOutputModel>(model =>
                {
                    model.Status.Should().Be("ValidationError");
                    model.Method.Should().Be("/api/shipper.PUT");

                    var errors = JsonSerializer.Deserialize<List<FailureInformation>>(JsonSerializer.Serialize(model.Errors));
                    errors[0].Message.Should().Be("CompanyName error");
                    errors[0].Description.Should().Be("未輸入 CompanyName");
                });
    }

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper.PUT")]
    public async Task Put_parameter的CompanyName字串長度超過40_資料驗證失敗_StatusCode應回傳400以及驗證錯誤訊息()
    {
        // arrange
        var fixture = new Fixture();
        var companyName = string.Join("", fixture.CreateMany<IEnumerable<string>>(2));
        var parameter = new ShipperUpdateParameter { ShipperId = 1, CompanyName = companyName, Phone = "12345678" };

        // act
        var response = await this._httpClient.PutAsJsonAsync($"{BaseRequestUrl}", parameter);

        // assert
        response.Should().Be400BadRequest()
                .And
                .Satisfy<FailureResultOutputModel>(model =>
                {
                    model.Status.Should().Be("ValidationError");
                    model.Method.Should().Be("/api/shipper.PUT");

                    var errors = JsonSerializer.Deserialize<List<FailureInformation>>(JsonSerializer.Serialize(model.Errors));
                    errors[0].Message.Should().Be("CompanyName error");
                    errors[0].Description.Should().Be("CompanyName 長度不可超過 40");
                });
    }

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper.PUT")]
    public async Task Put_parameter的Phone為null_資料驗證失敗_StatusCode應回傳400以及驗證錯誤訊息()
    {
        // arrange
        var parameter = new ShipperUpdateParameter { ShipperId = 1, CompanyName = "test", Phone = null };

        // act
        var response = await this._httpClient.PutAsJsonAsync($"{BaseRequestUrl}", parameter);

        // assert
        response.Should().Be400BadRequest()
                .And
                .Satisfy<FailureResultOutputModel>(model =>
                {
                    model.Status.Should().Be("ValidationError");
                    model.Method.Should().Be("/api/shipper.PUT");

                    var errors = JsonSerializer.Deserialize<List<FailureInformation>>(JsonSerializer.Serialize(model.Errors));
                    errors[0].Message.Should().Be("Phone error");
                    errors[0].Description.Should().Be("Phone 不可為 null");
                });
    }

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper.PUT")]
    public async Task Put_parameter的Phone為空白字串_資料驗證失敗_StatusCode應回傳400以及驗證錯誤訊息()
    {
        // arrange
        var parameter = new ShipperUpdateParameter { ShipperId = 1, CompanyName = "test", Phone = "" };

        // act
        var response = await this._httpClient.PutAsJsonAsync($"{BaseRequestUrl}", parameter);

        // assert
        response.Should().Be400BadRequest()
                .And
                .Satisfy<FailureResultOutputModel>(model =>
                {
                    model.Status.Should().Be("ValidationError");
                    model.Method.Should().Be("/api/shipper.PUT");

                    var errors = JsonSerializer.Deserialize<List<FailureInformation>>(JsonSerializer.Serialize(model.Errors));
                    errors[0].Message.Should().Be("Phone error");
                    errors[0].Description.Should().Be("未輸入 Phone");
                });
    }

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper.PUT")]
    public async Task Put_parameter的Phone字串長度超過24_資料驗證失敗_StatusCode應回傳400以及驗證錯誤訊息()
    {
        // arrange
        var fixture = new Fixture();
        var parameter = new ShipperUpdateParameter { ShipperId = 1, CompanyName = "test", Phone = fixture.Create<string>() };

        // act
        var response = await this._httpClient.PutAsJsonAsync($"{BaseRequestUrl}", parameter);

        // assert
        response.Should().Be400BadRequest()
                .And
                .Satisfy<FailureResultOutputModel>(model =>
                {
                    model.Status.Should().Be("ValidationError");
                    model.Method.Should().Be("/api/shipper.PUT");

                    var errors = JsonSerializer.Deserialize<List<FailureInformation>>(JsonSerializer.Serialize(model.Errors));
                    errors[0].Message.Should().Be("Phone error");
                    errors[0].Description.Should().Be("Phone 長度不可超過 24");
                });
    }

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper.PUT")]
    public async Task Put_ShipperUpdateParameter的屬性輸入正確的資料_資料更新成功_StatusCode應回傳200以及成功訊息()
    {
        // arrange

        //-- 先建立測試資料
        await this._httpClient.PostAsJsonAsync(
            $"{BaseRequestUrl}",
            new ShipperParameter { CompanyName = "demo", Phone = "23456789" });

        var parameter = new ShipperUpdateParameter
        {
            ShipperId = 1, CompanyName = "test", Phone = "12345678"
        };

        // act
        var response = await this._httpClient.PutAsJsonAsync($"{BaseRequestUrl}", parameter);

        // assert
        response.Should().Be200Ok()
                .And
                .Satisfy<SuccessResultOutputModel<ResponseMessageOutputModel>>(model =>
                {
                    model.Status.Should().Be("Success");
                    model.Method.Should().Be("/api/shipper.PUT");
                    model.Data.Message.Should().Be("update success");
                });
    }

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper.PUT")]
    public async Task Put_ShipperUpdateParameter的屬性輸入正確的資料_資料不存在_StatusCode應回傳400以及失敗訊息()
    {
        // arrange
        var parameter = new ShipperUpdateParameter { ShipperId = 1, CompanyName = "test", Phone = "12345678" };

        var expectedMessage = "shipper not exists";

        // act
        var response = await this._httpClient.PutAsJsonAsync($"{BaseRequestUrl}", parameter);

        // assert
        response.Should().Be400BadRequest()
                .And
                .Satisfy<FailureResultOutputModel<ResponseMessageOutputModel>>(model =>
                {
                    model.Status.Should().Be("Error");
                    model.Method.Should().Be("/api/shipper.PUT");
                    model.Errors.Message.Should().Be(expectedMessage);
                });
    }

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper.PUT")]
    public async Task Put_ShipperParameter的屬性輸入正確的資料_資料更新失敗_StatusCode應回傳400以及失敗訊息()
    {
        // arrange
        var shipperService = Substitute.For<IShipperService>();

        shipperService.IsExistsAsync(Arg.Any<int>()).Returns(true);
        shipperService.GetAsync(Arg.Any<int>()).Returns(new ShipperDto
        {
            ShipperId = 1,
            CompanyName = "test",
            Phone = "12345678"
        });
        shipperService.UpdateAsync(Arg.Any<ShipperDto>()).Returns(new Result(false));

        var webApplicationFactory = this._classFixture.TestWebApplicationFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IShipperService>();
                services.TryAddScoped(_ => shipperService);
            });
        });

        this._httpClient = webApplicationFactory.CreateDefaultClient(new HttpClientLogger(this._testOutputHelper));

        var parameter = new ShipperUpdateParameter { ShipperId = 1, CompanyName = "test", Phone = "12345678" };

        var expectedMessage = "update failure";

        // act
        var response = await this._httpClient.PutAsJsonAsync($"{BaseRequestUrl}", parameter);

        // assert
        response.Should().Be400BadRequest()
                .And
                .Satisfy<FailureResultOutputModel<ResponseMessageOutputModel>>(model =>
                {
                    model.Status.Should().Be("Error");
                    model.Method.Should().Be("/api/shipper.PUT");
                    model.Errors.Message.Should().Be(expectedMessage);
                });
    }

    //---------------------------------------------------------------------------------------------
    // Delete (api/shipper.DELETE)

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper.DELETE")]
    public async Task Delete_RequestUri未帶入ShipperId_應回傳UnsupportedMediaType_StatusCode為415()
    {
        // arrange

        // act
        var response = await this._httpClient.DeleteAsync($"{BaseRequestUrl}");

        // assert
        response.Should().Be415UnsupportedMediaType();
    }

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper.DELETE")]
    public async Task Delete_ShipperId輸入0_不符合規格_ValidationError_應回傳BadRequestResult_StatusCode為400()
    {
        // arrange
        var parameter = new ShipperIdParameter { ShipperId = 0 };

        // act
        var response = await this._httpClient.DeleteAsJsonAsync($"{BaseRequestUrl}", parameter);

        // assert
        response.Should().Be400BadRequest()
                .And
                .Satisfy<FailureResultOutputModel>(model =>
                {
                    model.Status.Should().Be("ValidationError");
                    model.Method.Should().Be("/api/shipper.DELETE");

                    var errors = JsonSerializer.Deserialize<List<FailureInformation>>(JsonSerializer.Serialize(model.Errors));
                    errors[0].Message.Should().Be("ShipperId error");
                    errors[0].Description.Should().Be("ShipperId 必須大於 0");
                });
    }

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper.DELETE")]
    public async Task Delete_ShipperId輸入負1_不符合規格_ValidationError_應回傳BadRequestResult_StatusCode為400()
    {
        // arrange
        var parameter = new ShipperIdParameter { ShipperId = -1 };

        // act
        var response = await this._httpClient.DeleteAsJsonAsync($"{BaseRequestUrl}", parameter);

        // assert
        response.Should().Be400BadRequest()
                .And
                .Satisfy<FailureResultOutputModel>(model =>
                {
                    model.Status.Should().Be("ValidationError");
                    model.Method.Should().Be("/api/shipper.DELETE");

                    var errors = JsonSerializer.Deserialize<List<FailureInformation>>(JsonSerializer.Serialize(model.Errors));
                    errors[0].Message.Should().Be("ShipperId error");
                    errors[0].Description.Should().Be("ShipperId 必須大於 0");
                });
    }

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper.DELETE")]
    public async Task Delete_輸入正確的parameter_資料不存在_應回傳BadRequestResult_StatusCode為400()
    {
        // arrange
        var shipperService = Substitute.For<IShipperService>();

        var webApplicationFactory = this._classFixture.TestWebApplicationFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IShipperService>();
                services.TryAddScoped(_ => shipperService);
            });
        });

        this._httpClient = webApplicationFactory.CreateDefaultClient(new HttpClientLogger(this._testOutputHelper));

        shipperService.IsExistsAsync(Arg.Any<int>()).Returns(false);

        var parameter = new ShipperIdParameter { ShipperId = 1 };

        var expectedMessage = "shipper not exists";

        // act
        var response = await this._httpClient.DeleteAsJsonAsync($"{BaseRequestUrl}", parameter);

        // assert
        // assert
        response.Should().Be400BadRequest()
                .And
                .Satisfy<FailureResultOutputModel<ResponseMessageOutputModel>>(model =>
                {
                    model.Status.Should().Be("Error");
                    model.Method.Should().Be("/api/shipper.DELETE");
                    model.Errors.Message.Should().Be(expectedMessage);
                });
    }

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper.DELETE")]
    public async Task Delete_輸入正確的parameter_資料存在_資料更新成功_StatusCode應回傳200以及成功訊息()
    {
        // arrange

        //-- 建立測試資料
        await this._httpClient.PostAsJsonAsync(
            $"{BaseRequestUrl}",
            new ShipperParameter { CompanyName = "demo", Phone = "12345467" });

        var parameter = new ShipperIdParameter { ShipperId = 1 };

        var expectedMessage = "delete success";

        // act
        var response = await this._httpClient.DeleteAsJsonAsync($"{BaseRequestUrl}", parameter);

        // assert
        response.Should().Be200Ok()
                .And
                .Satisfy<SuccessResultOutputModel<ResponseMessageOutputModel>>(model =>
                {
                    model.Status.Should().Be("Success");
                    model.Method.Should().Be("/api/shipper.DELETE");
                    model.Data.Message.Should().Be(expectedMessage);
                });
    }

    //---------------------------------------------------------------------------------------------
    // GetAllAsync

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper/all.GET")]
    public async Task GetAllAsync_資料庫裡無資料_應回傳空集合()
    {
        // arrange

        // act
        var response = await this._httpClient.GetAsync($"{BaseRequestUrl}/all");

        // assert
        response.Should().Be200Ok()
                .And
                .Satisfy<SuccessResultOutputModel<IEnumerable<ShipperOutputModel>>>(model =>
                {
                    model.Status.Should().Be("Success");
                    model.Method.Should().Be("/api/shipper/all.GET");
                    model.Data.Should().BeEmpty();
                });
    }

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper/all.GET")]
    public async Task GetAllAsync_資料表裡有一筆資料_回傳集合裡應有一筆()
    {
        // arrange
        var data = new { CompanyName = "test", Phone = "1234" };
        InsertShipperData(data);

        // act
        var response = await this._httpClient.GetAsync($"{BaseRequestUrl}/all");

        // assert
        response.Should().Be200Ok()
                .And
                .Satisfy<SuccessResultOutputModel<IEnumerable<ShipperOutputModel>>>(model =>
                {
                    model.Status.Should().Be("Success");
                    model.Method.Should().Be("/api/shipper/all.GET");
                    model.Data.Should().NotBeEmpty();
                    model.Data.Should().HaveCount(1);
                });
    }

    [Theory]
    [AutoData]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper/all.GET")]
    public async Task GetAllAsync_資料表裡有10筆資料_回傳集合裡應有10筆([CollectionSize(10)] IEnumerable<ShipperParameter> parameters)
    {
        // arrange
        parameters.ForEach(x => InsertShipperData(new { x.CompanyName, x.Phone }));

        // act
        var response = await this._httpClient.GetAsync($"{BaseRequestUrl}/all");

        // assert
        response.Should().Be200Ok()
                .And
                .Satisfy<SuccessResultOutputModel<IEnumerable<ShipperOutputModel>>>(model =>
                {
                    model.Status.Should().Be("Success");
                    model.Method.Should().Be("/api/shipper/all.GET");
                    model.Data.Should().NotBeEmpty();
                    model.Data.Should().HaveCount(10);
                });
    }

    //---------------------------------------------------------------------------------------------
    // GetCollectionAsync

    [Fact]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper/from/a/size/b.GET")]
    public async Task GetCollectionAsync_from為1_size為10_資料庫無資料_回傳應為空集合()
    {
        // arrange

        // act
        var response = await this._httpClient.GetAsync($"{BaseRequestUrl}/from/1/size/10");

        // assert
        response.Should().Be200Ok()
                .And
                .Satisfy<SuccessResultOutputModel<IEnumerable<ShipperOutputModel>>>(model =>
                {
                    model.Status.Should().Be("Success");
                    model.Method.Should().Be("/api/shipper/from/1/size/10.GET");
                    model.Data.Should().BeEmpty();
                });
    }

    [Theory]
    [AutoData]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper/from/a/size/b.GET")]
    public async Task GetCollectionAsync_from為1_size為5_資料庫有10筆資料_回傳的集合應有五筆(
        [CollectionSize(10)] IEnumerable<ShipperParameter> parameters)
    {
        // arrange
        parameters.ForEach(x => InsertShipperData(new { x.CompanyName, x.Phone }));

        var pagedParameter = new ShipperPageParameter { From = 1, Size = 5 };

        // act
        var response = await this._httpClient.GetAsync($"{BaseRequestUrl}/from/{pagedParameter.From}/size/{pagedParameter.Size}");

        // assert
        response.Should().Be200Ok()
                .And
                .Satisfy<SuccessResultOutputModel<IEnumerable<ShipperOutputModel>>>(model =>
                {
                    model.Status.Should().Be("Success");
                    model.Method.Should().Be("/api/shipper/from/1/size/5.GET");
                    model.Data.Should().NotBeEmpty();
                    model.Data.Should().HaveCount(5);
                    model.Data.First().ShipperId.Should().Be(1);
                    model.Data.Last().ShipperId.Should().Be(5);
                });
    }

    [Theory]
    [AutoData]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper/from/a/size/b.GET")]
    public async Task GetCollectionAsync_from為6_size為5_資料庫有10筆資料_回傳的集合應有五筆(
        [CollectionSize(10)] IEnumerable<ShipperParameter> parameters)
    {
        // arrange
        parameters.ForEach(x => InsertShipperData(new { x.CompanyName, x.Phone }));

        var pagedParameter = new ShipperPageParameter { From = 6, Size = 5 };

        // act
        var response = await this._httpClient.GetAsync($"{BaseRequestUrl}/from/{pagedParameter.From}/size/{pagedParameter.Size}");

        // assert
        response.Should().Be200Ok()
                .And
                .Satisfy<SuccessResultOutputModel<IEnumerable<ShipperOutputModel>>>(model =>
                {
                    model.Status.Should().Be("Success");
                    model.Method.Should().Be("/api/shipper/from/6/size/5.GET");
                    model.Data.Should().NotBeEmpty();
                    model.Data.Should().HaveCount(5);
                    model.Data.First().ShipperId.Should().Be(6);
                    model.Data.Last().ShipperId.Should().Be(10);
                });
    }

    [Theory]
    [AutoData]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper/from/a/size/b.GET")]
    public async Task GetCollectionAsync_from為6_size為10_資料庫有10筆資料_回傳的集合應有五筆(
        [CollectionSize(10)] IEnumerable<ShipperParameter> parameters)
    {
        // arrange
        parameters.ForEach(x => InsertShipperData(new { x.CompanyName, x.Phone }));

        var pagedParameter = new ShipperPageParameter { From = 6, Size = 10 };

        // act
        var response = await this._httpClient.GetAsync($"{BaseRequestUrl}/from/{pagedParameter.From}/size/{pagedParameter.Size}");

        // assert
        response.Should().Be200Ok()
                .And
                .Satisfy<SuccessResultOutputModel<IEnumerable<ShipperOutputModel>>>(model =>
                {
                    model.Status.Should().Be("Success");
                    model.Method.Should().Be("/api/shipper/from/6/size/10.GET");
                    model.Data.Should().NotBeEmpty();
                    model.Data.Should().HaveCount(5);
                    model.Data.First().ShipperId.Should().Be(6);
                    model.Data.Last().ShipperId.Should().Be(10);
                });
    }

    [Theory]
    [AutoData]
    [Trait("Owner", "Kevin")]
    [Trait("Category", "api/shipper/from/a/size/b.GET")]
    public async Task GetCollectionAsync_from為11_size為5_資料庫有10筆資料_超出範圍_回傳應為空集合(
        [CollectionSize(10)] IEnumerable<ShipperParameter> parameters)
    {
        // arrange
        parameters.ForEach(x => InsertShipperData(new { x.CompanyName, x.Phone }));

        var pagedParameter = new ShipperPageParameter { From = 11, Size = 5 };

        // act
        var response = await this._httpClient.GetAsync($"{BaseRequestUrl}/from/{pagedParameter.From}/size/{pagedParameter.Size}");

        // assert
        response.Should().Be200Ok()
                .And
                .Satisfy<SuccessResultOutputModel<IEnumerable<ShipperOutputModel>>>(model =>
                {
                    model.Status.Should().Be("Success");
                    model.Method.Should().Be("/api/shipper/from/11/size/5.GET");
                    model.Data.Should().BeEmpty();
                });
    }

    //---------------------------------------------------------------------------------------------
    // SearchAsync

    [Fact]
    public async Task SearchAsync_未帶入CompanyName與Phone_不符合規格_ValidationError_應回傳BadRequestResult_StatusCode為400()
    {
        // arrange
        var requestUri = $"{BaseRequestUrl}/search";

        // act
        var response = await this._httpClient.GetAsync(requestUri);

        // assert
        response.Should().Be400BadRequest()
                .And
                .Satisfy<FailureResultOutputModel>(model =>
                {
                    model.Method.Should().Be("/api/shipper/search.GET");
                    model.Status.Should().Be("ValidationError");

                    var errors = JsonSerializer.Deserialize<List<FailureInformation>>(JsonSerializer.Serialize(model.Errors));
                    errors.Should().NotBeEmpty();
                    errors.Exists(x => x.Message.Contains("CompanyName error")).Should().BeTrue();
                    errors.Exists(x => x.Message.Contains("Phone error")).Should().BeTrue();
                });
    }

    [Fact]
    public async Task SearchAsync_CompanyName與Phone都是空白字串_不符合規格_ValidationError_應回傳BadRequestResult_StatusCode為400()
    {
        // arrange
        var parameter = new ShipperSearchParameter { CompanyName = "", Phone = "" };

        var requestUri = $"{BaseRequestUrl}/search?CompanyName={parameter.CompanyName}&Phone={parameter.Phone}";

        // act
        var response = await this._httpClient.GetAsync(requestUri);

        // assert
        response.Should().Be400BadRequest()
                .And
                .Satisfy<FailureResultOutputModel>(model =>
                {
                    model.Method.Should().Be("/api/shipper/search.GET");
                    model.Status.Should().Be("ValidationError");

                    var errors = JsonSerializer.Deserialize<List<FailureInformation>>(JsonSerializer.Serialize(model.Errors));
                    errors.Should().NotBeEmpty();
                    errors.Exists(x => x.Message.Contains("CompanyName error")).Should().BeTrue();
                    errors.Exists(x => x.Message.Contains("Phone error")).Should().BeTrue();
                });
    }

    [Fact]
    public async Task SearchAsync_CompanyName有輸入_Phone為空白字串_資料庫無資料_回傳集合應為空()
    {
        // arrange
        var parameter = new ShipperSearchParameter { CompanyName = "test", Phone = "" };

        var requestUri = $"{BaseRequestUrl}/search".SetQueryParams(parameter).ToUri();

        // act
        var response = await this._httpClient.GetAsync(requestUri);

        // assert
        response.Should().Be200Ok()
                .And
                .Satisfy<SuccessResultOutputModel<IEnumerable<ShipperOutputModel>>>(model =>
                {
                    model.Method.Should().Be("/api/shipper/search.GET");
                    model.Status.Should().Be("Success");
                    model.Data.Should().BeEmpty();
                });
    }

    [Fact]
    public async Task SearchAsync_CompanyName為空白字串_Phone有輸入_資料庫無資料_回傳集合應為空()
    {
        // arrange
        var parameter = new ShipperSearchParameter { CompanyName = "", Phone = "12345" };

        var requestUri = $"{BaseRequestUrl}/search".SetQueryParams(parameter).ToUri();

        // act
        var response = await this._httpClient.GetAsync(requestUri);

        // assert
        response.Should().Be200Ok()
                .And
                .Satisfy<SuccessResultOutputModel<IEnumerable<ShipperOutputModel>>>(model =>
                {
                    model.Method.Should().Be("/api/shipper/search.GET");
                    model.Status.Should().Be("Success");
                    model.Data.Should().BeEmpty();
                });
    }

    [Fact]
    public async Task SearchAsync_CompanyName有輸入_Phone有輸入_資料庫無資料_回傳集合應為空()
    {
        // arrange
        var parameter = new ShipperSearchParameter { CompanyName = "test", Phone = "12345" };

        var requestUri = $"{BaseRequestUrl}/search".SetQueryParams(parameter).ToUri();

        // act
        var response = await this._httpClient.GetAsync(requestUri);

        // assert
        response.Should().Be200Ok()
                .And
                .Satisfy<SuccessResultOutputModel<IEnumerable<ShipperOutputModel>>>(model =>
                {
                    model.Method.Should().Be("/api/shipper/search.GET");
                    model.Status.Should().Be("Success");
                    model.Data.Should().BeEmpty();
                });
    }

    [Theory]
    [AutoData]
    public async Task SearchAsync_CompanyName有輸入_Phone有輸入_資料庫有資料_但都不符合條件_回傳集合應為空(
        [CollectionSize(10)] IEnumerable<ShipperParameter> parameters)
    {
        // arrange
        parameters.ForEach(x => InsertShipperData(new { x.CompanyName, x.Phone }));

        var parameter = new ShipperSearchParameter { CompanyName = "test", Phone = "12345" };

        var requestUri = $"{BaseRequestUrl}/search".SetQueryParams(parameter).ToUri();

        // act
        var response = await this._httpClient.GetAsync(requestUri);

        // assert
        response.Should().Be200Ok()
                .And
                .Satisfy<SuccessResultOutputModel<IEnumerable<ShipperOutputModel>>>(model =>
                {
                    model.Method.Should().Be("/api/shipper/search.GET");
                    model.Status.Should().Be("Success");
                    model.Data.Should().BeEmpty();
                });
    }

    [Theory]
    [AutoData]
    public async Task SearchAsync_CompanyName有輸入_Phone有輸入_資料庫有資料_有一筆資料符合條件_回傳集合應包含符合條件的資料(
        [CollectionSize(10)] IEnumerable<ShipperParameter> parameters)
    {
        // arrange
        parameters.ForEach(x => InsertShipperData(new { x.CompanyName, x.Phone }));
        InsertShipperData(new { CompanyName = "test", Phone = "12345" });

        var parameter = new ShipperSearchParameter { CompanyName = "test", Phone = "12345" };

        var requestUri = $"{BaseRequestUrl}/search".SetQueryParams(parameter).ToUri();

        // act
        var response = await this._httpClient.GetAsync(requestUri);

        // assert
        response.Should().Be200Ok()
                .And
                .Satisfy<SuccessResultOutputModel<IEnumerable<ShipperOutputModel>>>(model =>
                {
                    model.Method.Should().Be("/api/shipper/search.GET");
                    model.Status.Should().Be("Success");
                    model.Data.Should().NotBeEmpty();
                    model.Data.Should().HaveCount(1);
                    model.Data.First().CompanyName.Should().Be("test");
                    model.Data.First().Phone.Should().Be("12345");
                });
    }

    //---------------------------------------------------------------------------------------------

    /// <summary>
    /// Inserts the shipper data using the specified model
    /// </summary>
    /// <param name="model">The model</param>
    private static void InsertShipperData(object model)
    {
        const string sqlCommand = """
                                  Insert into Shippers (CompanyName, Phone)
                                  Values (@CompanyName, @Phone)
                                  """;
        DatabaseCommand.ExecuteSqlCommand(ProjectFixture.SampleDbConnectionString, sqlCommand, model);
    }
}