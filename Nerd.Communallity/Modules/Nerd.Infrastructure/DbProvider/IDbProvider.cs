using Dapper;
using System.Data;

namespace Nerd.Domain.Abstractions;

public interface IDbProvider
{
    Task<int> ExecuteAsync(string sql, object? param = null);
    Task<T?> ExecuteScalarAsync<T>(string sql, object? param = null);
    Task<T> QuerySingleAsync<T>(string sql, object? param = null);
    IAsyncEnumerable<T> QueryIncrementallyAsync<T>(CommandDefinition commandDefinition, CommandBehavior behavior = CommandBehavior.CloseConnection);
}
