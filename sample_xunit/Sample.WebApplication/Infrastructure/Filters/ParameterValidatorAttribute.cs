using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Sample.WebApplication.Infrastructure.Exceptions;

namespace Sample.WebApplication.Infrastructure.Filters;

/// <summary>
/// class ParameterValidatorAttribute.
/// </summary>
public class ParameterValidatorAttribute : ActionFilterAttribute
{
    private readonly string _nameOfParameter;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterValidatorAttribute"/> class.
    /// </summary>
    /// <param name="nameOfParameter">name of parameter</param>
    public ParameterValidatorAttribute(string nameOfParameter)
    {
        // 有指定被驗證的 parameter 引數名稱 (這通常會在 Action 方法的方法簽章裡有多個 arguments 的時候)
        this._nameOfParameter = nameOfParameter;
    }

    /// <summary>
    /// on action execution as an asynchronous operation.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="next">The next.</param>
    /// <inheritdoc />
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var parameters = context.ActionArguments;
        if (parameters.Count <= 0)
        {
            // model binding errors
            var errorMessage = context.ModelState
                                      .Where(e => e.Value.Errors.Count > 0)
                                      .Select(e => e.Value.Errors[0].ErrorMessage)
                                      .First();

            context.Result = new BadRequestObjectResult(errorMessage);
            await base.OnActionExecutionAsync(context, next);
        }
        else
        {
            await this.ValidateParameterAsync(context, next);
        }
    }

    /// <summary>
    /// validate parameter as an asynchronous operation.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <exception cref="FluentValidationException"></exception>
    private async Task ValidateParameterAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var parameters = context.ActionArguments;

        object parameterObject;

        if (string.IsNullOrWhiteSpace(this._nameOfParameter))
        {
            parameterObject = parameters.FirstOrDefault().Value;
        }
        else
        {
            parameterObject = parameters.Any(x => x.Key.Equals(this._nameOfParameter, StringComparison.OrdinalIgnoreCase))
                ? parameters.FirstOrDefault(x => x.Key.Equals(this._nameOfParameter, StringComparison.OrdinalIgnoreCase)).Value
                : null;
        }

        if (parameterObject is null)
        {
            context.Result = new BadRequestObjectResult("輸入 Parameter 錯誤");
            await base.OnActionExecutionAsync(context, next);
        }
        else
        {
            var parameterTypeName = parameterObject.GetType().Name;
            var validator = context.HttpContext.RequestServices.GetKeyedService<IValidator>(parameterTypeName);

            var validationContext = new ValidationContext<object>(parameterObject);
            var validationResult = await validator.ValidateAsync(validationContext);

            if (!validationResult.IsValid)
            {
                throw new FluentValidationException
                {
                    HttpStatusCode = HttpStatusCode.BadRequest,
                    Errors = validationResult.Errors
                };
            }

            await base.OnActionExecutionAsync(context, next);
        }
    }
}