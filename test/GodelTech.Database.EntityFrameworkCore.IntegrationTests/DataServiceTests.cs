using System;
using System.Collections.Generic;
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

        public static IEnumerable<object[]> ApplyDataMemberData =>
            new Collection<object[]>
            {
                new object[]
                {
                    true,
                    new Collection<FakeEntity>(),
                    null,
                    new Collection<FakeEntity>()
                },
                new object[]
                {
                    true,
                    new Collection<FakeEntity>(),
                    new Collection<FakeEntity>(),
                    new Collection<FakeEntity>()
                },
                new object[]
                {
                    true,
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
                    true,
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
                },
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
        public async Task ApplyDataAsync_Success(
            bool enableIdentityInsert,
            Collection<FakeEntity> existingEntities,
            Collection<FakeEntity> entities,
            Collection<FakeEntity> expectedEntities)
        {
            // Arrange
            var cancellationToken = new CancellationToken();

            _dbContext
                .FakeEntities
                .AddRange(existingEntities);

            await _dbContext.SaveChangesAsync(cancellationToken);

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
            await service.ApplyDataAsync(cancellationToken);

            // Assert
            _dbContext.FakeEntities.ToList().Should().BeEquivalentTo(expectedEntities);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ExecuteSqlRawAsync_Success(bool enableIdentityInsert)
        {
            // Arrange
            var cancellationToken = new CancellationToken();

            var service = new FakeDataService(
                _configurationBuilder,
                _hostingEnvironment,
                "Test FolderPath",
                _dbContext,
                enableIdentityInsert,
                x => x.Id,
                _logger
            );

            // Act
            await service.ExposedExecuteSqlRawAsync("SELECT * FROM FakeEntity", cancellationToken);

            // Assert
            Assert.NotNull(service);
        }
    }
}
