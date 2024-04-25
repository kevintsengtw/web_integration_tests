using Mapster;
using MapsterMapper;
using Sample.Service.MapConfig;
using Sample.WebApplication.Infrastructure.MapConfig;

namespace Sample.WebApplication.Infrastructure.ServiceCollections;

/// <summary>
/// Class MapSterServiceCollectionExtensions
/// </summary>
public static class MapSterServiceCollectionExtensions
{
    /// <summary>
    /// Add MapSter
    /// </summary>
    /// <param name="services">services</param>
    /// <returns></returns>
    public static IServiceCollection AddMapSter(this IServiceCollection services)
    {
        var config = new TypeAdapterConfig();

        var serviceAssembly = typeof(ServiceMapRegister).Assembly;
        var webAssembly = typeof(WebApplicationMapRegister).Assembly;

        config.Scan(serviceAssembly);
        config.Scan(webAssembly);

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}