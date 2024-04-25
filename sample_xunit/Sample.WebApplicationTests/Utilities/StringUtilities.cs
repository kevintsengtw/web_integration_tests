namespace Sample.WebApplicationTests.Utilities;

/// <summary>
/// class StringUtilities
/// </summary>
public static class StringUtilities
{
    /// <summary>
    /// RandomString
    /// </summary>
    /// <param name="length">length</param>
    /// <returns></returns>
    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string
        (
            Enumerable.Repeat(chars, length)
                      .Select(s => s[Random.Shared.Next(s.Length)])
                      .ToArray()
        );
    }
}