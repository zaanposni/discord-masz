using Microsoft.EntityFrameworkCore.Migrations;

namespace MASZ.Migrations
{
    public partial class WhoIsOnJoin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ExecuteWhoisOnJoin",
                table: "GuildConfigs",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExecuteWhoisOnJoin",
                table: "GuildConfigs");
        }
    }
}
