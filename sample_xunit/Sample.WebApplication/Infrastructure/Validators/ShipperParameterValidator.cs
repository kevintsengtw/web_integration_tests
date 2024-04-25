using FluentValidation;
using Sample.WebApplication.Models.InputParameters;

namespace Sample.WebApplication.Infrastructure.Validators;

/// <summary>
/// class ShipperParameterValidator
/// </summary>
public class ShipperParameterValidator : AbstractValidator<ShipperParameter>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShipperParameterValidator"/> class
    /// </summary>
    public ShipperParameterValidator()
    {
        this.RuleFor(x => x.CompanyName)
            .NotNull().WithMessage("CompanyName 不可為 null")
            .NotEmpty().WithMessage("未輸入 CompanyName")
            .MaximumLength(40).WithMessage("CompanyName 長度不可超過 40");

        this.RuleFor(x => x.Phone)
            .NotNull().WithMessage("Phone 不可為 null")
            .NotEmpty().WithMessage("未輸入 Phone")
            .MaximumLength(24).WithMessage("Phone 長度不可超過 24");
    }
}