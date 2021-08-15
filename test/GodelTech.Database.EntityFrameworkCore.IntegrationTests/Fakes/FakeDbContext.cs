using System;
using Microsoft.EntityFrameworkCore;

namespace GodelTech.Database.EntityFrameworkCore.IntegrationTests.Fakes
{
    public class FakeDbContext : DbContext
    {
        public FakeDbContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions)
        {

        }

        public DbSet<FakeEntity> FakeEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));

            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<FakeEntity>()
                .ToTable("FakeEntity");
        }
    }
}