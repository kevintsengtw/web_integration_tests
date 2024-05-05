using System.Reflection;
using Microsoft.Data.SqlClient;

namespace Sample.WebApplicationIntegrationTests.Utilities;

public static class SqlExceptionCreator
{
    private static T Construct<T>(params object[] p)
    {
        var ctors = typeof(T).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
        return (T)ctors.First(ctor => ctor.GetParameters().Length == p.Length).Invoke(p);
    }

    internal static SqlException Create(string message, int number = 1)
    {
        var collection = Construct<SqlErrorCollection>();
        var error = Construct<SqlError>(number, (byte)2, (byte)3, "server name", message, "proc", 100, null);

        typeof(SqlErrorCollection)
            .GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(collection, new object[] { error });

        return typeof(SqlException)
               .GetMethod
               (
                   "CreateException", BindingFlags.NonPublic | BindingFlags.Static,
                   null,
                   CallingConventions.ExplicitThis,
                   new[] { typeof(SqlErrorCollection), typeof(string) },
                   new ParameterModifier[] { }
               )
               .Invoke(null, new object[] { collection, "7.0.0" }) as SqlException;
    }
}