using System.Net;
using FluentValidation.Results;

namespace Sample.WebApplication.Infrastructure.Exceptions;

/// <summary>
/// Class FluentValidationException.
/// </summary>
/// <seealso cref="System.Exception" />
public class FluentValidationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FluentValidationException"/> class.
    /// </summary>
    public FluentValidationException()
    {
    }

    /// <summary>
    /// HttpStatusCode.
    /// </summary>
    public HttpStatusCode HttpStatusCode { get; set; } = HttpStatusCode.BadRequest;

    /// <summary>
    /// ValidationFailures.
    /// </summary>
    /// <value>The errors.</value>
    public List<ValidationFailure> Errors { get; set; } = [];
}