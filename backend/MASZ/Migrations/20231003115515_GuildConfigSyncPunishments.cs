using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MASZ.Migrations
{
    public partial class GuildConfigSyncPunishments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SyncPunishments",
                table: "GuildConfigs",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SyncPunishments",
                table: "GuildConfigs");
        }
    }
}
