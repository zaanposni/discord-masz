using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MASZ.Migrations
{
    public partial class CaseTemplates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CaseTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: true),
                    TemplateName = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CaseTitle = table.Column<string>(nullable: true),
                    CaseDescription = table.Column<string>(nullable: true),
                    CaseLabels = table.Column<string>(nullable: true),
                    CasePunishment = table.Column<string>(nullable: true),
                    CasePunishmentType = table.Column<int>(nullable: false),
                    CasePunishedUntil = table.Column<DateTime>(nullable: true),
                    sendPublicNotification = table.Column<bool>(nullable: false),
                    handlePunishment = table.Column<bool>(nullable: false),
                    announceDm = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseTemplates", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaseTemplates");
        }
    }
}
