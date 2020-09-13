using GodelTech.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GodelTech.Database.EntityFrameworkCore
{
    /// <summary>
    /// JSON data service.
    /// </summary>
    /// <typeparam name="TEntity">The type of the T entity.</typeparam>
    /// <typeparam name="TType">The type of the T type.</typeparam>
    public class JsonDataService<TEntity, TType> : IDataService
            where TEntity : class, IEntity<TType>
    {
        private readonly string _folderPath;
        private readonly DbContext _dbContext;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDataService{TEntity, TType}"/> class.
        /// </summary>
        /// <param name="folderPath">The folder path.</param>
        /// <param name="dbContext">The database context.</param>
        /// <param name="hostEnvironment">The host environment.</param>
        /// <param name="logger">The logger.</param>
        public JsonDataService(string folderPath, DbContext dbContext, IHostEnvironment hostEnvironment, ILogger logger)
        {
            _folderPath = folderPath;
            _dbContext = dbContext;
            _hostEnvironment = hostEnvironment;
            _logger = logger;
        }

        private static IConfigurationRoot BuildConfiguration(IHostEnvironment hostEnvironment, string folderPath, string fileName) =>
            new ConfigurationBuilder()
                .SetBasePath(Path.Combine(hostEnvironment.ContentRootPath, folderPath))
                .AddJsonFile($"{fileName}.json")
                .AddJsonFile($"{fileName}.{hostEnvironment.EnvironmentName}.json", true)
                .AddEnvironmentVariables()
                .Build();

        private IList<TEntity> GetDataItems()
        {
            var configuration = BuildConfiguration(_hostEnvironment, _folderPath, typeof(TEntity).Name);

            return configuration.GetSection("Data").Get<IList<TEntity>>();
        }

        /// <summary>
        /// Apply data.
        /// </summary>
        public async Task ApplyDataAsync()
        {
            _logger.LogInformation("Apply data: {entity}", typeof(TEntity).Name);
            foreach (var item in GetDataItems())
            {
                if (_dbContext.Set<TEntity>().Any(x => x.Id.Equals(item.Id)))
                {
                    _logger.LogInformation("Update entity: {id}", item.Id);
                    _dbContext.Set<TEntity>().Update(item);
                }
                else
                {
                    _logger.LogInformation("Add entity: {id}", item.Id);
                    await _dbContext.Set<TEntity>().AddAsync(item);
                }
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
