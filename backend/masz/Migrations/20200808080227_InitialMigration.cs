using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace masz.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModCaseComments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ModCaseId = table.Column<int>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    ModId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModCaseComments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModCases",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    GuildId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    CurrentUsername = table.Column<string>(nullable: true),
                    ModId = table.Column<int>(nullable: false),
                    Severity = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    OccuredAt = table.Column<DateTime>(nullable: false),
                    Punishment = table.Column<string>(nullable: true),
                    Labels = table.Column<string>(nullable: true),
                    Others = table.Column<string>(nullable: true),
                    Valid = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModCases", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModCaseComments");

            migrationBuilder.DropTable(
                name: "ModCases");
        }
    }
}
