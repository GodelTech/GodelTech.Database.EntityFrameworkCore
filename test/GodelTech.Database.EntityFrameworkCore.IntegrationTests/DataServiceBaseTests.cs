﻿using System.Collections.Generic;
using System.IO;
using GodelTech.Database.EntityFrameworkCore.IntegrationTests.Fakes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace GodelTech.Database.EntityFrameworkCore.IntegrationTests
{
    public class DataServiceBaseTests
    {
        private const string FolderPath = "Fakes/Json";

        private readonly FakeDataServiceBase _service;

        public DataServiceBaseTests()
        {
            var configurationBuilder = new ConfigurationBuilder();

            var hostEnvironment = new HostingEnvironment
            {
                ContentRootPath = Directory.GetCurrentDirectory(),
                EnvironmentName = "Development"
            };

            using var loggerFactory = new NullLoggerFactory();
            var logger = new Logger<DataServiceBaseTests>(loggerFactory);

            _service = new FakeDataServiceBase(
                configurationBuilder,
                hostEnvironment,
                FolderPath,
                logger
            );
        }

        [Fact]
        public void GetDataItems_Success()
        {
            // Arrange
            var expectedResult = new List<FakeItem>
            {
                new FakeItem
                {
                    Id = 1
                },
                new FakeItem
                {
                    Id = 2
                },
                new FakeItem
                {
                    Id = 3
                }
            };

            // Act
            var result = _service.ExposedGetData();

            // Assert
            Assert.Equal(expectedResult, result, new FakeItemEqualityComparer());
        }
    }
}