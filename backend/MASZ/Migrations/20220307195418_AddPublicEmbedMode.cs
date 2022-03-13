using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MASZ.Migrations
{
    public partial class AddPublicEmbedMode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PublicEmbedMode",
                table: "GuildConfigs",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicEmbedMode",
                table: "GuildConfigs");
        }
    }
}
