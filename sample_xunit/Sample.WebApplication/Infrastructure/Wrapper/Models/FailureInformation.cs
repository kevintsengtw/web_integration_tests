using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Sample.WebApplication.Infrastructure.Wrapper.Models;

/// <summary>
/// class FailureInformation
/// </summary>
public class FailureInformation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FailureInformation"/> class
    /// </summary>
    public FailureInformation()
    {
        this.Message = string.Empty;
        this.Description = string.Empty;
    }
    
    /// <summary>
    /// The message.
    /// </summary>
    [JsonProperty(PropertyName = "message", Order = 2)]
    [JsonPropertyName("message")]
    public string Message { get; set; }

    /// <summary>
    /// The description.
    /// </summary>
    [JsonProperty(PropertyName = "description", Order = 3)]
    [JsonPropertyName("description")]
    public string Description { get; set; }
}