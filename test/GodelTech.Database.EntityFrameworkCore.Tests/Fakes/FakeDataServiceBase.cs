using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GodelTech.Database.EntityFrameworkCore.Tests.Fakes
{
    public class FakeDataServiceBase : DataServiceBase<FakeItem>
    {
        public FakeDataServiceBase(
            IConfigurationBuilder configurationBuilder,
            IHostEnvironment hostEnvironment,
            string folderPath,
            ILogger logger)
            : base(
                configurationBuilder,
                hostEnvironment,
                folderPath,
                logger)
        {

        }

        public override Task ApplyDataAsync()
        {
            return Task.CompletedTask;
        }

        public IList<FakeItem> ExposedGetData()
        {
            return GetData();
        }
    }
}