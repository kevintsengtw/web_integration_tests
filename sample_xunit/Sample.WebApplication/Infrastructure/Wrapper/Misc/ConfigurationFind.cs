using System.Diagnostics;
using System.Reflection;

namespace Sample.WebApplication.Infrastructure.Wrapper.Misc;

/// <summary>
/// Class ConfigurationFind.
/// </summary>
public class ConfigurationFind
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationFind"/> class
    /// </summary>
    protected ConfigurationFind()
    {
    }

    /// <summary>
    /// Gets the type of the configuration.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <returns>System.Int32.</returns>
    internal static ConfigurationType GetConfigurationType(Assembly assembly)
    {
        var assm = assembly;
        var attributes = assm.GetCustomAttributes(typeof(DebuggableAttribute), false);
        if (attributes.Length.Equals(0))
        {
            return ConfigurationType.Release;
        }

        foreach (Attribute attr in attributes)
        {
            if (attr is not DebuggableAttribute debuggableAttribute)
            {
                continue;
            }

            return debuggableAttribute.IsJITOptimizerDisabled ? ConfigurationType.Debug : ConfigurationType.Release;
        }

        return ConfigurationType.Unknown;
    }
}