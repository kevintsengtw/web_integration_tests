using FluentValidation.TestHelper;
using Sample.WebApplication.Infrastructure.Validators;
using Sample.WebApplication.Models.InputParameters;
using Sample.WebApplicationTests.Utilities;

namespace Sample.WebApplicationTests.Infrastructure.Validators;

public class ShipperUpdateParameterValidatorTests
{
    private readonly ShipperUpdateParameterValidator _validator;

    public ShipperUpdateParameterValidatorTests()
    {
        this._validator = new ShipperUpdateParameterValidator();
    }

    //---------------------------------------------------------------------------------------------

    [Fact]
    public void Validate_Id輸入為0_驗證應有error()
    {
        var result = this._validator.TestValidate(new ShipperUpdateParameter { ShipperId = 0 });

        const string expectedMessage = "ShipperId 必須大於 0";

        result.ShouldHaveValidationErrorFor(x => x.ShipperId)
              .WithErrorMessage(expectedMessage);
    }

    [Fact]
    public void Validate_Id輸入為負1_驗證應有error()
    {
        var result = this._validator.TestValidate(new ShipperUpdateParameter { ShipperId = -1 });

        const string expectedMessage = "ShipperId 必須大於 0";

        result.ShouldHaveValidationErrorFor(x => x.ShipperId)
              .WithErrorMessage(expectedMessage);
    }

    [Fact]
    public void Validate_CompanyName為null_驗證結果應有error()
    {
        var result = this._validator.TestValidate(new ShipperUpdateParameter { CompanyName = null });

        const string expectedMessage = "CompanyName 不可為 null";

        result.ShouldHaveValidationErrorFor(x => x.CompanyName)
              .WithErrorMessage(expectedMessage);
    }

    [Fact]
    public void Validate_CompanyName為空白字串_驗證結果應有error()
    {
        var result = this._validator.TestValidate(new ShipperUpdateParameter { CompanyName = string.Empty });

        const string expectedMessage = "未輸入 CompanyName";

        result.ShouldHaveValidationErrorFor(x => x.CompanyName)
              .WithErrorMessage(expectedMessage);
    }

    [Fact]
    public void Validate_CompanyName的字串長度超過40_驗證結果應有error()
    {
        var result = this._validator.TestValidate(new ShipperUpdateParameter
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
        var result = this._validator.TestValidate(new ShipperUpdateParameter { Phone = null });

        const string expectedMessage = "Phone 不可為 null";

        result.ShouldHaveValidationErrorFor(x => x.Phone)
              .WithErrorMessage(expectedMessage);
    }

    [Fact]
    public void Validate_Phone為空白字串_驗證結果應有error()
    {
        var result = this._validator.TestValidate(new ShipperUpdateParameter { Phone = string.Empty });

        const string expectedMessage = "未輸入 Phone";

        result.ShouldHaveValidationErrorFor(x => x.Phone)
              .WithErrorMessage(expectedMessage);
    }

    [Fact]
    public void Validate_Phone的字串長度超過24_驗證結果應有error()
    {
        var result = this._validator.TestValidate(new ShipperUpdateParameter
        {
            Phone = StringUtilities.RandomString(50)
        });

        const string expectedMessage = "Phone 長度不可超過 24";

        result.ShouldHaveValidationErrorFor(x => x.Phone)
              .WithErrorMessage(expectedMessage);
    }
}