using Microsoft.EntityFrameworkCore.Migrations;

namespace MASZ.Migrations
{
    public partial class RemoveSeverity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Severity",
                table: "ModCases");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Severity",
                table: "ModCases",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
