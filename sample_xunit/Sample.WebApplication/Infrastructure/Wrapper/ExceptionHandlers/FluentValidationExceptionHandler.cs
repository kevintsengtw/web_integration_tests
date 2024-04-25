using System.Text.Json;
using System.Text.Json.Serialization;
using Correlate;
using Microsoft.AspNetCore.Diagnostics;
using Sample.WebApplication.Infrastructure.Exceptions;
using Sample.WebApplication.Infrastructure.Validators;
using Sample.WebApplication.Infrastructure.Wrapper.Models;

namespace Sample.WebApplication.Infrastructure.Wrapper.ExceptionHandlers;

/// <summary>
/// class FluentValidationExceptionHandler
/// </summary>
public class FluentValidationExceptionHandler : IExceptionHandler
{
    /// <summary>
    /// The logger
    /// </summary>
    private readonly ILogger<FluentValidationExceptionHandler> _logger;

    /// <summary>
    /// The correlationContextAccessor
    /// </summary>
    private readonly ICorrelationContextAccessor _correlationContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="FluentValidationExceptionHandler"/> class
    /// </summary>
    /// <param name="logger">The logger</param>
    /// <param name="correlationContextAccessor">The correlation context accessor</param>
    public FluentValidationExceptionHandler(ILogger<FluentValidationExceptionHandler> logger,
                                            ICorrelationContextAccessor correlationContextAccessor)
    {
        this._logger = logger;
        this._correlationContextAccessor = correlationContextAccessor;
    }

    /// <summary>
    /// Gets the value of the serializer options
    /// </summary>
    private static JsonSerializerOptions SerializerOptions => new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    /// <summary>
    /// Tries the handle using the specified http context
    /// </summary>
    /// <param name="httpContext">The http context</param>
    /// <param name="exception">The exception</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A value task of bool</returns>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not FluentValidationException fluentValidationException)
        {
            return false;
        }

        this._logger.LogError(
            fluentValidationException,
            "FluentValidationException occurred: {Message}", fluentValidationException.Message);

        var failResultOutputModel = new FailureResultOutputModel
        {
            Id = this._correlationContextAccessor.CorrelationContext?.CorrelationId,
            Method = $"{httpContext.Request.Path}.{httpContext.Request.Method}",
            Status = "ValidationError",
            Errors = fluentValidationException.Errors.Select(
                item => new FailureInformation
                {
                    Message = $"{item.PropertyName} error",
                    Description = item.ErrorMessage
                })
        };

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = (int)fluentValidationException.HttpStatusCode;

        await httpContext.Response.WriteAsJsonAsync(failResultOutputModel, SerializerOptions, cancellationToken);
        return true;
    }
}