using Microsoft.EntityFrameworkCore.Migrations;

namespace masz.Migrations
{
    public partial class AddedCurrentNickname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrentNickname",
                table: "ModCases",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentNickname",
                table: "ModCases");
        }
    }
}
