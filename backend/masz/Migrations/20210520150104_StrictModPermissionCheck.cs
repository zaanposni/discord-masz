using Microsoft.EntityFrameworkCore.Migrations;

namespace masz.Migrations
{
    public partial class StrictModPermissionCheck : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "StrictModPermissionCheck",
                table: "GuildConfigs",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StrictModPermissionCheck",
                table: "GuildConfigs");
        }
    }
}
