using Microsoft.EntityFrameworkCore.Migrations;

namespace masz.Migrations
{
    public partial class MultiplePermissionRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminRoleId",
                table: "GuildConfigs");

            migrationBuilder.DropColumn(
                name: "ModRoleId",
                table: "GuildConfigs");

            migrationBuilder.DropColumn(
                name: "MutedRoleId",
                table: "GuildConfigs");

            migrationBuilder.AddColumn<string>(
                name: "AdminRoles",
                table: "GuildConfigs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModRoles",
                table: "GuildConfigs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MutedRoles",
                table: "GuildConfigs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminRoles",
                table: "GuildConfigs");

            migrationBuilder.DropColumn(
                name: "ModRoles",
                table: "GuildConfigs");

            migrationBuilder.DropColumn(
                name: "MutedRoles",
                table: "GuildConfigs");

            migrationBuilder.AddColumn<string>(
                name: "AdminRoleId",
                table: "GuildConfigs",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModRoleId",
                table: "GuildConfigs",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MutedRoleId",
                table: "GuildConfigs",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}
