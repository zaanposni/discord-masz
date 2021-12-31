using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MASZ.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuildConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GuildId = table.Column<string>(nullable: true),
                    ModRoleId = table.Column<string>(nullable: true),
                    AdminRoleId = table.Column<string>(nullable: true),
                    ModNotificationWebhook = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModCases",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GuildId = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    CurrentUsername = table.Column<string>(nullable: true),
                    CurrentNickname = table.Column<string>(nullable: true),
                    ModId = table.Column<string>(nullable: true),
                    Severity = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    OccuredAt = table.Column<DateTime>(nullable: false),
                    LastEditedAt = table.Column<DateTime>(nullable: false),
                    LastEditedByModId = table.Column<string>(nullable: true),
                    Punishment = table.Column<string>(nullable: true),
                    Labels = table.Column<string>(nullable: true),
                    Others = table.Column<string>(nullable: true),
                    Valid = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModCases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModCaseComments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ModCaseGuildId = table.Column<string>(nullable: true),
                    ModCaseId = table.Column<int>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    ModId = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    LastEditedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModCaseComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModCaseComments_ModCases_ModCaseId",
                        column: x => x.ModCaseId,
                        principalTable: "ModCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModCaseComments_ModCaseId",
                table: "ModCaseComments",
                column: "ModCaseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuildConfigs");

            migrationBuilder.DropTable(
                name: "ModCaseComments");

            migrationBuilder.DropTable(
                name: "ModCases");
        }
    }
}
