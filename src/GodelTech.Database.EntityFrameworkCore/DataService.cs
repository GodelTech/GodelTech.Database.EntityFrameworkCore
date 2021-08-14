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
    /// <typeparam name="TItem">The type of the T item.</typeparam>
    /// <typeparam name="TType">The type of the T type.</typeparam>
    public class DataService<TItem, TType> : DataServiceBase<TItem>
        where TItem : class
    {
        private readonly DbContext _dbContext;
        private readonly Func<TItem, TType> _propertyToCompare;
        private readonly bool _enableIdentityInsert;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataService{TItem, TType}"/> class.
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
            Func<TItem, TType> propertyToCompare,
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
            var items = GetData();

            if (items == null)
            {
                Logger.LogWarning("Empty data: {item}", typeof(TItem).Name);
                return;
            }

            foreach (var item in items)
            {
                Expression<Func<TItem, bool>> predicate = x => _propertyToCompare(x).Equals(_propertyToCompare(item));

                if (_dbContext.Set<TItem>().AsNoTracking().Any(predicate.Compile()))
                {
                    Logger.LogInformation("Update item: {property}", _propertyToCompare(item));
                    _dbContext.Set<TItem>().Update(item);
                }
                else
                {
                    Logger.LogInformation("Add item: {property}", _propertyToCompare(item));
                    await _dbContext.Set<TItem>().AddAsync(item);
                }
            }

            Logger.LogInformation("Saving changes...");

            var entityType = _dbContext.Model.FindEntityType(typeof(TItem));
            var schema = entityType.GetSchema();
            var tableName = entityType.GetTableName();

            if (_enableIdentityInsert)
            {
                await _dbContext.Database.OpenConnectionAsync();
                try
                {
                    await _dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [" + schema + "].[" + tableName + "] ON;");
                    await _dbContext.SaveChangesAsync();
                    await _dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [" + schema + "].[" + tableName + "] OFF;");

                    Logger.LogInformation("Changes saved successfully");
                }
                finally
                {
                    await _dbContext.Database.CloseConnectionAsync();
                }
            }
            else
            {
                await _dbContext.SaveChangesAsync();

                Logger.LogInformation("Changes saved successfully");
            }
        }
    }
}