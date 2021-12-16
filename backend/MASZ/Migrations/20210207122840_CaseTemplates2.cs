using Microsoft.EntityFrameworkCore.Migrations;

namespace MASZ.Migrations
{
    public partial class CaseTemplates2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedForGuildId",
                table: "CaseTemplates",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ViewPermission",
                table: "CaseTemplates",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedForGuildId",
                table: "CaseTemplates");

            migrationBuilder.DropColumn(
                name: "ViewPermission",
                table: "CaseTemplates");
        }
    }
}
