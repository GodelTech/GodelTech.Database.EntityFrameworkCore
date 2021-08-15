using System.Threading.Tasks;
using GodelTech.Database.EntityFrameworkCore.IntegrationTests.Fakes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace GodelTech.Database.EntityFrameworkCore.IntegrationTests
{
    public class DatabaseServiceBaseTests
    {
        private readonly FakeDatabaseServiceBase _service;

        public DatabaseServiceBaseTests()
        {
            using var loggerFactory = new NullLoggerFactory();
            var logger = new Logger<DataServiceBaseTests>(loggerFactory);

            var dbContextOptions = new DbContextOptionsBuilder<FakeDbContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            var dbContext = new FakeDbContext(dbContextOptions);

            _service = new FakeDatabaseServiceBase(logger, dbContext);
        }

        [Fact]
        public async Task ApplyMigrationsAsync_Success()
        {
            // Arrange & Act
            await _service.ApplyMigrationsAsync();

            // Assert
            Assert.NotNull(_service);
        }

        [Fact]
        public async Task DeleteMigrationsAsync_Success()
        {
            // Arrange & Act
            await _service.DeleteMigrationsAsync();

            // Assert
            Assert.NotNull(_service);
        }
    }
}