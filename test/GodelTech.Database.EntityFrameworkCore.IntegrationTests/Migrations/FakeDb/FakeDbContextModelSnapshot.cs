﻿// <auto-generated />
using GodelTech.Database.EntityFrameworkCore.IntegrationTests.Fakes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GodelTech.Database.EntityFrameworkCore.IntegrationTests.Migrations.FakeDb
{
    [DbContext(typeof(FakeDbContext))]
    partial class FakeDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.21");

            modelBuilder.Entity("GodelTech.Database.EntityFrameworkCore.IntegrationTests.Fakes.FakeEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("FakeEntity", "FakeSchema");
                });
#pragma warning restore 612, 618
        }
    }
}
