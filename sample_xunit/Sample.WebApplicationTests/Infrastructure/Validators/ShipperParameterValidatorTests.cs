using FluentValidation.TestHelper;
using Sample.WebApplication.Infrastructure.Validators;
using Sample.WebApplication.Models.InputParameters;
using Sample.WebApplicationTests.Utilities;

namespace Sample.WebApplicationTests.Infrastructure.Validators;

public class ShipperParameterValidatorTests
{
    private readonly ShipperParameterValidator _validator;

    public ShipperParameterValidatorTests()
    {
        this._validator = new ShipperParameterValidator();
    }

    //---------------------------------------------------------------------------------------------

    [Fact]
    public void Validate_CompanyName為null_驗證結果應有error()
    {
        var result = this._validator.TestValidate(new ShipperParameter { CompanyName = null });

        const string expectedMessage = "CompanyName 不可為 null";

        result.ShouldHaveValidationErrorFor(x => x.CompanyName)
              .WithErrorMessage(expectedMessage);
    }

    [Fact]
    public void Validate_CompanyName為空白字串_驗證結果應有error()
    {
        var result = this._validator.TestValidate(new ShipperParameter { CompanyName = string.Empty });

        const string expectedMessage = "未輸入 CompanyName";

        result.ShouldHaveValidationErrorFor(x => x.CompanyName)
              .WithErrorMessage(expectedMessage);
    }

    [Fact]
    public void Validate_CompanyName的字串長度超過40_驗證結果應有error()
    {
        var result = this._validator.TestValidate(new ShipperParameter
        {
            CompanyName = StringUtilities.RandomString(50)
        });

        const string expectedMessage = "CompanyName 長度不可超過 40";

        result.ShouldHaveValidationErrorFor(x => x.CompanyName)
              .WithErrorMessage(expectedMessage);
    }
    
    [Fact]
    public void Validate_Phone為null_驗證結果應有error()
    {
        var result = this._validator.TestValidate(new ShipperParameter { Phone = null });

        const string expectedMessage = "Phone 不可為 null";

        result.ShouldHaveValidationErrorFor(x => x.Phone)
              .WithErrorMessage(expectedMessage);
    }

    [Fact]
    public void Validate_Phone為空白字串_驗證結果應有error()
    {
        var result = this._validator.TestValidate(new ShipperParameter { Phone = string.Empty });

        const string expectedMessage = "未輸入 Phone";

        result.ShouldHaveValidationErrorFor(x => x.Phone)
              .WithErrorMessage(expectedMessage);
    }

    [Fact]
    public void Validate_Phone的字串長度超過24_驗證結果應有error()
    {
        var result = this._validator.TestValidate(new ShipperParameter
        {
            Phone = StringUtilities.RandomString(50)
        });

        const string expectedMessage = "Phone 長度不可超過 24";

        result.ShouldHaveValidationErrorFor(x => x.Phone)
              .WithErrorMessage(expectedMessage);
    }    
}