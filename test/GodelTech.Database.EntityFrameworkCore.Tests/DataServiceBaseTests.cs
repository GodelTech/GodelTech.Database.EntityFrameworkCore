using GodelTech.Database.EntityFrameworkCore.Tests.Fakes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GodelTech.Database.EntityFrameworkCore.Tests
{
    public class DataServiceBaseTests
    {
        [Fact]
        public void Inherit_IUnitOfWork()
        {
            // Arrange
            var mockDbContext = new Mock<DbContext>(MockBehavior.Strict);
            var mockHostEnvironment = new Mock<IHostEnvironment>(MockBehavior.Strict);
            var mockLogger = new Mock<ILogger>(MockBehavior.Strict);

            // Act
            var jsonDataService = new DataService<FakeItem, int>("", mockDbContext.Object, x => x.Id, mockHostEnvironment.Object, mockLogger.Object);

            // Assert
            Assert.IsAssignableFrom<IDataService>(jsonDataService);
        }
    }
}
