using AutoFixture;
using Mapster;
using MapsterMapper;
using Sample.Service.MapConfig;

namespace Sample.ServiceTests.AutoFixtureConfigurations;

/// <summary>
/// class MapsterMapperCustomization
/// </summary>
public class MapsterMapperCustomization : ICustomization
{
    /// <summary>
    /// Customizes the fixture
    /// </summary>
    /// <param name="fixture">The fixture</param>
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
            typeAdapterConfig.Scan(typeof(ServiceMapRegister).Assembly);
            this._mapper = new Mapper(typeAdapterConfig);
            return this._mapper;
        }
    }
}