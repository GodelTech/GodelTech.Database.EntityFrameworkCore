using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GodelTech.Database.EntityFrameworkCore.Tests.Fakes
{
    public class FakeDataService : DataService<FakeEntity, int>
    {
        private readonly IList<FakeEntity> _entities;

        public FakeDataService(
            IConfigurationBuilder configurationBuilder,
            IHostEnvironment hostEnvironment,
            string folderPath,
            DbContext dbContext,
            bool enableIdentityInsert,
            Func<FakeEntity, int> propertyToCompare,
            ILogger logger,
            IList<FakeEntity> entities)
            : base(
                configurationBuilder,
                hostEnvironment,
                folderPath,
                dbContext,
                enableIdentityInsert,
                propertyToCompare,
                logger)
        {
            _entities = entities;
        }

        protected override IList<FakeEntity> GetData()
        {
            return _entities;
        }
    }
}