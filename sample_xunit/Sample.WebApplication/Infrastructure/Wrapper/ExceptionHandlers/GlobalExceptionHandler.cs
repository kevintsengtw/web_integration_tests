using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Correlate;
using Microsoft.AspNetCore.Diagnostics;
using Sample.WebApplication.Infrastructure.Wrapper.Misc;
using Sample.WebApplication.Infrastructure.Wrapper.Models;

namespace Sample.WebApplication.Infrastructure.Wrapper.ExceptionHandlers;

/// <summary>
/// class GlobalExceptionHandler
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    /// <summary>
    /// The logger
    /// </summary>
    private readonly ILogger<GlobalExceptionHandler> _logger;

    /// <summary>
    /// The correlationContextAccessor
    /// </summary>
    private readonly ICorrelationContextAccessor _correlationContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalExceptionHandler"/> class
    /// </summary>
    /// <param name="logger">The logger</param>
    /// <param name="correlationContextAccessor">The correlationContextAccessor</param>
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger,
                                  ICorrelationContextAccessor correlationContextAccessor)
    {
        this._logger = logger;
        this._correlationContextAccessor = correlationContextAccessor;
    }

    private static JsonSerializerOptions SerializerOptions => new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    /// <summary>
    /// Gets the value of the build type
    /// </summary>
    private static ConfigurationType BuildType
    {
        get
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var buildType = ConfigurationFind.GetConfigurationType(executingAssembly);
            return buildType;
        }
    }

    /// <summary>
    /// Tries the handle using the specified http context
    /// </summary>
    /// <param name="httpContext">The http context</param>
    /// <param name="exception">The exception</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A value task of bool</returns>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        this._logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

        var (message, description) = GetMessageDescription(exception);

        var failResultOutputModel = new FailureResultOutputModel
        {
            Id = this._correlationContextAccessor.CorrelationContext?.CorrelationId,
            Method = $"{httpContext.Request.Path}.{httpContext.Request.Method}",
            Status = "Error",
            Errors = new List<FailureInformation>()
            {
                new()
                {
                    Message = message,
                    Description = description
                }
            }
        };

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(failResultOutputModel, SerializerOptions, cancellationToken);
        return true;
    }

    /// <summary>
    /// Gets the message and description.
    /// </summary>
    /// <param name="ex">The ex.</param>
    /// <returns>System.ValueTuple&lt;System.String, System.String&gt;.</returns>
    private static (string message, string description) GetMessageDescription(Exception ex)
    {
        string message;
        string description;

        if (BuildType.Equals(ConfigurationType.Debug))
        {
            message = ex.GetBaseException().Message;
            description = $"{ex}";

            return (message, description);
        }

        message = "An unhandled exception has occurred while executing the request.";
        description = string.Empty;

        return (message, description);
    }
}