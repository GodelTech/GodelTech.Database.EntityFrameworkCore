using Microsoft.EntityFrameworkCore;

namespace GodelTech.Database.EntityFrameworkCore.IntegrationTests.Fakes
{
    public class FakeBaseDbContext : DbContext
    {
        public FakeBaseDbContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions)
        {

        }
    }
}
