using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GodelTech.Database.EntityFrameworkCore
{
    internal class SqlExecutor : ISqlExecutor
    {
        public async Task<int> ExecuteSqlRawAsync(DbContext dbContext, string sql, CancellationToken cancellationToken = default)
        {
            return await dbContext.Database.ExecuteSqlRawAsync(sql, cancellationToken);
        }
    }
}
