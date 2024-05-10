using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Sample.WebApplication.Infrastructure.Wrapper.ExceptionHandlers;
using Sample.WebApplication.Infrastructure.Wrapper.Models;
using Sample.WebApplicationTests.AutoFixtureConfigurations;
using Xunit.Abstractions;

namespace Sample.WebApplicationTests.Infrastructure.Wrapper.ExceptionHandlers;

public class GlobalExceptionHandlerTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public GlobalExceptionHandlerTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    //---------------------------------------------------------------------------------------------

    [Theory]
    [AutoDataWithCustomization]
    public async Task HandleAsync_收到Exception_應回傳FailureResultOutputModel(GlobalExceptionHandler handler)
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        using var memoryStream = new MemoryStream();
        httpContext.Response.Body = memoryStream;

        var exception = new InvalidOperationException("Test exception");

        // Act
        var actual = await handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        // Assert
        actual.Should().BeTrue();
        httpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

        memoryStream.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(memoryStream);
        var responseBody = await reader.ReadToEndAsync();

        _testOutputHelper.WriteLine(responseBody);

        var failureResultOutputModel = JsonSerializer.Deserialize<FailureResultOutputModel>(responseBody);
        failureResultOutputModel.Status.Should().Be("Error");

        var errors = DeserializeContent<List<FailureInformation>>(failureResultOutputModel.Errors);
        errors.Should().HaveCount(1);
        errors[0].Message.Should().Be("Test exception");
        errors[0].Description.Should().Contain(nameof(InvalidOperationException));
    }

    private static T DeserializeContent<T>(object data)
    {
        var result = JsonSerializer.Deserialize<T>(
            JsonSerializer.Serialize(data),
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        return result;
    }
}