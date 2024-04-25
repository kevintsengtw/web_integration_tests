using FluentValidation;
using Sample.WebApplication.Models.InputParameters;

namespace Sample.WebApplication.Infrastructure.Validators;

/// <summary>
/// class ShipperPageParameterValidator
/// </summary>
public class ShipperPageParameterValidator : AbstractValidator<ShipperPageParameter>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShipperPageParameterValidator"/> class
    /// </summary>
    public ShipperPageParameterValidator()
    {
        this.RuleFor(x => x.From)
            .GreaterThanOrEqualTo(1).WithMessage("'{PropertyName}' 必須大於等於 1");

        this.RuleFor(x => x.Size)
            .GreaterThanOrEqualTo(1).WithMessage("'{PropertyName}' 必須大於等於 1")
            .LessThanOrEqualTo(100).WithMessage("'{PropertyName}' 不可大於 100");
    }
}