using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using GodelTech.Database.EntityFrameworkCore.Tests.Fakes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GodelTech.Database.EntityFrameworkCore.Tests
{
    public class DataServiceBaseTests
    {
        private const string FolderPath = "Fakes/Json";

        private readonly Mock<IConfigurationBuilder> _mockConfigurationBuilder;
        private readonly Mock<IHostEnvironment> _mockHostEnvironment;
        private readonly Mock<ILogger> _mockLogger;

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
        public void GetData_Success()
        {
            // Arrange
            const string environmentName = "Development";

            var expectedResult = new List<FakeEntity>
            {
                new FakeEntity
                {
                    Id = 1,
                    Name = "Test Name First"
                },
                new FakeEntity
                {
                    Id = 2,
                    Name = "Test Name Second"
                },
                new FakeEntity
                {
                    Id = 3,
                    Name = "Test Name Third"
                }
            };

            Expression<Action<ILogger>> loggerExpressionConfiguration = x => x.Log(
                LogLevel.Information,
                0,
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString() ==
                    $"Get configuration: {nameof(FakeEntity)}"
                ),
                null,
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)
            );
            _mockLogger.Setup(loggerExpressionConfiguration);

            _mockHostEnvironment
                .Setup(x => x.ContentRootPath)
                .Returns(Directory.GetCurrentDirectory());

            _mockConfigurationBuilder
                .Setup(x => x.Properties)
                .Returns(new Dictionary<string, object>());

            _mockConfigurationBuilder
                .Setup(
                    x => x.Add(
                        It.Is<JsonConfigurationSource>(
                            y => y.Path == $"{nameof(FakeEntity)}.json"
                                 && y.Optional == false
                        )
                    )
                )
                .Returns(_mockConfigurationBuilder.Object);

            _mockHostEnvironment
                .Setup(x => x.EnvironmentName)
                .Returns(environmentName);

            _mockConfigurationBuilder
                .Setup(
                    x => x.Add(
                        It.Is<JsonConfigurationSource>(
                            y => y.Path == $"{nameof(FakeEntity)}.{environmentName}.json"
                                 && y.Optional
                        )
                    )
                )
                .Returns(_mockConfigurationBuilder.Object);

            _mockConfigurationBuilder
                .Setup(
                    x => x.Add(
                        It.Is<EnvironmentVariablesConfigurationSource>(
                            y => string.IsNullOrEmpty(y.Prefix)
                        )
                    )
                )
                .Returns(_mockConfigurationBuilder.Object);

            var configurationRoot = new ConfigurationBuilder()
                .AddJsonFile($"{FolderPath}/{nameof(FakeEntity)}.json")
                .Build();

            _mockConfigurationBuilder
                .Setup(x => x.Build())
                .Returns(configurationRoot);

            Expression<Action<ILogger>> loggerExpressionData = x => x.Log(
                LogLevel.Information,
                0,
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString() ==
                    $"Get data: {nameof(FakeEntity)}"
                ),
                null,
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)
            );
            _mockLogger.Setup(loggerExpressionData);

            // Act
            var result = _service.ExposedGetData();

            // Assert
            _mockLogger.Verify(loggerExpressionConfiguration, Times.Once);

            _mockHostEnvironment
                .Verify(
                    x => x.ContentRootPath,
                    Times.Once
                );

            _mockConfigurationBuilder
                .Verify(
                    x => x.Properties,
                    Times.Once
                );

            _mockConfigurationBuilder
                .Verify(
                    x => x.Add(
                        It.Is<JsonConfigurationSource>(
                            y => y.Path == $"{nameof(FakeEntity)}.json"
                                 && y.Optional == false
                        )
                    ),
                    Times.Once
                );

            _mockHostEnvironment
                .Verify(
                    x => x.EnvironmentName,
                    Times.Once
                );

            _mockConfigurationBuilder
                .Verify(
                    x => x.Add(
                        It.Is<JsonConfigurationSource>(
                            y => y.Path == $"{nameof(FakeEntity)}.{environmentName}.json"
                                 && y.Optional
                        )
                    ),
                    Times.Once
                );

            _mockConfigurationBuilder
                .Verify(
                    x => x.Add(
                        It.Is<EnvironmentVariablesConfigurationSource>(
                            y => string.IsNullOrEmpty(y.Prefix)
                        )
                    ),
                    Times.Once
                );

            _mockConfigurationBuilder
                .Verify(
                    x => x.Build(),
                    Times.Once
                );

            _mockLogger.Verify(loggerExpressionData, Times.Once);

            Assert.Equal(expectedResult, result, new FakeEntityEqualityComparer());
        }
    }
}