using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GodelTech.Database.EntityFrameworkCore.IntegrationTests.Migrations.FakeDb
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "FakeSchema");

            migrationBuilder.CreateTable(
                name: "FakeEntity",
                schema: "FakeSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FakeEntity", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FakeEntity",
                schema: "FakeSchema");
        }
    }
}
