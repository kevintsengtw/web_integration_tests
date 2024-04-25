using AutoFixture;
using AutoFixture.AutoNSubstitute;

namespace Sample.WebApplicationTests;

public class WebApplicationFixture
{
    private IFixture _fixture;

    public IFixture Fixture => this._fixture ??= new Fixture().Customize(new AutoNSubstituteCustomization())
                                                              .Customize(new MapsterMapperCustomization());
}