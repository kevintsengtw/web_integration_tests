using FluentValidation;
using Sample.WebApplication.Models.InputParameters;

namespace Sample.WebApplication.Infrastructure.Validators;

/// <summary>
/// class ShipperSearchParameterValidator
/// </summary>
public class ShipperSearchParameterValidator : AbstractValidator<ShipperSearchParameter>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShipperSearchParameterValidator"/> class
    /// </summary>
    public ShipperSearchParameterValidator()
    {
        this.RuleFor(x => x.CompanyName)
            .NotNull().When(x => string.IsNullOrWhiteSpace(x.Phone))
            .NotEmpty().When(x => string.IsNullOrWhiteSpace(x.Phone));

        this.RuleFor(x => x.Phone)
            .NotNull().When(x => string.IsNullOrWhiteSpace(x.CompanyName))
            .NotEmpty().When(x => string.IsNullOrWhiteSpace(x.CompanyName));
    }
}