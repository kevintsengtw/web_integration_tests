using FluentValidation;
using Sample.WebApplication.Models.InputParameters;

namespace Sample.WebApplication.Infrastructure.Validators;

/// <summary>
/// Class ShipperUpdateParameterValidator
/// </summary>
public class ShipperUpdateParameterValidator : AbstractValidator<ShipperUpdateParameter>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShipperUpdateParameterValidator"/> class
    /// </summary>
    public ShipperUpdateParameterValidator()
    {
        this.RuleFor(x => x.ShipperId)
            .GreaterThan(0).WithMessage("ShipperId 必須大於 0");

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