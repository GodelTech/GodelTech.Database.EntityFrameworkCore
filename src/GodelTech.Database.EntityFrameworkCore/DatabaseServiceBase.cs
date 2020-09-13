using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GodelTech.Database.EntityFrameworkCore
{
    /// <summary>
    /// Base class for DatabaseService.
    /// </summary>
    public class DatabaseServiceBase
    {
        private readonly ILogger _logger;

        private readonly IList<DbContext> _dbContexts;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="dbContexts">Database contexts.</param>
        protected DatabaseServiceBase(ILogger logger, params DbContext[] dbContexts)
        {
            _logger = logger;

            _dbContexts = new List<DbContext>(dbContexts);
        }

        /// <summary>
        /// Apply migrations for provided contexts.
        /// </summary>
        public async Task ApplyMigrations()
        {
            foreach (var dbContext in _dbContexts)
            {
                _logger.LogInformation("Apply migrations: {dbContext}", dbContext.GetType().Name);
                await dbContext.Database.MigrateAsync();
            }
        }

        /// <summary>
        /// Delete migrations for provided contexts.
        /// </summary>
        public async Task DeleteMigrations()
        {
            foreach (var dbContext in _dbContexts.Reverse())
            {
                _logger.LogInformation("Delete migrations: {dbContext}", dbContext.GetType().Name);
                await dbContext.GetService<IMigrator>().MigrateAsync("0");
            }
        }
    }
}
