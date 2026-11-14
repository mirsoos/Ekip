using System.Data.Common;

namespace Ekip.Application.Interfaces;

public interface ISqlConnectionFactory
{
    ValueTask<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default);
}