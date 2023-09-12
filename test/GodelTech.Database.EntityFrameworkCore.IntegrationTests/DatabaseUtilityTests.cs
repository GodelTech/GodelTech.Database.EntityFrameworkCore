using System;
using System.Threading;
using System.Threading.Tasks;
using GodelTech.Database.EntityFrameworkCore.IntegrationTests.Fakes;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GodelTech.Database.EntityFrameworkCore.IntegrationTests
{
    public sealed class DatabaseUtilityTests : IDisposable
    {
        private readonly FakeDbContext _dbContext;

        private readonly DatabaseUtility _databaseUtility = new DatabaseUtility();

        public DatabaseUtilityTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<FakeDbContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            _dbContext = new FakeDbContext(dbContextOptions);
            _dbContext.Database.OpenConnection();
            _dbContext.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _dbContext.Database.CloseConnection();
            _dbContext.Dispose();
        }

        [Fact]
        public async Task OpenConnectionAsync_Success()
        {
            // Arrange
            var cancellationToken = new CancellationToken();

            await _dbContext.Database.CloseConnectionAsync();

            // Act
            await _databaseUtility.OpenConnectionAsync(_dbContext, cancellationToken);
            await _dbContext.Database.EnsureCreatedAsync(cancellationToken);

            // Assert
            var result = await _databaseUtility.ExecuteSqlRawAsync(_dbContext, "SELECT * FROM FakeEntity", cancellationToken);

            Assert.Equal(-1, result);
        }

        [Fact]
        public async Task ExecuteSqlRawAsync_Success()
        {
            // Arrange
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _databaseUtility.ExecuteSqlRawAsync(_dbContext, "SELECT * FROM FakeEntity", cancellationToken);

            // Assert
            Assert.Equal(-1, result);
        }

        [Fact]
        public async Task CloseConnectionAsync_Success()
        {
            // Arrange
            var cancellationToken = new CancellationToken();

            // Act
            await _databaseUtility.CloseConnectionAsync(_dbContext);

            // Assert
            var exception = await Assert.ThrowsAsync<SqliteException>(
                () => _databaseUtility.ExecuteSqlRawAsync(_dbContext, "SELECT * FROM FakeEntity", cancellationToken)
            );
            Assert.Equal(
                "SQLite Error 1: 'no such table: FakeEntity'.",
                exception.Message
            );
        }
    }
}
