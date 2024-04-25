using FluentValidation.TestHelper;
using Sample.WebApplication.Infrastructure.Validators;
using Sample.WebApplication.Models.InputParameters;

namespace Sample.WebApplicationTests.Infrastructure.Validators;

public class ShipperIdParameterValidatorTests
{
    private readonly ShipperIdParameterValidator _validator;

    public ShipperIdParameterValidatorTests()
    {
        this._validator = new ShipperIdParameterValidator();
    }

    //---------------------------------------------------------------------------------------------

    [Fact]
    public void Validate_Id輸入為0_驗證應有error()
    {
        var result = this._validator.TestValidate(new ShipperIdParameter { ShipperId = 0 });

        const string expectedMessage = "ShipperId 必須大於 0";

        result.ShouldHaveValidationErrorFor(x => x.ShipperId)
              .WithErrorMessage(expectedMessage);
    }

    [Fact]
    public void Validate_Id輸入為負1_驗證應有error()
    {
        var result = this._validator.TestValidate(new ShipperIdParameter { ShipperId = -1 });

        const string expectedMessage = "ShipperId 必須大於 0";

        result.ShouldHaveValidationErrorFor(x => x.ShipperId)
              .WithErrorMessage(expectedMessage);
    }
}