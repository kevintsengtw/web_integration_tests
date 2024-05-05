using System.Text.Json.Serialization;

namespace Sample.WebApplication.Infrastructure.Wrapper.Models;

/// <summary>
/// 執行完成的 OutputModel
/// </summary>
/// <typeparam name="T">任意型別</typeparam>
public class SuccessResultOutputModel<T>
{
    /// <summary>
    /// CorrelationId.
    /// </summary>
    [JsonPropertyOrder(0)]
    [JsonPropertyName("id")]
    public string CorrelationId { get; set; }

    /// <summary>
    /// Method.
    /// </summary>
    [JsonPropertyOrder(2)]
    [JsonPropertyName("method")]
    public string Method { get; set; }

    /// <summary>
    /// Status.
    /// </summary>
    [JsonPropertyOrder(3)]
    [JsonPropertyName("status")]
    public string Status { get; set; } = "Success";

    /// <summary>
    /// Data.
    /// </summary>
    [JsonPropertyOrder(4)]
    [JsonPropertyName("data")]
    public T Data { get; set; }
}

/// <summary>
/// class SuccessResultOutputModel
/// </summary>
public class SuccessResultOutputModel
{
    /// <summary>
    /// The CorrelationId.
    /// </summary>
    [JsonPropertyOrder(0)]
    [JsonPropertyName("id")]
    public Guid? Id { get; set; }

    /// <summary>
    /// The Method.
    /// </summary>
    [JsonPropertyOrder(1)]
    [JsonPropertyName("method")]
    public string Method { get; set; }

    /// <summary>
    /// The Status.
    /// </summary>
    [JsonPropertyOrder(2)]
    [JsonPropertyName("status")]
    public string Status { get; set; }

    /// <summary>
    /// Data.
    /// </summary>
    [JsonPropertyOrder(3)]
    [JsonPropertyName("data")]
    public object Data { get; set; }
}