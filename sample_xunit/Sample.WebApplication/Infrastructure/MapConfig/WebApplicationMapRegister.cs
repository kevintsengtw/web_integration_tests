using Mapster;
using Sample.Service.Dto;
using Sample.WebApplication.Models.InputParameters;
using Sample.WebApplication.Models.OutputModels;

namespace Sample.WebApplication.Infrastructure.MapConfig;

/// <summary>
/// class WebApplicationMapRegister
/// </summary>
public class WebApplicationMapRegister : IRegister
{
    /// <summary>
    /// Register
    /// </summary>
    /// <param name="config">config</param>
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ShipperDto, ShipperOutputModel>();

        config.NewConfig<ShipperParameter, ShipperDto>()
              .Map(d => d.CompanyName, s => s.CompanyName)
              .Map(d => d.Phone, s => s.Phone);
    }
}