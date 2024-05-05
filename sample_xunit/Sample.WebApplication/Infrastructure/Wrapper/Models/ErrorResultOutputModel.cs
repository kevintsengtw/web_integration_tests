using System.Text.Json.Serialization;

namespace Sample.WebApplication.Infrastructure.Wrapper.Models;

/// <summary>
/// class ErrorResultOutputModel
/// </summary>
public class ErrorResultOutputModel
{
    /// <summary>
    /// The CorrelationId.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// The Method.
    /// </summary>
    [JsonPropertyName("method")]
    public string Method { get; set; }

    /// <summary>
    /// The Status.
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; }

    /// <summary>
    /// Data.
    /// </summary>
    [JsonPropertyName("data")]
    public object Data { get; set; }
}