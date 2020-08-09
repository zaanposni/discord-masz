using Microsoft.EntityFrameworkCore.Migrations;

namespace masz.Migrations
{
    public partial class AddedLastedEditedByField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastEditedByModId",
                table: "ModCases",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastEditedByModId",
                table: "ModCases");
        }
    }
}
