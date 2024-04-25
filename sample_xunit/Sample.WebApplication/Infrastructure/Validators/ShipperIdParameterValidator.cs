using FluentValidation;
using Sample.WebApplication.Models.InputParameters;

namespace Sample.WebApplication.Infrastructure.Validators;

/// <summary>
/// class ShipperIdParameterValidator
/// </summary>
public class ShipperIdParameterValidator : AbstractValidator<ShipperIdParameter>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShipperIdParameterValidator"/> class
    /// </summary>
    public ShipperIdParameterValidator()
    {
        this.RuleFor(x => x.ShipperId)
            .GreaterThan(0).WithMessage("ShipperId 必須大於 0");
    }
}