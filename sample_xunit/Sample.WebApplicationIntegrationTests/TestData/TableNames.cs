namespace Sample.WebApplicationIntegrationTests.TestData;

/// <summary>
/// class TableNames
/// </summary>
public static class TableNames
{
    public static string Shippers => "Shippers";

    public static IEnumerable<string> TableNameCollection => new List<string>
    {
        Shippers
    };
}