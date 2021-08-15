using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GodelTech.Database.EntityFrameworkCore.IntegrationTests.Fakes
{
    public class FakeDataService : DataService<FakeItem, int>
    {
        private IList<FakeItem> _items;

        public FakeDataService(
            IConfigurationBuilder configurationBuilder,
            IHostEnvironment hostEnvironment,
            string folderPath,
            DbContext dbContext,
            bool enableIdentityInsert,
            Func<FakeItem, int> propertyToCompare,
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

        public void SetData(IList<FakeItem> items)
        {
            _items = items;
        }

        protected override IList<FakeItem> GetData()
        {
            return _items;
        }
    }
}