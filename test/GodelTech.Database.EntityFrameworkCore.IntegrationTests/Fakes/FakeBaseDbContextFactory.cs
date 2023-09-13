using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GodelTech.Database.EntityFrameworkCore.IntegrationTests.Fakes
{
    public class FakeBaseDbContextFactory : IDesignTimeDbContextFactory<FakeBaseDbContext>
    {
        public FakeBaseDbContext CreateDbContext(string[] args)
        {
            var dbContextOptions = new DbContextOptionsBuilder<FakeBaseDbContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            return new FakeBaseDbContext(dbContextOptions);
        }
    }
}
