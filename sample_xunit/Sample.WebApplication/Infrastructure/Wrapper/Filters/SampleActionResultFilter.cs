using Correlate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Sample.WebApplication.Infrastructure.Wrapper.Models;

namespace Sample.WebApplication.Infrastructure.Wrapper.Filters;

/// <summary>
/// class SampleActionResultFilter
/// </summary>
public class SampleActionResultFilter : IAsyncActionFilter
{
    /// <summary>
    /// Called asynchronously before the action, after model binding is complete.
    /// </summary>
    /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext" />.</param>
    /// <param name="next">The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate" />. Invoked to execute the next action filter or the action itself.</param>
    /// <returns>
    /// A <see cref="T:System.Threading.Tasks.Task" /> that on completion indicates the filter has executed.
    /// </returns>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executedContext = await next();

        if (executedContext.Result is ObjectResult result)
        {
            switch (result.Value)
            {
                case HttpResponseMessage _:
                    return;
                case SuccessResultOutputModel<object> _:
                    return;
                case SuccessResultOutputModel _:
                    return;
            }

            var correlationContextAccessor = context.HttpContext.RequestServices.GetRequiredService<ICorrelationContextAccessor>();
            var correlationId = correlationContextAccessor.CorrelationContext?.CorrelationId;

            var controllerTypeName = context.Controller.GetType().Name;
            var notActionController = controllerTypeName.Equals("ActionController", StringComparison.OrdinalIgnoreCase) is false;

            if (notActionController)
            {
                var requestPath = context.HttpContext.Request.Path;
                var httpMethod = context.HttpContext.Request.Method;

                switch (result.StatusCode)
                {
                    case < 400 and >= 200:
                    {
                        var successResponse = new SuccessResultOutputModel<object>
                        {
                            CorrelationId = correlationId,
                            Method = $"{requestPath}.{httpMethod}",
                            Status = result.StatusCode >= 400 ? "Faliure" : "Success",
                            Data = result.Value
                        };

                        executedContext.Result = new ObjectResult(successResponse) { StatusCode = result.StatusCode };
                        return;
                    }
                    case >= 400:
                    {
                        var errorResponse = new FailureResultOutputModel<object>
                        {
                            Id = correlationId,
                            Method = $"{requestPath}.{httpMethod}",
                            Status = "Error",
                            Errors = result.Value
                        };

                        executedContext.Result = new ObjectResult(errorResponse) { StatusCode = result.StatusCode };
                        return;
                    }
                    default:
                    {
                        var response = new SuccessResultOutputModel<object>
                        {
                            CorrelationId = correlationId,
                            Method = $"{requestPath}.{httpMethod}",
                            Data = result.Value
                        };

                        executedContext.Result = new ObjectResult(response) { StatusCode = result.StatusCode };
                        break;
                    }
                }
            }
        }
    }
}