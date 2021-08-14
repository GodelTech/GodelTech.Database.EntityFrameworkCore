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
        private readonly string _folderPath;
        private readonly IHostEnvironment _hostEnvironment;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceBase{TItem}"/> class.
        /// </summary>
        /// <param name="folderPath">The folder path.</param>
        /// <param name="hostEnvironment">The host environment.</param>
        /// <param name="logger">The logger.</param>
        protected DataServiceBase(string folderPath, IHostEnvironment hostEnvironment, ILogger logger)
        {
            _folderPath = folderPath;
            _hostEnvironment = hostEnvironment;
            Logger = logger;
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        protected readonly ILogger Logger;

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
        protected virtual IList<TItem> GetDataItems()
        {
            Logger.LogInformation("Get configuration: {item}", typeof(TItem).Name);
            var configuration = BuildConfiguration(_hostEnvironment, _folderPath, typeof(TItem).Name);

            Logger.LogInformation("Get data: {item}", typeof(TItem).Name);
            return configuration.GetSection("Data").Get<IList<TItem>>();
        }

        /// <summary>
        /// Apply data.
        /// </summary>
        public abstract Task ApplyDataAsync();
    }
}
