using AutoFixture;
using MapsterMapper;
using Sample.Domain.Repositories;
using Sample.Service.Implements;

namespace Sample.ServiceTests.Implements;

public class ShipperServiceClassFixture : ServiceFixture
{
    public StubShipperService Stub => this.Fixture.Create<StubShipperService>();
}

public class StubShipperService
{
    public IMapper Mapper { get; set; }

    public IShipperRepository ShipperRepository { get; set; }

    public ShipperService SystemUnderTest => new(this.Mapper, this.ShipperRepository);
}