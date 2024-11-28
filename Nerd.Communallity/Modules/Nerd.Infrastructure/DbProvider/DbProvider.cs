using Dapper;
using Microsoft.Data.SqlClient;
using Nerd.Domain.Abstractions;
using System.Data;
using System.Data.Common;

namespace Nerd.Infrastructure.DbProvider;

public class DbProvider(string connectionString) : IDbProvider
{
    private SqlConnection CreateConnection()
    {
        return new SqlConnection(connectionString);
    }

    public async Task<int> ExecuteAsync(string sql, object? param = null)
    {
        using SqlConnection connection = CreateConnection();
        await connection.OpenAsync();
        return await connection.ExecuteAsync(sql, param);
    }

    public async Task<T?> ExecuteScalarAsync<T>(string sql, object? param = null)
    {
        using SqlConnection connection = CreateConnection();
        await connection.OpenAsync();
        return await connection.ExecuteScalarAsync<T>(sql, param);
    }

    public async Task<T> QuerySingleAsync<T>(string sql, object? param = null)
    {
        using SqlConnection connection = CreateConnection();
        await connection.OpenAsync();
        return await connection.QuerySingleAsync<T>(sql, param);
    }

    public async IAsyncEnumerable<T> QueryIncrementallyAsync<T>(CommandDefinition commandDefinition, CommandBehavior behavior = CommandBehavior.CloseConnection)
    {
        using SqlConnection connection = CreateConnection();
        await using DbDataReader reader = await connection.ExecuteReaderAsync(commandDefinition, behavior);

        Func<DbDataReader, T> rowParser = reader.GetRowParser<T>();

        while (await reader.ReadAsync())
        {
            yield return rowParser(reader);
        }
    }
}
