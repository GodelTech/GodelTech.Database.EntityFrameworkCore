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

        /// <summary>
        /// Apply data.
        /// </summary>
        public override async Task ApplyDataAsync()
        {
            var entities = GetData();

            if (entities == null || !entities.Any())
            {
                Logger.LogWarning("Empty data: {entity}", typeof(TEntity).Name);
                return;
            }

            foreach (var entity in entities)
            {
                Expression<Func<TEntity, bool>> predicate = x => _propertyToCompare(x).Equals(_propertyToCompare(entity));

                if (_dbContext.Set<TEntity>().AsNoTracking().Any(predicate.Compile()))
                {
                    Logger.LogInformation("Update entity: {property}", _propertyToCompare(entity));
                    _dbContext.Set<TEntity>().Update(entity);
                }
                else
                {
                    Logger.LogInformation("Add entity: {property}", _propertyToCompare(entity));
                    await _dbContext.Set<TEntity>().AddAsync(entity);
                }
            }

            Logger.LogInformation("Saving changes...");

            if (_enableIdentityInsert)
            {
                var entityType = _dbContext.Model.FindEntityType(typeof(TEntity));
                var schema = entityType.GetSchema();
                var tableName = entityType.GetTableName();

                await ExecuteSqlRawAsync("SET IDENTITY_INSERT [" + schema + "].[" + tableName + "] ON;");
                await _dbContext.SaveChangesAsync();
                await ExecuteSqlRawAsync("SET IDENTITY_INSERT [" + schema + "].[" + tableName + "] OFF;");

                Logger.LogInformation("Changes saved successfully");
            }
            else
            {
                await _dbContext.SaveChangesAsync();

                Logger.LogInformation("Changes saved successfully");
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