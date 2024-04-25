using System.Text.Json.Serialization;

namespace Sample.WebApplication.Infrastructure.Wrapper.Models;

/// <summary>
/// 執行失敗的 OutputModel.
/// </summary>
public class FailureResultOutputModel
{
    /// <summary>
    /// The CorrelationId
    /// </summary>
    [JsonPropertyOrder(0)]
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// The Method
    /// </summary>
    [JsonPropertyOrder(1)]
    [JsonPropertyName("method")]
    public string Method { get; set; }

    /// <summary>
    /// The status
    /// </summary>
    [JsonPropertyOrder(2)]
    [JsonPropertyName("status")]
    public string Status { get; set; }

    /// <summary>
    /// The errors
    /// </summary>
    [JsonPropertyOrder(3)]
    [JsonPropertyName("errors")]
    public object Errors { get; set; }
}

/// <summary>
/// 執行失敗的 OutputModel.
/// </summary>
public class FailureResultOutputModel<T>
{
    /// <summary>
    /// The CorrelationId
    /// </summary>
    [JsonPropertyOrder(0)]
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// The Method
    /// </summary>
    [JsonPropertyOrder(1)]
    [JsonPropertyName("method")]
    public string Method { get; set; }

    /// <summary>
    /// The status
    /// </summary>
    [JsonPropertyOrder(2)]
    [JsonPropertyName("status")]
    public string Status { get; set; }

    /// <summary>
    /// The errors
    /// </summary>
    [JsonPropertyOrder(3)]
    [JsonPropertyName("errors")]
    public T Errors { get; set; }
}