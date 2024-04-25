using FluentValidation.TestHelper;
using Sample.WebApplication.Infrastructure.Validators;
using Sample.WebApplication.Models.InputParameters;

namespace Sample.WebApplicationTests.Infrastructure.Validators;

public class ShipperPageParameterValidatorTests
{
    private readonly ShipperPageParameterValidator _validator;

    public ShipperPageParameterValidatorTests()
    {
        this._validator = new ShipperPageParameterValidator();
    }
    
    //---------------------------------------------------------------------------------------------

    [Fact]
    public void Validate_from為0_驗證結果應有error()
    {
        var result = this._validator.TestValidate(new ShipperPageParameter
        {
            From = 0,
            Size = 10
        });

        const string expectedMessage = "'From' 必須大於等於 1";

        result.ShouldHaveValidationErrorFor(x => x.From)
              .WithErrorMessage(expectedMessage);
    }
    
    [Fact]
    public void Validate_from為負1_驗證結果應有error()
    {
        var result = this._validator.TestValidate(new ShipperPageParameter
        {
            From = -1,
            Size = 10
        });

        const string expectedMessage = "'From' 必須大於等於 1";

        result.ShouldHaveValidationErrorFor(x => x.From)
              .WithErrorMessage(expectedMessage);
    }
    
    [Fact]
    public void Validate_size為0_驗證結果應有error()
    {
        var result = this._validator.TestValidate(new ShipperPageParameter
        {
            From = 1,
            Size = 0
        });

        const string expectedMessage = "'Size' 必須大於等於 1";

        result.ShouldHaveValidationErrorFor(x => x.Size)
              .WithErrorMessage(expectedMessage);
    }
    
    [Fact]
    public void Validate_size為負1_驗證結果應有error()
    {
        var result = this._validator.TestValidate(new ShipperPageParameter
        {
            From = 1,
            Size = -1
        });

        const string expectedMessage = "'Size' 必須大於等於 1";

        result.ShouldHaveValidationErrorFor(x => x.Size)
              .WithErrorMessage(expectedMessage);
    }
    
    [Fact]
    public void Validate_size為101_驗證結果應有error()
    {
        var result = this._validator.TestValidate(new ShipperPageParameter
        {
            From = 9,
            Size = 101
        });

        const string expectedMessage = "'Size' 不可大於 100";

        result.ShouldHaveValidationErrorFor(x => x.Size)
              .WithErrorMessage(expectedMessage);
    }
}