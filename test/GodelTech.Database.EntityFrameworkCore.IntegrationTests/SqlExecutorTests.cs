using System;
using System.Threading;
using System.Threading.Tasks;
using GodelTech.Database.EntityFrameworkCore.IntegrationTests.Fakes;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GodelTech.Database.EntityFrameworkCore.IntegrationTests
{
    public sealed class SqlExecutorTests : IDisposable
    {
        private readonly FakeDbContext _dbContext;

        private readonly SqlExecutor _executor = new SqlExecutor();

        public SqlExecutorTests()
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
        public async Task ExecuteSqlRawAsync_Success()
        {
            // Arrange
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _executor.ExecuteSqlRawAsync(_dbContext, "SELECT * FROM FakeEntity", cancellationToken);

            // Assert
            Assert.Equal(-1, result);
        }
    }
}
