using System;
using AutoFixture;
using Mapster;
using MapsterMapper;
using Sample.Service.MapConfig;

namespace Sample.ServiceTests;

public class MapsterMapperCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Register(() => this.Mapper);
    }

    private IMapper _mapper;

    private IMapper Mapper
    {
        get
        {
            if (this._mapper is not null)
            {
                return this._mapper;
            }

            var serviceAssembly = typeof(ServiceMapRegister).Assembly;
            TypeAdapterConfig.GlobalSettings.Scan(serviceAssembly);
            var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
            this._mapper = new Mapper(typeAdapterConfig);
            return this._mapper;
        }
    }
}