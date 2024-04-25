using System;

namespace Sample.RepositoryTests.TestData;

public static class TableNames
{
    public static string Shippers => "Shippers";

    public static IEnumerable<string> TableNameCollection => new List<string>
    {
        Shippers
    };
}