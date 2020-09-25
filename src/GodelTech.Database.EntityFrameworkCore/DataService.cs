using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
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
    public class DataService<TItem, TType> : IDataService
        where TItem : class
    {
        private readonly string _folderPath;
        private readonly DbContext _dbContext;
        private readonly Func<TItem, TType> _propertyToCompare;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger _logger;
        private readonly bool _enableIdentityInsert;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataService{TItem, TType}"/> class.
        /// </summary>
        /// <param name="folderPath">The folder path.</param>
        /// <param name="dbContext">The database context.</param>
        /// <param name="propertyToCompare">The property selector to compare by.</param>
        /// <param name="hostEnvironment">The host environment.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="enableIdentityInsert">Enable IDENTITY_INSERT to insert an explicit value to id property.</param>
        public DataService(
            string folderPath,
            DbContext dbContext,
            Func<TItem, TType> propertyToCompare,
            IHostEnvironment hostEnvironment,
            ILogger logger,
            bool enableIdentityInsert = true)
        {
            _folderPath = folderPath;
            _dbContext = dbContext;
            _propertyToCompare = propertyToCompare;
            _hostEnvironment = hostEnvironment;
            _logger = logger;
            _enableIdentityInsert = enableIdentityInsert;
        }

        private static IConfigurationRoot BuildConfiguration(IHostEnvironment hostEnvironment, string folderPath, string fileName) =>
            new ConfigurationBuilder()
                .SetBasePath(Path.Combine(hostEnvironment.ContentRootPath, folderPath))
                .AddJsonFile($"{fileName}.json")
                .AddJsonFile($"{fileName}.{hostEnvironment.EnvironmentName}.json", true)
                .AddEnvironmentVariables()
                .Build();

        /// <summary>
        /// Get data.
        /// </summary>
        /// <returns><cref>IList{TItem}</cref>.</returns>
        protected IList<TItem> GetDataItems()
        {
            _logger.LogInformation("Get configuration: {item}", typeof(TItem).Name);
            var configuration = BuildConfiguration(_hostEnvironment, _folderPath, typeof(TItem).Name);

            _logger.LogInformation("Get data: {item}", typeof(TItem).Name);
            return configuration.GetSection("Data").Get<IList<TItem>>();
        }

        /// <summary>
        /// Apply data.
        /// </summary>
        public async Task ApplyDataAsync()
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
            if (_enableIdentityInsert)
            {
                var entityType = _dbContext.Model.FindEntityType(typeof(TItem));
                var schema = entityType.GetSchema();
                var tableName = entityType.GetTableName();

                _dbContext.Database.OpenConnection();
                try
                {
                    _dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [" + schema + "].[" + tableName + "] ON;");
                    await _dbContext.SaveChangesAsync();
                    _dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [" + schema + "].[" + tableName + "] OFF;");
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
            }
            _logger.LogInformation("Changes saved successfully");
        }
    }
}
