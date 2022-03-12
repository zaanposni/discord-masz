using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MASZ.Migrations
{
    public partial class AddIgnoreRulesToGuildAuditLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IgnoreChannels",
                table: "GuildLevelAuditLogConfigs",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "IgnoreRoles",
                table: "GuildLevelAuditLogConfigs",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IgnoreChannels",
                table: "GuildLevelAuditLogConfigs");

            migrationBuilder.DropColumn(
                name: "IgnoreRoles",
                table: "GuildLevelAuditLogConfigs");
        }
    }
}
