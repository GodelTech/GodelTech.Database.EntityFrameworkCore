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
    public abstract class DatabaseServiceBase
    {
        private readonly ILogger _logger;
        private readonly IList<DbContext> _dbContexts;
        private readonly IDictionary<Type, IDataService> _dataServices;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="dbContexts">Database contexts.</param>
        protected DatabaseServiceBase(
            ILogger logger,
            params DbContext[] dbContexts)
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
                _logger.LogInformation("Apply migrations: {dbContext}", dbContext.GetType().FullName);
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
                _logger.LogInformation("Delete migrations: {dbContext}", dbContext.GetType().FullName);
                await dbContext.GetService<IMigrator>().MigrateAsync("0");
            }
        }

        /// <summary>
        /// Apply data using data services.
        /// </summary>
        public async Task ApplyDataAsync()
        {
            foreach (var keyValuePair in _dataServices)
            {
                _logger.LogInformation("Apply data: {dataService}", keyValuePair.Value.GetType().FullName);
                await keyValuePair.Value.ApplyDataAsync();
            }
        }

        /// <summary>
        /// Registers data service instance.
        /// </summary>
        /// <param name="dataService">The data service.</param>
        protected void RegisterDataService(IDataService dataService)
        {
            if (dataService == null) throw new ArgumentNullException(nameof(dataService));

            _dataServices[dataService.GetType()] = dataService;
        }
    }
}