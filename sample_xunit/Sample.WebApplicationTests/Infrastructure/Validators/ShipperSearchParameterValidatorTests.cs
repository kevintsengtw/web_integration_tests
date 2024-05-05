using FluentValidation.TestHelper;
using Sample.WebApplication.Infrastructure.Validators;
using Sample.WebApplication.Models.InputParameters;

namespace Sample.WebApplicationTests.Infrastructure.Validators;

public class ShipperSearchParameterValidatorTests
{
    private readonly ShipperSearchParameterValidator _validator;

    public ShipperSearchParameterValidatorTests()
    {
        this._validator = new ShipperSearchParameterValidator();
    }

    //---------------------------------------------------------------------------------------------

    [Fact]
    public void Validate_CompanyName與Phone都為null_驗證結果應有error()
    {
        var result = this._validator.TestValidate(new ShipperSearchParameter
        {
            CompanyName = null,
            Phone = null
        });

        result.ShouldHaveValidationErrorFor(x => x.CompanyName);
    }

    [Fact]
    public void Validate_CompanyName與Phone都為空白字串_驗證結果應有error()
    {
        var result = this._validator.TestValidate(new ShipperSearchParameter
        {
            CompanyName = string.Empty,
            Phone = string.Empty
        });

        result.ShouldHaveValidationErrorFor(x => x.CompanyName);
    }

    [Fact]
    public void Validate_CompanyName為null_Phone為空白字串_驗證結果應有error()
    {
        var result = this._validator.TestValidate(new ShipperSearchParameter
        {
            CompanyName = null,
            Phone = string.Empty
        });

        result.ShouldHaveValidationErrorFor(x => x.CompanyName);
    }

    [Fact]
    public void Validate_CompanyName為空白字串_Phone為null_驗證結果應有error()
    {
        var result = this._validator.TestValidate(new ShipperSearchParameter
        {
            CompanyName = string.Empty,
            Phone = null
        });

        result.ShouldHaveValidationErrorFor(x => x.CompanyName);
    }

    [Fact]
    public void Validate_CompanyName為null_Phone有值_驗證結果不應有error()
    {
        var result = this._validator.TestValidate(new ShipperSearchParameter
        {
            CompanyName = null,
            Phone = "02123456789"
        });

        result.ShouldNotHaveValidationErrorFor(x => x.CompanyName);
    }

    [Fact]
    public void Validate_CompanyName為空白字串_Phone有值_驗證結果不應有error()
    {
        var result = this._validator.TestValidate(new ShipperSearchParameter
        {
            CompanyName = string.Empty,
            Phone = "02123456789"
        });

        result.ShouldNotHaveValidationErrorFor(x => x.CompanyName);
    }

    [Fact]
    public void Validate_CompanyName為有值_Phone為null_驗證結果不應有error()
    {
        var result = this._validator.TestValidate(new ShipperSearchParameter
        {
            CompanyName = "test",
            Phone = null
        });

        result.ShouldNotHaveValidationErrorFor(x => x.CompanyName);
    }

    [Fact]
    public void Validate_CompanyName有值_Phone為空白字串_驗證結果不應有error()
    {
        var result = this._validator.TestValidate(new ShipperSearchParameter
        {
            CompanyName = "test",
            Phone = string.Empty
        });

        result.ShouldNotHaveValidationErrorFor(x => x.CompanyName);
    }
}