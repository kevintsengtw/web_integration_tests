using System.Diagnostics;
using Xunit.Abstractions;

namespace Sample.WebApplicationIntegrationTests.Utilities;

/// <summary>
/// class HttpClientLogger
/// </summary>
/// <seealso cref="DelegatingHandler"/>
public class HttpClientLogger : DelegatingHandler
{
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpClientLogger"/> class
    /// </summary>
    /// <param name="testOutputHelper">The test output helper</param>
    public HttpClientLogger(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    /// <summary>
    /// Sends the request
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The response</returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await LogHttpRequestAsync(request);

        var stopwatch = Stopwatch.StartNew();

        var response = await base.SendAsync(request, cancellationToken);

        stopwatch.Stop();

        await LogHttpResponseAsync(response, stopwatch.Elapsed);

        return response;
    }

    /// <summary>
    /// Logs the http request using the specified request
    /// </summary>
    /// <param name="request">The request</param>
    private async Task LogHttpRequestAsync(HttpRequestMessage request)
    {
        _testOutputHelper.WriteLine($"Request Method: {request.Method}");
        _testOutputHelper.WriteLine($"Request URL: {request.RequestUri}");

        foreach (var header in request.Headers)
        {
            _testOutputHelper.WriteLine($"Request Header: {header.Key} - {string.Join(", ", header.Value)}");
        }

        if (request.Content != null)
        {
            var content = await request.Content.ReadAsStringAsync();
            _testOutputHelper.WriteLine($"Request Content: {content}");
        }
    }

    /// <summary>
    /// Logs the http response using the specified response
    /// </summary>
    /// <param name="response">The response</param>
    /// <param name="duration">The duration</param>
    private async Task LogHttpResponseAsync(HttpResponseMessage response, TimeSpan duration)
    {
        _testOutputHelper.WriteLine($"Response HttpStatusCode: {response.StatusCode} {response.StatusCode.GetHashCode()}");
        _testOutputHelper.WriteLine($"Response Time: {duration.TotalMilliseconds} ms");

        foreach (var header in response.Headers)
        {
            _testOutputHelper.WriteLine($"Response Header: {header.Key} - {string.Join(", ", header.Value)}");
        }

        var content = await response.Content.ReadAsStringAsync();

        _testOutputHelper.WriteLine($"Response Content: {content}");
    }
}