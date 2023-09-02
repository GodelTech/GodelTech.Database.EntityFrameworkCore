using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GodelTech.Database.EntityFrameworkCore.IntegrationTests.Fakes
{
    public class FakeSqlExecutor : ISqlExecutor
    {
        public IList<string> SqlStrings { get; } = new List<string>();

        public Task<int> ExecuteSqlRawAsync(DbContext dbContext, string sql, CancellationToken cancellationToken = default)
        {
            SqlStrings.Add(sql);

            return Task.FromResult(0);
        }
    }
}
