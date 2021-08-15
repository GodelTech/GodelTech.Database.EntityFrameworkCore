using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GodelTech.Database.EntityFrameworkCore.Tests.Fakes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GodelTech.Database.EntityFrameworkCore.Tests
{
    public class DataServiceTests
    {
        private const string FolderPath = "Fakes/Json";

        private readonly Mock<IConfigurationBuilder> _mockConfigurationBuilder;
        private readonly Mock<IHostEnvironment> _mockHostEnvironment;
        private readonly Mock<DbContext> _mockDbContext;
        private readonly Mock<ILogger> _mockLogger;

        public DataServiceTests()
        {
            _mockConfigurationBuilder = new Mock<IConfigurationBuilder>(MockBehavior.Strict);
            _mockHostEnvironment = new Mock<IHostEnvironment>(MockBehavior.Strict);
            _mockDbContext = new Mock<DbContext>(MockBehavior.Strict);
            _mockLogger = new Mock<ILogger>(MockBehavior.Strict);
        }

        public static IEnumerable<object[]> ItemsIsNullOrEmptyMemberData =>
            new Collection<object[]>
            {
                new object[] { true, null },
                new object[] { false, null },
                new object[] { true, new List<FakeItem>() },
                new object[] { false, new List<FakeItem>() }
            };

        [Theory]
        [MemberData(nameof(ItemsIsNullOrEmptyMemberData))]
        public async Task ApplyDataAsync_WhenItemsIsNullOrEmpty(
            bool enableIdentityInsert,
            IList<FakeItem> items)
        {
            // Arrange
            var service = new FakeDataService(
                _mockConfigurationBuilder.Object,
                _mockHostEnvironment.Object,
                FolderPath,
                _mockDbContext.Object,
                enableIdentityInsert,
                x => x.Id,
                _mockLogger.Object,
                items
            );

            Expression<Action<ILogger>> loggerExpression = x => x.Log(
                LogLevel.Warning,
                0,
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString() ==
                    $"Empty data: {nameof(FakeItem)}"
                ),
                null,
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)
            );
            _mockLogger.Setup(loggerExpression);

            // Act
            await service.ApplyDataAsync();

            // Assert
            _mockLogger.Verify(loggerExpression, Times.Once);
        }
    }
}