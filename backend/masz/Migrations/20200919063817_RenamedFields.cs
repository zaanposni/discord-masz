using Microsoft.EntityFrameworkCore.Migrations;

namespace MASZ.Migrations
{
    public partial class RenamedFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentNickname",
                table: "ModCases");

            migrationBuilder.DropColumn(
                name: "CurrentUsername",
                table: "ModCases");

            migrationBuilder.AddColumn<string>(
                name: "Nickname",
                table: "ModCases",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "ModCases",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nickname",
                table: "ModCases");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "ModCases");

            migrationBuilder.AddColumn<string>(
                name: "CurrentNickname",
                table: "ModCases",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentUsername",
                table: "ModCases",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}
