using System;
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
    /// <typeparam name="TEntity">The type of the T entity.</typeparam>
    public abstract class DataServiceBase<TEntity> : IDataService
        where TEntity : class
    {
        private readonly IConfigurationBuilder _configurationBuilder;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly string _folderPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceBase{TEntity}"/> class.
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

        private readonly Action<ILogger, string, Exception> _logGetDataGetConfigurationInformationCallback =
            LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(0, nameof(GetData)),
                "Get configuration: {Entity}"
            );

        private readonly Action<ILogger, string, Exception> _logGetDataGetDataInformationCallback =
            LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(0, nameof(GetData)),
                "Get data: {Entity}"
            );

        /// <summary>
        /// Get data.
        /// </summary>
        /// <returns><cref>IList{TEntity}</cref>.</returns>
        protected virtual IList<TEntity> GetData()
        {
            if (Logger.IsEnabled(LogLevel.Information))
            {
                _logGetDataGetConfigurationInformationCallback(
                    Logger,
                    typeof(TEntity).Name,
                    null
                );
            }

            var configuration = BuildConfiguration(
                _configurationBuilder,
                _hostEnvironment,
                _folderPath,
                typeof(TEntity).Name
            );

            if (Logger.IsEnabled(LogLevel.Information))
            {
                _logGetDataGetDataInformationCallback(
                    Logger,
                    typeof(TEntity).Name,
                    null
                );
            }

            return configuration
                .GetSection("Data")
                .Get<IList<TEntity>>();
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