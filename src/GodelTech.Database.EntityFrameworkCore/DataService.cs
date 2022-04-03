using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace GodelTech.Database.EntityFrameworkCore
{
    /// <summary>
    /// Data service.
    /// </summary>
    /// <typeparam name="TEntity">The type of the T entity.</typeparam>
    /// <typeparam name="TType">The type of the T type.</typeparam>
    public class DataService<TEntity, TType> : DataServiceBase<TEntity>
        where TEntity : class
    {
        private readonly DbContext _dbContext;
        private readonly Func<TEntity, TType> _propertyToCompare;
        private readonly bool _enableIdentityInsert;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataService{TEntity, TType}"/> class.
        /// </summary>
        /// <param name="configurationBuilder">The configuration builder.</param>
        /// <param name="hostEnvironment">The host environment.</param>
        /// <param name="folderPath">The folder path.</param>
        /// <param name="dbContext">The database context.</param>
        /// <param name="enableIdentityInsert">Enable IDENTITY_INSERT to insert an explicit value to id property.</param>
        /// <param name="propertyToCompare">The property selector to compare by.</param>
        /// <param name="logger">The logger.</param>
        public DataService(
            IConfigurationBuilder configurationBuilder,
            IHostEnvironment hostEnvironment,
            string folderPath,
            DbContext dbContext,
            bool enableIdentityInsert,
            Func<TEntity, TType> propertyToCompare,
            ILogger logger)
            : base(
                configurationBuilder,
                hostEnvironment,
                folderPath,
                logger)
        {
            _dbContext = dbContext;
            _propertyToCompare = propertyToCompare;
            _enableIdentityInsert = enableIdentityInsert;
        }

        private readonly Action<ILogger, string, Exception> _logApplyDataAsyncEmptyDataWarningCallback =
            LoggerMessage.Define<string>(
                LogLevel.Warning,
                new EventId(0, nameof(ApplyDataAsync)),
                "Empty data: {Entity}"
            );

        private static readonly Action<ILogger, TType, Exception> LogApplyDataAsyncUpdateEntityInformationCallback =
            LoggerMessage.Define<TType>(
                LogLevel.Information,
                new EventId(0, nameof(ApplyDataAsync)),
                "Update entity: {Property}"
            );

        private static readonly Action<ILogger, TType, Exception> LogApplyDataAsyncAddEntityInformationCallback =
            LoggerMessage.Define<TType>(
                LogLevel.Information,
                new EventId(0, nameof(ApplyDataAsync)),
                "Add entity: {Property}"
            );

        private readonly Action<ILogger, Exception> _logApplyDataAsyncSavingChangesInformationCallback =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(0, nameof(ApplyDataAsync)),
                "Saving changes..."
            );

        private readonly Action<ILogger, Exception> _logApplyDataAsyncChangesSavedInformationCallback =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(0, nameof(ApplyDataAsync)),
                "Changes saved successfully"
            );

        /// <summary>
        /// Apply data.
        /// </summary>
        public override async Task ApplyDataAsync()
        {
            var entities = GetData();

            if (entities == null || !entities.Any())
            {
                if (Logger.IsEnabled(LogLevel.Warning))
                {
                    _logApplyDataAsyncEmptyDataWarningCallback(
                        Logger,
                        typeof(TEntity).Name,
                        null
                    );
                }

                return;
            }

            foreach (var entity in entities)
            {
                Expression<Func<TEntity, bool>> predicate = x => _propertyToCompare(x).Equals(_propertyToCompare(entity));

                if (_dbContext.Set<TEntity>().AsNoTracking().Any(predicate.Compile()))
                {
                    if (Logger.IsEnabled(LogLevel.Information))
                    {
                        LogApplyDataAsyncUpdateEntityInformationCallback(
                            Logger,
                            _propertyToCompare(entity),
                            null
                        );
                    }

                    _dbContext.Set<TEntity>().Update(entity);
                }
                else
                {
                    if (Logger.IsEnabled(LogLevel.Information))
                    {
                        LogApplyDataAsyncAddEntityInformationCallback(
                            Logger,
                            _propertyToCompare(entity),
                            null
                        );
                    }

                    await _dbContext.Set<TEntity>().AddAsync(entity);
                }
            }

            if (Logger.IsEnabled(LogLevel.Information))
            {
                _logApplyDataAsyncSavingChangesInformationCallback(
                    Logger,
                    null
                );
            }

            if (_enableIdentityInsert)
            {
                var entityType = _dbContext.Model.FindEntityType(typeof(TEntity));
                var schema = entityType.GetSchema();
                var tableName = entityType.GetTableName();

                await ExecuteSqlRawAsync("SET IDENTITY_INSERT [" + schema + "].[" + tableName + "] ON;");
                await _dbContext.SaveChangesAsync();
                await ExecuteSqlRawAsync("SET IDENTITY_INSERT [" + schema + "].[" + tableName + "] OFF;");

                if (Logger.IsEnabled(LogLevel.Information))
                {
                    _logApplyDataAsyncChangesSavedInformationCallback(
                        Logger,
                        null
                    );
                }
            }
            else
            {
                await _dbContext.SaveChangesAsync();

                if (Logger.IsEnabled(LogLevel.Information))
                {
                    _logApplyDataAsyncChangesSavedInformationCallback(
                        Logger,
                        null
                    );
                }
            }
        }

        /// <summary>
        /// Executes SQL.
        /// </summary>
        /// <param name="sql">SQL string.</param>
        protected virtual async Task ExecuteSqlRawAsync(string sql)
        {
            await _dbContext.Database.ExecuteSqlRawAsync(sql);
        }
    }
}