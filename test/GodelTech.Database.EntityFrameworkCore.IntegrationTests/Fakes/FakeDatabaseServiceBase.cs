using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GodelTech.Database.EntityFrameworkCore.IntegrationTests.Fakes
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
    }
}