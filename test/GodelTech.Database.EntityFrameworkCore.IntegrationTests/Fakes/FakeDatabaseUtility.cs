using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GodelTech.Database.EntityFrameworkCore.IntegrationTests.Fakes
{
    public class FakeDatabaseUtility : IDatabaseUtility
    {
        public bool WasOpened { get; private set; }
        public IList<string> SqlStrings { get; } = new List<string>();
        public bool WasClosed { get; private set; }

        public async Task OpenConnectionAsync([NotNull] DbContext dbContext, CancellationToken cancellationToken = default)
        {
            await dbContext.Database.OpenConnectionAsync(cancellationToken);

            WasOpened = true;
        }

        public Task<int> ExecuteSqlRawAsync(DbContext dbContext, string sql, CancellationToken cancellationToken = default)
        {
            SqlStrings.Add(sql);

            return Task.FromResult(0);
        }

        public async Task CloseConnectionAsync([NotNull] DbContext dbContext)
        {
            await dbContext.Database.CloseConnectionAsync();

            WasClosed = true;
        }
    }
}
