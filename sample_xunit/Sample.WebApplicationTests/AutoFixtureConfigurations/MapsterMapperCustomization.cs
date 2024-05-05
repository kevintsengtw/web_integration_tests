using AutoFixture;
using Mapster;
using MapsterMapper;
using Sample.WebApplication.Infrastructure.MapConfig;

namespace Sample.WebApplicationTests.AutoFixtureConfigurations;

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

            var typeAdapterConfig = new TypeAdapterConfig();
            typeAdapterConfig.Scan(typeof(WebApplicationMapRegister).Assembly);
            this._mapper = new Mapper(typeAdapterConfig);
            return this._mapper;
        }
    }
}