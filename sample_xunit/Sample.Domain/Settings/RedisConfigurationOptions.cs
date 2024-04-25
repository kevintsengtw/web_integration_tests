using System;

namespace Sample.Domain.Settings;

/// <summary>
/// Class RedisConfigurationOptions
/// </summary>
public class RedisConfigurationOptions
{
    /// <summary>
    /// The Section Name
    /// </summary>
    public const string SectionName = "RedisConnection";
    
    /// <summary>
    /// InstanceName
    /// </summary>
    public string InstanceName { get; set; }
    
    /// <summary>
    /// Configuration
    /// </summary>
    public RedisConfiguration RedisConfiguration { get; set; }
}