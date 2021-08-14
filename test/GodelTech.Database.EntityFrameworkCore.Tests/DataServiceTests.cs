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
        [Fact]
        public void Inherit_IUnitOfWork()
        {
            // Arrange
            var mockConfigurationBuilder = new Mock<IConfigurationBuilder>(MockBehavior.Strict);
            var mockHostEnvironment = new Mock<IHostEnvironment>(MockBehavior.Strict);
            var mockDbContext = new Mock<DbContext>(MockBehavior.Strict);
            var mockLogger = new Mock<ILogger>(MockBehavior.Strict);

            // Act
            var jsonDataService = new DataService<FakeItem, int>(
                mockConfigurationBuilder.Object,
                mockHostEnvironment.Object,
                "",
                mockDbContext.Object,
                true,
                x => x.Id,
                mockLogger.Object
            );

            // Assert
            Assert.IsAssignableFrom<IDataService>(jsonDataService);
        }
    }
}
