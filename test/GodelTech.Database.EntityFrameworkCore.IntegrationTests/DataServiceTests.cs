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
                    false,
                    new Collection<FakeEntity>(),
                    new Collection<FakeEntity>
                    {
                        new FakeEntity
                        {
                            Id = 1,
                            Name = "Test Name First"
                        }
                    },
                    new Collection<FakeEntity>
                    {
                        new FakeEntity
                        {
                            Id = 1,
                            Name = "Test Name First"
                        }
                    }
                },
                new object[]
                {
                    false,
                    new Collection<FakeEntity>
                    {
                        new FakeEntity
                        {
                            Id = 1,
                            Name = "Test Name First Old"
                        }
                    },
                    new Collection<FakeEntity>
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
                    },
                    new Collection<FakeEntity>
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
                    }
                }
            };

        [Theory]
        [MemberData(nameof(ApplyDataMemberData))]
        public async Task ApplyDataAsync_WithEnabledIdentityInsert_Success(
            bool enableIdentityInsert,
            Collection<FakeEntity> existingEntities,
            Collection<FakeEntity> entities,
            Collection<FakeEntity> expectedEntities)
        {
            // Arrange
            _dbContext
                .FakeEntities
                .AddRange(existingEntities);

            await _dbContext.SaveChangesAsync();

            _dbContext.ChangeTracker.Clear();

            var service = new FakeDataService(
                _configurationBuilder,
                _hostingEnvironment,
                "Test FolderPath",
                _dbContext,
                enableIdentityInsert,
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