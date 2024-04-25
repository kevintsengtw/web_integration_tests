namespace Sample.Domain.Settings;

/// <summary>
/// class RedisConfiguration
/// </summary>
public class RedisConfiguration
{
    /// <summary>
    /// Hosts
    /// </summary>
    public string[] Hosts { get; set; }

    /// <summary>
    /// Ports
    /// </summary>
    public int[] Ports { get; set; }

    /// <summary>
    /// Configuration
    /// </summary>
    public string Configuration => string.Join(",", this.Hosts
                                                        .Select((t, i) => $"{t}:{this.Ports[i]}")
                                                        .OrderBy(x => Random.Shared.Next()));
}