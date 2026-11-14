using System.Data.Common;
using Dapper;
using Ekip.Application.Interfaces;
using Npgsql;

namespace Ekip.Persistence.Context;

internal sealed class SqlConnectionFactory(NpgsqlDataSource dataSource) : ISqlConnectionFactory
{
    static SqlConnectionFactory()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
    }

    public async ValueTask<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default) =>
        await dataSource.OpenConnectionAsync(cancellationToken);
}