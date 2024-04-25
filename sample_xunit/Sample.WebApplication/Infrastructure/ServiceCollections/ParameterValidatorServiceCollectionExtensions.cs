using System.Reflection;
using FluentValidation;

namespace Sample.WebApplication.Infrastructure.ServiceCollections;

/// <summary>
/// class ParameterValidatorServiceCollectionExtensions
/// </summary>
public static class ParameterValidatorServiceCollectionExtensions
{
    /// <summary>
    /// add the Keyed ParameterValidators.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddKeyedParameterValidators(this IServiceCollection services)
    {
        var validators = Assembly.GetExecutingAssembly().GetTypes()
                                 .Where(t => typeof(IValidator).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                                 .ToList();

        foreach (var validatorType in validators)
        {
            var parameterName = validatorType.Name.Replace("Validator", "");
            services.AddKeyedScoped(typeof(IValidator), parameterName, validatorType);
        }

        return services;
    }
}