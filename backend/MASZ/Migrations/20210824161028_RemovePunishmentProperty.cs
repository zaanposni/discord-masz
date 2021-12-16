using Microsoft.EntityFrameworkCore.Migrations;

namespace MASZ.Migrations
{
    public partial class RemovePunishmentProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Punishment",
                table: "ModCases");

            migrationBuilder.DropColumn(
                name: "CasePunishment",
                table: "CaseTemplates");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Punishment",
                table: "ModCases",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CasePunishment",
                table: "CaseTemplates",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}
