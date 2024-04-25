using System;
using AutoFixture;
using AutoFixture.AutoNSubstitute;

namespace Sample.ServiceTests;

public class ServiceFixture
{
    private IFixture _fixture;

    public IFixture Fixture => this._fixture ??= new Fixture().Customize(new AutoNSubstituteCustomization())
                                                              .Customize(new MapsterMapperCustomization());
}