using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GodelTech.Database.EntityFrameworkCore.IntegrationTests.Fakes
{
    public class FakeDatabaseUtility : IDatabaseUtility
    {
        private readonly Action _onOpenConnection;
        private readonly Action<string> _onExecuteSql;
        private readonly Action _onCloseConnection;

        public FakeDatabaseUtility(
            Action onOpenConnection = null,
            Action<string> onExecuteSql = null,
            Action onCloseConnection = null)
        {
            _onOpenConnection = onOpenConnection;
            _onExecuteSql = onExecuteSql;
            _onCloseConnection = onCloseConnection;
        }

        public async Task OpenConnectionAsync([NotNull] DbContext dbContext, CancellationToken cancellationToken = default)
        {
            await dbContext.Database.OpenConnectionAsync(cancellationToken);

            _onOpenConnection?.Invoke();
        }

        public Task<int> ExecuteSqlRawAsync(DbContext dbContext, string sql, CancellationToken cancellationToken = default)
        {
            _onExecuteSql?.Invoke(sql);

            return Task.FromResult(0);
        }

        public async Task CloseConnectionAsync([NotNull] DbContext dbContext)
        {
            await dbContext.Database.CloseConnectionAsync();

            _onCloseConnection?.Invoke();
        }
    }
}
