using Microsoft.EntityFrameworkCore.Migrations;

namespace masz.Migrations
{
    public partial class PublishModeratorInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PublishModeratorInfo",
                table: "GuildConfigs",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublishModeratorInfo",
                table: "GuildConfigs");
        }
    }
}
