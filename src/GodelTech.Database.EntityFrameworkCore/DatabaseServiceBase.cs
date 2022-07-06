using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging;

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

        private static readonly Action<ILogger, string, Exception> LogApplyMigrationsAsyncInformationCallback =
            LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(0, nameof(ApplyMigrationsAsync)),
                "Apply migrations: {DbContext}"
            );

        /// <summary>
        /// Apply migrations for provided contexts.
        /// </summary>
        public async Task ApplyMigrationsAsync()
        {
            foreach (var dbContext in _dbContexts)
            {
                LogApplyMigrationsAsyncInformationCallback(
                    _logger,
                    dbContext.GetType().FullName,
                    null
                );

                await dbContext.Database.MigrateAsync();
            }
        }

        private static readonly Action<ILogger, string, Exception> LogDeleteMigrationsAsyncInformationCallback =
            LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(0, nameof(DeleteMigrationsAsync)),
                "Delete migrations: {DbContext}"
            );

        /// <summary>
        /// Delete migrations for provided contexts.
        /// </summary>
        public async Task DeleteMigrationsAsync()
        {
            foreach (var dbContext in _dbContexts.Reverse())
            {
                LogDeleteMigrationsAsyncInformationCallback(
                    _logger,
                    dbContext.GetType().FullName,
                    null
                );

                await dbContext.GetService<IMigrator>().MigrateAsync("0");
            }
        }

        private static readonly Action<ILogger, string, Exception> LogApplyDataAsyncInformationCallback =
            LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(0, nameof(ApplyDataAsync)),
                "Apply data: {DataService}"
            );

        /// <summary>
        /// Apply data using data services.
        /// </summary>
        public async Task ApplyDataAsync()
        {
            foreach (var dataService in _dataServices.Select(x => x.Value))
            {
                LogApplyDataAsyncInformationCallback(
                    _logger,
                    dataService.GetType().FullName,
                    null
                );

                await dataService.ApplyDataAsync();
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
