using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GodelTech.Database.EntityFrameworkCore.IntegrationTests.Fakes
{
    public class FakeDataService : DataService<FakeEntity, int>
    {
        private IList<FakeEntity> _entities;

        public FakeDataService(
            IConfigurationBuilder configurationBuilder,
            IHostEnvironment hostEnvironment,
            string folderPath,
            DbContext dbContext,
            bool enableIdentityInsert,
            Func<FakeEntity, int> propertyToCompare,
            ILogger logger)
            : base(
                configurationBuilder,
                hostEnvironment,
                folderPath,
                dbContext,
                enableIdentityInsert,
                propertyToCompare,
                logger)
        {

        }

        public void SetData(IList<FakeEntity> entities)
        {
            _entities = entities;
        }

        public async Task ExposedExecuteSqlRawAsync(string sql)
        {
            await base.ExecuteSqlRawAsync(sql);
        }

        protected override IList<FakeEntity> GetData()
        {
            return _entities;
        }

        protected override Task ExecuteSqlRawAsync(string sql)
        {
            return Task.CompletedTask;
        }
    }
}