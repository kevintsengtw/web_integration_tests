using Mapster;
using Sample.Domain.Entities;
using Sample.Service.Dto;

namespace Sample.Service.MapConfig;

public class ServiceMapRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ShipperModel, ShipperDto>().TwoWays();
    }
}