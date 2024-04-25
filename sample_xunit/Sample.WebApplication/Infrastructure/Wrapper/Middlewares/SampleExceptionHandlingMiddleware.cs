using System.Reflection;
using Correlate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Sample.WebApplication.Infrastructure.Wrapper.Filters;
using Sample.WebApplication.Infrastructure.Wrapper.Misc;
using Sample.WebApplication.Infrastructure.Wrapper.Models;

namespace Sample.WebApplication.Infrastructure.Wrapper.Middlewares;

/// <summary>
/// class SampleExceptionHandlingMiddleware
/// </summary>
public class SampleExceptionHandlingMiddleware
{
    /// <summary>
    /// The next
    /// </summary>
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="SampleExceptionHandlingMiddleware"/> class
    /// </summary>
    /// <param name="next">The next</param>
    public SampleExceptionHandlingMiddleware(RequestDelegate next)
    {
        this._next = next;
    }

    private static ConfigurationType ConfigurationType
    {
        get
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var buildType = ConfigurationFind.GetConfigurationType(executingAssembly);
            return buildType;
        }
    }

    /// <summary>
    /// Invokes the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await this._next(context);
        }
        catch (Exception ex)
        {
            ApiControllerAttribute apiControllerAttribute = null;
            WrapIgnoreAttribute wrapIgnoreAttribute = null;

            var endpoint = context.GetEndpoint();
            if (endpoint is not null)
            {
                var controllerActionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
                if ((controllerActionDescriptor is null) is false)
                {
                    apiControllerAttribute = (ApiControllerAttribute)controllerActionDescriptor.ControllerTypeInfo.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();
                }

                if (endpoint.Metadata?.GetMetadata<WrapIgnoreAttribute>() != null)
                {
                    wrapIgnoreAttribute = endpoint.Metadata?.GetMetadata<WrapIgnoreAttribute>();
                }
            }

            if (apiControllerAttribute is null)
            {
                await this._next(context);
            }
            else
            {
                if (wrapIgnoreAttribute is { ShouldIgnore: true })
                {
                    await this._next(context);
                }
                else
                {
                    var (message, description) = GetMessageDescription(ex);

                    await ResponseFailureResultOutputModel(context, message, description, ex);
                }
            }
        }
    }

    /// <summary>
    /// 輸出 FailureResultOutputModel (單數的 Error)
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="message">The message.</param>
    /// <param name="description">The description.</param>
    /// <param name="ex">The ex.</param>
    private static async Task ResponseFailureResultOutputModel(HttpContext context, string message, string description, Exception ex)
    {
        var correlationContextAccessor = context.RequestServices.GetRequiredService<ICorrelationContextAccessor>();
        var correlationId = correlationContextAccessor.CorrelationContext?.CorrelationId;

        var result = new FailureResultOutputModel
        {
            Id = correlationId,
            Method = $"{context.Request.Path}.{context.Request.Method}",
            Status = "Error",
            Errors = new List<FailureInformation>
            {
                new()
                {
                    Message = message,
                    Description = description
                }
            }
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(result);
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

        if (ConfigurationType.Equals(ConfigurationType.Debug))
        {
            message = ex.GetBaseException().Message;
            description = ex.ToString();

            return (message, description);
        }

        message = "An unhandled error occurred.";
        description = string.Empty;

        return (message, description);
    }
}