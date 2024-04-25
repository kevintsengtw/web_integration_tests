using System.ComponentModel.DataAnnotations;

namespace Sample.Domain.Validation;

/// <summary>
/// Class NotDefaultAttribute.
/// Implements the <see cref="System.ComponentModel.DataAnnotations.ValidationAttribute" />
/// </summary>
/// <seealso cref="System.ComponentModel.DataAnnotations.ValidationAttribute" />
public class NotDefaultAttribute : ValidationAttribute
{
    public const string DefaultErrorMessage = "The {0} field must not have the default value";

    /// <summary>
    /// Initializes a new instance of the <see cref="NotDefaultAttribute"/> class.
    /// </summary>
    public NotDefaultAttribute() : base(DefaultErrorMessage)
    {
    }

    /// <summary>
    /// Determines whether the specified value of the object is valid.
    /// </summary>
    /// <param name="value">The value of the object to validate.</param>
    /// <returns><see langword="true" /> if the specified value is valid; otherwise, <see langword="false" />.</returns>
    public override bool IsValid(object value)
    {
        // NotDefault doesn't necessarily mean required
        if (value is null)
        {
            return true;
        }

        var type = value.GetType();
        if (type.IsValueType is false)
        {
            // non-null ref type
            return true;
        }

        var defaultValue = Activator.CreateInstance(type);
        return !value.Equals(defaultValue);
    }
}