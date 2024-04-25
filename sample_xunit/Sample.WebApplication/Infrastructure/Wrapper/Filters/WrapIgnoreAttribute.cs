using System;

namespace Sample.WebApplication.Infrastructure.Wrapper.Filters;

/// <summary>
/// class WrapIgnoreAttribute.
/// </summary>
public class WrapIgnoreAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the value of the ShouldIgnore
    /// </summary>
    public bool ShouldIgnore { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WrapIgnoreAttribute"/> class
    /// </summary>
    public WrapIgnoreAttribute()
    {
        this.ShouldIgnore = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WrapIgnoreAttribute"/> class
    /// </summary>
    /// <param name="shouldIgnore">The should ignore</param>
    public WrapIgnoreAttribute(bool shouldIgnore)
    {
        this.ShouldIgnore = shouldIgnore;
    }
}