using System;
using AutoFixture;
using Mapster;
using MapsterMapper;
using Sample.WebApplication.Infrastructure.MapConfig;

namespace Sample.WebApplicationTests;

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

            var serviceAssembly = typeof(WebApplicationMapRegister).Assembly;
            TypeAdapterConfig.GlobalSettings.Scan(serviceAssembly);
            var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
            this._mapper = new Mapper(typeAdapterConfig);
            return this._mapper;
        }
    }
}