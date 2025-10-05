using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace QuickForm.Modules.Person.Persistence;
internal sealed class DbConnectionFactory(string connectionString) : IDbConnectionFactory
{
    private readonly string _connectionString = connectionString;

    public async ValueTask<DbConnection> OpenConnectionAsync()
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }
}
