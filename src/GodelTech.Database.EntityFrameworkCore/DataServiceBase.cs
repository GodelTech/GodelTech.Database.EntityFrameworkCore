using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GodelTech.Database.EntityFrameworkCore
{
    /// <summary>
    /// Data service base.
    /// </summary>
    /// <typeparam name="TItem">The type of the T item.</typeparam>
    public abstract class DataServiceBase<TItem> : IDataService
        where TItem : class
    {
        private readonly IConfigurationBuilder _configurationBuilder;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly string _folderPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceBase{TItem}"/> class.
        /// </summary>
        /// <param name="configurationBuilder">The configuration builder.</param>
        /// <param name="hostEnvironment">The host environment.</param>
        /// <param name="folderPath">The folder path.</param>
        /// <param name="logger">The logger.</param>
        protected DataServiceBase(
            IConfigurationBuilder configurationBuilder,
            IHostEnvironment hostEnvironment,
            string folderPath,
            ILogger logger)
        {
            _configurationBuilder = configurationBuilder;
            _hostEnvironment = hostEnvironment;
            _folderPath = folderPath;
            Logger = logger;
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        protected ILogger Logger { get; }

        /// <summary>
        /// Apply data.
        /// </summary>
        public abstract Task ApplyDataAsync();

        /// <summary>
        /// Get data.
        /// </summary>
        /// <returns><cref>IList{TItem}</cref>.</returns>
        protected virtual IList<TItem> GetData()
        {
            Logger.LogInformation("Get configuration: {item}", typeof(TItem).Name);
            var configuration = BuildConfiguration(
                _configurationBuilder,
                _hostEnvironment,
                _folderPath,
                typeof(TItem).Name
            );

            Logger.LogInformation("Get data: {item}", typeof(TItem).Name);
            return configuration.GetSection("Data").Get<IList<TItem>>();
        }

        private static IConfigurationRoot BuildConfiguration(
            IConfigurationBuilder configurationBuilder,
            IHostEnvironment hostEnvironment,
            string folderPath,
            string fileName)
        {
            return configurationBuilder
                .SetBasePath(Path.Combine(hostEnvironment.ContentRootPath, folderPath))
                .AddJsonFile($"{fileName}.json")
                .AddJsonFile($"{fileName}.{hostEnvironment.EnvironmentName}.json", true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}