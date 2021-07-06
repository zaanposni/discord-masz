using Microsoft.EntityFrameworkCore.Migrations;

namespace masz.Migrations
{
    public partial class CustomWordFilter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomWordFilter",
                table: "AutoModerationConfigs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomWordFilter",
                table: "AutoModerationConfigs");
        }
    }
}
