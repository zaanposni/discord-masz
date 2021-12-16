using Microsoft.EntityFrameworkCore.Migrations;

namespace MASZ.Migrations
{
    public partial class CommentLocking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowComments",
                table: "ModCases",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockedAt",
                table: "ModCases",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LockedByUserId",
                table: "ModCases",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowComments",
                table: "ModCases");

            migrationBuilder.DropColumn(
                name: "LockedAt",
                table: "ModCases");

            migrationBuilder.DropColumn(
                name: "LockedByUserId",
                table: "ModCases");
        }
    }
}
