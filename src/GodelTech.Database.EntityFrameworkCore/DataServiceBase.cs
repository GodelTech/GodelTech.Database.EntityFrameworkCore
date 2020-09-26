using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GodelTech.Database.EntityFrameworkCore
{
    public abstract class DataServiceBase<TItem, TType> : IDataService
        where TItem : class
    {
        private readonly string _folderPath;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceBase{TItem, TType}"/> class.
        /// </summary>
        /// <param name="folderPath">The folder path.</param>
        /// <param name="hostEnvironment">The host environment.</param>
        /// <param name="logger">The logger.</param>
        public DataServiceBase(string folderPath, IHostEnvironment hostEnvironment, ILogger logger)
        {
            _folderPath = folderPath;
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
        public abstract Task ApplyDataAsync();
    }
}
