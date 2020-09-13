using GodelTech.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging;
using System;
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
        private readonly IDictionary<Type, IDataService> _dataServices;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="dbContexts">Database contexts.</param>
        protected DatabaseServiceBase(ILogger logger, params DbContext[] dbContexts)
        {
            _logger = logger;
            _dbContexts = new List<DbContext>(dbContexts);
            _dataServices = new Dictionary<Type, IDataService>();
        }

        /// <summary>
        /// Apply migrations for provided contexts.
        /// </summary>
        public async Task ApplyMigrationsAsync()
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
        public async Task DeleteMigrationsAsync()
        {
            foreach (var dbContext in _dbContexts.Reverse())
            {
                _logger.LogInformation("Delete migrations: {dbContext}", dbContext.GetType().Name);
                await dbContext.GetService<IMigrator>().MigrateAsync("0");
            }
        }

        /// <summary>
        /// Apply data using data services.
        /// </summary>
        public async Task ApplyDataAsync()
        {
            foreach (var dataService in _dataServices)
            {
                await dataService.Value.ApplyDataAsync();
            }
        }

        /// <summary>
        /// Registers data service instance.
        /// </summary>
        /// <typeparam name="TEntity">The type of the T entity.</typeparam>
        /// <typeparam name="TType">The type of the T type.</typeparam>
        /// <param name="dataService">The data service.</param>
        protected void RegisterDataService<TEntity, TType>(IDataService dataService)
            where TEntity : class, IEntity<TType>
        {
            _dataServices[typeof(TEntity)] = dataService;
        }
    }
}
