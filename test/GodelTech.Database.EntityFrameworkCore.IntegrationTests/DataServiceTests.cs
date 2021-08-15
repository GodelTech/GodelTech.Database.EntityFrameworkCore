using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GodelTech.Database.EntityFrameworkCore.IntegrationTests.Fakes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace GodelTech.Database.EntityFrameworkCore.IntegrationTests
{
    public sealed class DataServiceTests : IDisposable
    {
        private readonly ConfigurationBuilder _configurationBuilder;
        private readonly HostingEnvironment _hostingEnvironment;
        private readonly FakeDbContext _dbContext;
        private readonly ILogger _logger;

        public DataServiceTests()
        {
            _configurationBuilder = new ConfigurationBuilder();

            _hostingEnvironment = new HostingEnvironment
            {
                ContentRootPath = Directory.GetCurrentDirectory(),
                EnvironmentName = "Development"
            };

            var dbContextOptions = new DbContextOptionsBuilder<FakeDbContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            _dbContext = new FakeDbContext(dbContextOptions);
            _dbContext.Database.OpenConnection();
            _dbContext.Database.EnsureCreated();

            using var loggerFactory = new NullLoggerFactory();
            _logger = new Logger<DataServiceTests>(loggerFactory);
        }

        public void Dispose()
        {
            _dbContext.Database.CloseConnection();
            _dbContext.Dispose();
        }

        public static IEnumerable<object[]> ApplyDataMemberData =>
            new Collection<object[]>
            {
                new object[]
                {
                    new Collection<FakeEntity>(),
                    new Collection<FakeEntity>
                    {
                        new FakeEntity
                        {
                            Id = 1
                        }
                    },
                    new Collection<FakeEntity>
                    {
                        new FakeEntity
                        {
                            Id = 1
                        }
                    }
                }
            };

        [Theory]
        [MemberData(nameof(ApplyDataMemberData))]
        public async Task ApplyDataAsync_Success(
            Collection<FakeEntity> existingEntities,
            Collection<FakeEntity> entities,
            Collection<FakeEntity> expectedEntities)
        {
            // Arrange
            _dbContext
                .FakeEntities
                .AddRange(existingEntities);

            var service = new FakeDataService(
                _configurationBuilder,
                _hostingEnvironment,
                "Test FolderPath",
                _dbContext,
                false,
                x => x.Id,
                _logger
            );

            service.SetData(entities);

            // Act
            await service.ApplyDataAsync();

            // Assert
            Assert.Equal(
                expectedEntities,
                _dbContext.FakeEntities.ToList(),
                new FakeEntityEqualityComparer()
            );
        }
    }
}