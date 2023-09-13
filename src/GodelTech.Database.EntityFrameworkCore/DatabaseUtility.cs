using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GodelTech.Database.EntityFrameworkCore
{
    internal class DatabaseUtility : IDatabaseUtility
    {
        public async Task OpenConnectionAsync(DbContext dbContext, CancellationToken cancellationToken = default)
        {
            await dbContext.Database.OpenConnectionAsync(cancellationToken);
        }

        public async Task<int> ExecuteSqlRawAsync(DbContext dbContext, string sql, CancellationToken cancellationToken = default)
        {
            return await dbContext.Database.ExecuteSqlRawAsync(sql, cancellationToken);
        }

        public async Task CloseConnectionAsync(DbContext dbContext)
        {
            await dbContext.Database.CloseConnectionAsync();
        }
    }
}
