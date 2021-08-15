using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GodelTech.Database.EntityFrameworkCore.Tests.Fakes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GodelTech.Database.EntityFrameworkCore.Tests
{
    public class DatabaseServiceBaseTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<DbContext> _mockDbContext;

        private readonly FakeDatabaseServiceBase _service;

        public DatabaseServiceBaseTests()
        {
            _mockLogger = new Mock<ILogger>(MockBehavior.Strict);
            _mockDbContext = new Mock<DbContext>(MockBehavior.Strict);

            _service = new FakeDatabaseServiceBase(
                _mockLogger.Object,
                _mockDbContext.Object
            );
        }

        [Fact]
        public async Task ApplyDataAsync_Success()
        {
            // Arrange
            var mockDataService = new Mock<IDataService>();

            _service.ExposedRegisterDataService(mockDataService.Object);

            Expression<Action<ILogger>> loggerExpression = x => x.Log(
                LogLevel.Information,
                0,
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString() ==
                    $"Apply data: {mockDataService.Object.GetType().FullName}"
                ),
                null,
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)
            );
            _mockLogger.Setup(loggerExpression);

            mockDataService
                .Setup(x => x.ApplyDataAsync());

            // Act
            await _service.ApplyDataAsync();

            // Assert
            _mockLogger.Verify(loggerExpression, Times.Once);

            mockDataService
                .Verify(
                    x => x.ApplyDataAsync(),
                    Times.Once
                );
        }

        [Fact]
        public void RegisterDataService_WhenDataServiceIsNull_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(
                () => _service.ExposedRegisterDataService(null)
            );
            Assert.Equal("dataService", exception.ParamName);
        }

        [Fact]
        public void RegisterDataService_Success()
        {
            // Arrange
            var mockDataService = new Mock<IDataService>();

            // Act & Assert
            _service.ExposedRegisterDataService(mockDataService.Object);
        }
    }
}