using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GodelTech.Database.EntityFrameworkCore.Tests.Fakes
{
    public class FakeDataServiceBase : DataServiceBase<FakeEntity>
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

        public override Task ApplyDataAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public IList<FakeEntity> ExposedGetData()
        {
            return GetData();
        }
    }
}
