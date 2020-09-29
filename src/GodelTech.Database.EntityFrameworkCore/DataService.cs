using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GodelTech.Database.EntityFrameworkCore
{
    /// <summary>
    /// Data service.
    /// </summary>
    /// <typeparam name="TItem">The type of the T item.</typeparam>
    /// <typeparam name="TType">The type of the T type.</typeparam>
    public class DataService<TItem, TType> : DataServiceBase<TItem, TType>
        where TItem : class
    {
        private readonly DbContext _dbContext;
        private readonly Func<TItem, TType> _propertyToCompare;
        private readonly ILogger _logger;
        private readonly bool _enableIdentityInsert;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataService{TItem, TType}"/> class.
        /// </summary>
        /// <param name="folderPath">The folder path.</param>
        /// <param name="dbContext">The database context.</param>
        /// <param name="enableIdentityInsert">Enable IDENTITY_INSERT to insert an explicit value to id property.</param>
        /// <param name="propertyToCompare">The property selector to compare by.</param>
        /// <param name="hostEnvironment">The host environment.</param>
        /// <param name="logger">The logger.</param>
        public DataService(
            string folderPath,
            DbContext dbContext,
            bool enableIdentityInsert,
            Func<TItem, TType> propertyToCompare,
            IHostEnvironment hostEnvironment,
            ILogger logger)
            : base(folderPath, hostEnvironment, logger)
        {
            _dbContext = dbContext;
            _propertyToCompare = propertyToCompare;
            _logger = logger;
            _enableIdentityInsert = enableIdentityInsert;
        }

        /// <summary>
        /// Apply data.
        /// </summary>
        public override async Task ApplyDataAsync()
        {
            _logger.LogInformation("Apply data: {item}", typeof(TItem).Name);

            var items = GetDataItems();

            if (items == null)
            {
                _logger.LogWarning("Empty data: {item}", typeof(TItem).Name);
                return;
            }

            foreach (var item in items)
            {
                Expression<Func<TItem, bool>> predicate = x => _propertyToCompare(x).Equals(_propertyToCompare(item));

                if (_dbContext.Set<TItem>().AsNoTracking().Any(predicate.Compile()))
                {
                    _logger.LogInformation("Update item: {property}", _propertyToCompare(item));
                    _dbContext.Set<TItem>().Update(item);
                }
                else
                {
                    _logger.LogInformation("Add item: {property}", _propertyToCompare(item));
                    await _dbContext.Set<TItem>().AddAsync(item);
                }
            }

            _logger.LogInformation("Saving changes...");

            var entityType = _dbContext.Model.FindEntityType(typeof(TItem));
            var schema = entityType.GetSchema();
            var tableName = entityType.GetTableName();

            if (_enableIdentityInsert)
            {
                _dbContext.Database.OpenConnection();
                try
                {
                    _dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [" + schema + "].[" + tableName + "] ON;");
                    await _dbContext.SaveChangesAsync();
                    _dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [" + schema + "].[" + tableName + "] OFF;");

                    _logger.LogInformation("Changes saved successfully");
                }
                catch
                {
                    _logger.LogError("Error on save changes");
                }
                finally
                {
                    _dbContext.Database.CloseConnection();
                }
            }
            else
            {
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Changes saved successfully");
            }
        }
    }
}
