using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using GodelTech.Database.EntityFrameworkCore.Tests.Fakes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GodelTech.Database.EntityFrameworkCore.Tests
{
    public class DataServiceBaseTests
    {
        private Mock<IConfigurationBuilder> _mockConfigurationBuilder;
        private Mock<IHostEnvironment> _mockHostEnvironment;
        private const string FolderPath = "Fakes/Json";
        private Mock<ILogger> _mockLogger;

        private readonly FakeDataServiceBase _service;

        public DataServiceBaseTests()
        {
            _mockConfigurationBuilder = new Mock<IConfigurationBuilder>(MockBehavior.Strict);
            _mockHostEnvironment = new Mock<IHostEnvironment>(MockBehavior.Strict);
            _mockLogger = new Mock<ILogger>(MockBehavior.Strict);

            _service = new FakeDataServiceBase(
                _mockConfigurationBuilder.Object,
                _mockHostEnvironment.Object,
                FolderPath,
                _mockLogger.Object
            );
        }

        [Fact]
        public void GetDataItems_Success()
        {
            // Arrange
            const string environmentName = "Development";

            var expectedResult = new List<FakeItem>();

            Expression<Action<ILogger>> loggerExpressionConfiguration = x => x.Log(
                LogLevel.Information,
                0,
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString() ==
                    $"Get configuration: {nameof(FakeItem)}"
                ),
                null,
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)
            );
            _mockLogger.Setup(loggerExpressionConfiguration);

            _mockHostEnvironment
                .Setup(x => x.ContentRootPath)
                .Returns(Directory.GetCurrentDirectory());

            _mockHostEnvironment
                .Setup(x => x.EnvironmentName)
                .Returns(environmentName);

            Expression<Action<ILogger>> loggerExpressionDate = x => x.Log(
                LogLevel.Information,
                0,
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString() ==
                    $"Get data: {nameof(FakeItem)}"
                ),
                null,
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)
            );
            _mockLogger.Setup(loggerExpressionDate);

            // Act
            var result = _service.ExposedGetData();

            // Assert
            _mockLogger.Verify(loggerExpressionConfiguration, Times.Once);

            _mockHostEnvironment
                .Verify(
                    x => x.ContentRootPath,
                    Times.Once
                );

            _mockHostEnvironment
                .Verify(
                    x => x.EnvironmentName,
                    Times.Once
                );

            _mockLogger.Verify(loggerExpressionDate, Times.Once);

            Assert.Equal(expectedResult, result);
        }
    }
}