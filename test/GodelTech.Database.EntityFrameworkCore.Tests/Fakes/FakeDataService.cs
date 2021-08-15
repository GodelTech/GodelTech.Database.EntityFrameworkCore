using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GodelTech.Database.EntityFrameworkCore.Tests.Fakes
{
    public class FakeDataService : DataService<FakeItem, int>
    {
        private readonly IList<FakeItem> _items;

        public FakeDataService(
            IConfigurationBuilder configurationBuilder,
            IHostEnvironment hostEnvironment,
            string folderPath,
            DbContext dbContext,
            bool enableIdentityInsert,
            Func<FakeItem, int> propertyToCompare,
            ILogger logger,
            IList<FakeItem> items)
            : base(
                configurationBuilder,
                hostEnvironment,
                folderPath,
                dbContext,
                enableIdentityInsert,
                propertyToCompare,
                logger)
        {
            _items = items;
        }

        protected override IList<FakeItem> GetData()
        {
            return _items;
        }
    }
}