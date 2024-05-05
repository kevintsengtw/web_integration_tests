// ReSharper disable ClassNeverInstantiated.Global

namespace Sample.WebApplicationIntegrationTests.Tests;

/// <summary>
/// Class ApiTestClassFixture
/// </summary>
public sealed class ApiTestClassFixture
{
    public readonly TestWebApplicationFactory<Program> TestWebApplicationFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiTestClassFixture"/> class
    /// </summary>
    public ApiTestClassFixture()
    {
        this.TestWebApplicationFactory = new TestWebApplicationFactory<Program>();
    }
}