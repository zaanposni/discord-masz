using Microsoft.EntityFrameworkCore.Migrations;

namespace MASZ.Migrations
{
    public partial class RenamedFields02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModNotificationChannelId",
                table: "GuildConfigs");

            migrationBuilder.DropColumn(
                name: "ModNotificationWebhook",
                table: "GuildConfigs");

            migrationBuilder.AddColumn<string>(
                name: "ModInternalNotificationWebhook",
                table: "GuildConfigs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModPublicNotificationWebhook",
                table: "GuildConfigs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModInternalNotificationWebhook",
                table: "GuildConfigs");

            migrationBuilder.DropColumn(
                name: "ModPublicNotificationWebhook",
                table: "GuildConfigs");

            migrationBuilder.AddColumn<string>(
                name: "ModNotificationChannelId",
                table: "GuildConfigs",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModNotificationWebhook",
                table: "GuildConfigs",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}
