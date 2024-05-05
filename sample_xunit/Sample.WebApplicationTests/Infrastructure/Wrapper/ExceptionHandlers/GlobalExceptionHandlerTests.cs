using System.Text.Json;
using Correlate;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sample.WebApplication.Infrastructure.Wrapper.ExceptionHandlers;
using Sample.WebApplication.Infrastructure.Wrapper.Models;
using Xunit.Abstractions;

namespace Sample.WebApplicationTests.Infrastructure.Wrapper.ExceptionHandlers;

public class GlobalExceptionHandlerTests
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    private readonly ICorrelationContextAccessor _correlationContextAccessor;

    private readonly GlobalExceptionHandler _handler;

    private readonly ITestOutputHelper _testOutputHelper;

    public GlobalExceptionHandlerTests(ITestOutputHelper testOutputHelper)
    {
        this._logger = Substitute.For<ILogger<GlobalExceptionHandler>>();
        this._correlationContextAccessor = Substitute.For<ICorrelationContextAccessor>();
        this._handler = new GlobalExceptionHandler(this._logger, this._correlationContextAccessor);
        this._testOutputHelper = testOutputHelper;
    }

    //---------------------------------------------------------------------------------------------

    [Fact]
    public async Task HandleAsync_收到Exception_應回傳FailureResultOutputModel()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        using var memoryStream = new MemoryStream();
        httpContext.Response.Body = memoryStream;

        var exception = new InvalidOperationException("Test exception");

        // Act
        var actual = await this._handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        // Assert
        actual.Should().BeTrue();
        httpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

        memoryStream.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(memoryStream);
        var responseBody = await reader.ReadToEndAsync();

        this._testOutputHelper.WriteLine(responseBody);

        var failureResultOutputModel = JsonSerializer.Deserialize<FailureResultOutputModel>(responseBody);
        failureResultOutputModel.Status.Should().Be("Error");

        var errors = JsonSerializer.Deserialize<List<FailureInformation>>(JsonSerializer.Serialize(failureResultOutputModel.Errors), new JsonSerializerOptions(JsonSerializerDefaults.Web));
        errors.Should().HaveCount(1);
        errors[0].Message.Should().Be("Test exception");
        errors[0].Description.Should().Contain(nameof(InvalidOperationException));
    }
}