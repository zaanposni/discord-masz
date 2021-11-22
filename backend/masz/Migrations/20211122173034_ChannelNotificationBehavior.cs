using Microsoft.EntityFrameworkCore.Migrations;

namespace masz.Migrations
{
    public partial class ChannelNotificationBehavior : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChannelNotificationBehavior",
                table: "AutoModerationConfigs",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChannelNotificationBehavior",
                table: "AutoModerationConfigs");
        }
    }
}
