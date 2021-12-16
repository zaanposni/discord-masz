using Microsoft.EntityFrameworkCore.Migrations;

namespace MASZ.Migrations
{
    public partial class ModcaseBin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserId",
                table: "ModCases",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MarkedToDeleteAt",
                table: "ModCases",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "ModCases");

            migrationBuilder.DropColumn(
                name: "MarkedToDeleteAt",
                table: "ModCases");
        }
    }
}
