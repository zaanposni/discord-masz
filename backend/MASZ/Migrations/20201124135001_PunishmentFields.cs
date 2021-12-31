using Microsoft.EntityFrameworkCore.Migrations;

namespace MASZ.Migrations
{
    public partial class PunishmentFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PunishedUntil",
                table: "ModCases",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PunishmentActive",
                table: "ModCases",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PunishmentType",
                table: "ModCases",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PunishedUntil",
                table: "ModCases");

            migrationBuilder.DropColumn(
                name: "PunishmentActive",
                table: "ModCases");

            migrationBuilder.DropColumn(
                name: "PunishmentType",
                table: "ModCases");
        }
    }
}
