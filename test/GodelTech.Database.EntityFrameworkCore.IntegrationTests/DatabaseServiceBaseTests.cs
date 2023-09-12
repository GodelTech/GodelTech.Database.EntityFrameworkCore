using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GodelTech.Database.EntityFrameworkCore.IntegrationTests.Fakes;
using GodelTech.XUnit.Logging;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace GodelTech.Database.EntityFrameworkCore.IntegrationTests
{
    public sealed class DatabaseServiceBaseTests : IDisposable
    {
        private readonly FakeBaseDbContext _fakeBaseDbContext;
        private readonly FakeDbContext _fakeDbContext;

        private readonly ITestLoggerContextAccessor _testLoggerContextAccessor = new TestLoggerContextAccessor();
        private readonly TestLoggerProvider _testLoggerProvider;

        private readonly SqliteConnection _sqliteConnection;

        private readonly DatabaseServiceBase _service;

        public DatabaseServiceBaseTests(ITestOutputHelper output)
        {
            _testLoggerProvider = new TestLoggerProvider(output, _testLoggerContextAccessor, true);
            var logger = _testLoggerProvider.CreateLogger(nameof(DataServiceTests));

            _sqliteConnection = new SqliteConnection("DataSource=:memory:");
            _sqliteConnection.Open();

            var dbContextOptions = new DbContextOptionsBuilder<DbContext>()
                .UseSqlite(_sqliteConnection)
                .Options;

            _fakeBaseDbContext = new FakeBaseDbContext(dbContextOptions);
            _fakeDbContext = new FakeDbContext(dbContextOptions);

            _service = new FakeDatabaseServiceBase(logger, _fakeBaseDbContext, _fakeDbContext);
        }

        public void Dispose()
        {
            _fakeBaseDbContext.Dispose();
            _fakeDbContext.Dispose();

            _sqliteConnection.Close();
            _sqliteConnection.Dispose();

            _testLoggerProvider.Dispose();
        }

        [Fact]
        public async Task ApplyMigrationsAsync_Success()
        {
            // Arrange
            var cancellationToken = new CancellationToken();

            // Act
            await _service.ApplyMigrationsAsync(cancellationToken);

            // Assert
            var logMessages = _testLoggerContextAccessor.TestLoggerContext.Entries
                .Select(x => x.Message)
                .ToList();

            Assert.Equal($"Apply migrations: {_fakeBaseDbContext.GetType().FullName}", logMessages[0]);
            Assert.Equal($"Apply migrations: {_fakeDbContext.GetType().FullName}", logMessages[1]);

            Assert.Empty(_fakeDbContext.FakeEntities.ToList());
        }

        [Fact]
        public async Task DeleteMigrationsAsync_Success()
        {
            // Arrange
            var cancellationToken = new CancellationToken();

            await _fakeBaseDbContext.Database.MigrateAsync(cancellationToken);
            await _fakeDbContext.Database.MigrateAsync(cancellationToken);

            // Act
            await _service.DeleteMigrationsAsync(cancellationToken);

            // Assert
            var logMessages = _testLoggerContextAccessor.TestLoggerContext.Entries
                .Select(x => x.Message)
                .ToList();

            Assert.Equal($"Delete migrations: {_fakeDbContext.GetType().FullName}", logMessages[0]);
            Assert.Equal($"Delete migrations: {_fakeBaseDbContext.GetType().FullName}", logMessages[1]);

            var exception = Assert.Throws<SqliteException>(
                () => _fakeDbContext.FakeEntities.ToList()
            );
            Assert.Equal(
                "SQLite Error 1: 'no such table: FakeEntity'.",
                exception.Message
            );
        }
    }
}
