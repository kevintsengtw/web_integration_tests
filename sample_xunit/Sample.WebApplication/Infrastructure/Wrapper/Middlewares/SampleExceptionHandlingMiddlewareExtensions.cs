namespace Sample.WebApplication.Infrastructure.Wrapper.Middlewares;

/// <summary>
/// class SampleExceptionHandlingMiddlewareExtensions
/// </summary>
public static class SampleExceptionHandlingMiddlewareExtensions
{
    /// <summary>
    /// Uses the exception handling.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns></returns>
    public static IApplicationBuilder UseSampleExceptionHandling(this IApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.UseMiddleware<SampleExceptionHandlingMiddleware>();
    }
}