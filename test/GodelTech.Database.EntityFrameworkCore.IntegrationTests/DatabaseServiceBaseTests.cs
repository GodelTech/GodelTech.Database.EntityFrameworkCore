using System.Threading;
using System.Threading.Tasks;
using GodelTech.Database.EntityFrameworkCore.IntegrationTests.Fakes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace GodelTech.Database.EntityFrameworkCore.IntegrationTests
{
    public class DatabaseServiceBaseTests
    {
        private readonly FakeDatabaseServiceBase _service;

        public DatabaseServiceBaseTests()
        {
            var logger = new NullLogger<DatabaseServiceBaseTests>();

            var dbContextOptions = new DbContextOptionsBuilder<FakeDbContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            var dbContext = new FakeDbContext(dbContextOptions);

            _service = new FakeDatabaseServiceBase(logger, dbContext);
        }

        [Fact]
        public async Task ApplyMigrationsAsync_Success()
        {
            // Arrange
            var cancellationToken = new CancellationToken();

            // Act
            await _service.ApplyMigrationsAsync(cancellationToken);

            // Assert
            Assert.NotNull(_service);
        }

        [Fact]
        public async Task DeleteMigrationsAsync_Success()
        {
            // Arrange
            var cancellationToken = new CancellationToken();

            // Act
            await _service.DeleteMigrationsAsync(cancellationToken);

            // Assert
            Assert.NotNull(_service);
        }
    }
}
