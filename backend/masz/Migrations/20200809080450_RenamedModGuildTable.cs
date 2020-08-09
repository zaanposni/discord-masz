using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace masz.Migrations
{
    public partial class RenamedModGuildTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModGuilds");

            migrationBuilder.CreateTable(
                name: "GuildConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GuildId = table.Column<string>(nullable: true),
                    ModRoleId = table.Column<string>(nullable: true),
                    AdminRoleId = table.Column<string>(nullable: true),
                    ModNotificationWebhook = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildConfigs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuildConfigs");

            migrationBuilder.CreateTable(
                name: "ModGuilds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AdminRoleId = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    GuildId = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ModNotificationWebhook = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ModRoleId = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModGuilds", x => x.Id);
                });
        }
    }
}
