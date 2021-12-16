using Microsoft.EntityFrameworkCore.Migrations;

namespace MASZ.Migrations
{
    public partial class CreationType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreationType",
                table: "ModCases",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationType",
                table: "ModCases");
        }
    }
}
