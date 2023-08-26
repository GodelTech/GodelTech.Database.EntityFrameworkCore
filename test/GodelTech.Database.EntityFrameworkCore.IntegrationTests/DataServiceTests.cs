﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
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

            _logger = new NullLogger<DatabaseServiceBaseTests>();
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

#if NETCOREAPP3_1
            var notDetachedEntityEntries = _dbContext.ChangeTracker.Entries()
                .Where(x => x.State != EntityState.Detached)
                .ToList();

            foreach (var entityEntry in notDetachedEntityEntries)
            {
                entityEntry.State = EntityState.Detached;
            }
#else
            _dbContext.ChangeTracker.Clear();
#endif

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
