using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GodelTech.Database.EntityFrameworkCore.Tests.Fakes
{
    public class FakeDatabaseServiceBase : DatabaseServiceBase
    {
        public FakeDatabaseServiceBase(
            ILogger logger,
            params DbContext[] dbContexts)
            : base(
                logger,
                dbContexts)
        {

        }

        public void ExposedRegisterDataService(IDataService dataService)
        {
            RegisterDataService(dataService);
        }
    }
}