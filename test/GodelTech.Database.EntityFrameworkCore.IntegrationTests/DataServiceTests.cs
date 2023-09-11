using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GodelTech.Database.EntityFrameworkCore.IntegrationTests.Fakes;
using GodelTech.XUnit.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace GodelTech.Database.EntityFrameworkCore.IntegrationTests
{
    public sealed class DataServiceTests : IDisposable
    {
        private readonly ConfigurationBuilder _configurationBuilder;
        private readonly HostingEnvironment _hostingEnvironment;
        private readonly FakeDbContext _dbContext;
        private readonly ILogger _logger;

        private readonly ITestLoggerContextAccessor _testLoggerContextAccessor = new TestLoggerContextAccessor();
        private readonly TestLoggerProvider _testLoggerProvider;

        public DataServiceTests(ITestOutputHelper output)
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

            _testLoggerProvider = new TestLoggerProvider(output, _testLoggerContextAccessor, true);
            _logger = _testLoggerProvider.CreateLogger(nameof(DataServiceTests));
        }

        public void Dispose()
        {
            _dbContext.Database.CloseConnection();
            _dbContext.Dispose();

            _testLoggerProvider.Dispose();
        }

        [Fact]
        public async Task ApplyDataAsync_Update_Success()
        {
            // Arrange
            var cancellationToken = new CancellationToken();

            var existingEntities = new Collection<FakeEntity>
            {
                new FakeEntity
                {
                    Id = 1,
                    Name = "Test Name First"
                },
                new FakeEntity
                {
                    Id = 2,
                    Name = "Test Name Second"
                }
            };

            _dbContext
                .FakeEntities
                .AddRange(existingEntities);

            await _dbContext.SaveChangesAsync(cancellationToken);

            _dbContext.ChangeTracker.Clear();

            var entities = new Collection<FakeEntity>
            {
                new FakeEntity
                {
                    Id = 1,
                    Name = "Test Name First New"
                }
            };

            var service = new FakeDataService(
                _configurationBuilder,
                _hostingEnvironment,
                "Test FolderPath",
                _dbContext,
                false,
                x => x.Id,
                _logger,
                new DatabaseUtility()
            );

            service.SetData(entities);

            var expectedEntities = new Collection<FakeEntity>
            {
                new FakeEntity
                {
                    Id = 1,
                    Name = "Test Name First New"
                },
                new FakeEntity
                {
                    Id = 2,
                    Name = "Test Name Second"
                }
            };

            // Act
            await service.ApplyDataAsync(cancellationToken);

            // Assert
            _dbContext.FakeEntities.ToList().Should().BeEquivalentTo(expectedEntities);

            var logMessages = _testLoggerContextAccessor.TestLoggerContext.Entries
                .Select(x => x.Message)
                .ToList();

            Assert.Equal($"Update entity: {entities[0].Id}", logMessages[0]);
            Assert.Equal("Saving changes...", logMessages[1]);
            Assert.Equal("Changes saved successfully", logMessages[2]);
        }

        [Fact]
        public async Task ApplyDataAsync_Add_Success()
        {
            // Arrange
            var cancellationToken = new CancellationToken();

            var entities = new Collection<FakeEntity>
            {
                new FakeEntity
                {
                    Id = 1,
                    Name = "Test Name First"
                },
                new FakeEntity
                {
                    Id = 2,
                    Name = "Test Name Second"
                }
            };

            var service = new FakeDataService(
                _configurationBuilder,
                _hostingEnvironment,
                "Test FolderPath",
                _dbContext,
                false,
                x => x.Id,
                _logger,
                new DatabaseUtility()
            );

            service.SetData(entities);

            var expectedEntities = new Collection<FakeEntity>
            {
                new FakeEntity
                {
                    Id = 1,
                    Name = "Test Name First"
                },
                new FakeEntity
                {
                    Id = 2,
                    Name = "Test Name Second"
                }
            };

            // Act
            await service.ApplyDataAsync(cancellationToken);

            // Assert
            _dbContext.FakeEntities.ToList().Should().BeEquivalentTo(expectedEntities);

            var logMessages = _testLoggerContextAccessor.TestLoggerContext.Entries
                .Select(x => x.Message)
                .ToList();

            Assert.Equal($"Add entity: {entities[0].Id}", logMessages[0]);
            Assert.Equal($"Add entity: {entities[1].Id}", logMessages[1]);
            Assert.Equal("Saving changes...", logMessages[2]);
            Assert.Equal("Changes saved successfully", logMessages[3]);
        }

        [Fact]
        public async Task ApplyDataAsync_WithIdentity_Success()
        {
            // Arrange
            var cancellationToken = new CancellationToken();

            var existingEntities = new Collection<FakeEntity>
            {
                new FakeEntity
                {
                    Id = 1,
                    Name = "Test Name First"
                }
            };

            _dbContext
                .FakeEntities
                .AddRange(existingEntities);

            await _dbContext.SaveChangesAsync(cancellationToken);

            _dbContext.ChangeTracker.Clear();

            var entities = new Collection<FakeEntity>
            {
                new FakeEntity
                {
                    Id = 1,
                    Name = "Test Name First New"
                },
                new FakeEntity
                {
                    Id = 2,
                    Name = "Test Name Second"
                }
            };

            var databaseUtility = new FakeDatabaseUtility();

            var service = new FakeDataService(
                _configurationBuilder,
                _hostingEnvironment,
                "Test FolderPath",
                _dbContext,
                true,
                x => x.Id,
                _logger,
                databaseUtility
            );

            service.SetData(entities);

            var expectedEntities = new Collection<FakeEntity>
            {
                new FakeEntity
                {
                    Id = 1,
                    Name = "Test Name First New"
                },
                new FakeEntity
                {
                    Id = 2,
                    Name = "Test Name Second"
                }
            };

            // Act
            await service.ApplyDataAsync(cancellationToken);

            // Assert
            _dbContext.FakeEntities.ToList().Should().BeEquivalentTo(expectedEntities);

            var logMessages = _testLoggerContextAccessor.TestLoggerContext.Entries
                .Select(x => x.Message)
                .ToList();

            Assert.Equal($"Update entity: {entities[0].Id}", logMessages[0]);
            Assert.Equal($"Add entity: {entities[1].Id}", logMessages[1]);
            Assert.Equal("Saving changes...", logMessages[2]);
            Assert.Equal("Changes saved successfully", logMessages[3]);

            Assert.True(databaseUtility.WasOpened);

            var schema = "FakeSchema";
            var tableName = "FakeEntity";

            Assert.Equal($"SET IDENTITY_INSERT [{schema}].[{tableName}] ON;", databaseUtility.SqlStrings[0]);
            Assert.Equal($"SET IDENTITY_INSERT [{schema}].[{tableName}] OFF;", databaseUtility.SqlStrings[1]);

            Assert.True(databaseUtility.WasClosed);
        }
    }
}
