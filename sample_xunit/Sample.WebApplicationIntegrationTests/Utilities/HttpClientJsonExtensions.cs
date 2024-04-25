using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Sample.WebApplicationIntegrationTests.Utilities;

/// <summary>
/// class HttpClientJsonExtensions
/// </summary>
public static class HttpClientJsonExtensions
{
    /// <summary>
    /// Sends a DELETE request as an asynchronous operation to the specified Uri with the given <paramref name="value" /> serialized as JSON.
    /// </summary>
    /// <typeparam name="TValue">The value</typeparam>
    /// <param name="client">The client</param>
    /// <param name="requestUri">The request uri</param>
    /// <param name="value">The value</param>
    /// <param name="options">The options</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task containing the http response message</returns>
    public static async Task<HttpResponseMessage> DeleteAsJsonAsync<TValue>(this HttpClient client,
                                                                            string requestUri,
                                                                            TValue value,
                                                                            JsonSerializerOptions options = null,
                                                                            CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Content = value is null
            ? null
            : new StringContent(JsonSerializer.Serialize(value, options), Encoding.UTF8, new MediaTypeHeaderValue("application/json"));
        return await client.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false);
    }
}