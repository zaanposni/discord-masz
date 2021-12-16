using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MASZ.Migrations
{
    public partial class AutoModeration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AutoModerationConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GuildId = table.Column<string>(nullable: true),
                    AutoModerationType = table.Column<int>(nullable: false),
                    AutoModerationAction = table.Column<int>(nullable: false),
                    PunishmentType = table.Column<int>(nullable: true),
                    PunishmentDurationMinutes = table.Column<int>(nullable: true),
                    IgnoreChannels = table.Column<string>(nullable: true),
                    IgnoreRoles = table.Column<string>(nullable: true),
                    TimeLimitMinutes = table.Column<int>(nullable: true),
                    Limit = table.Column<int>(nullable: true),
                    SendDmNotification = table.Column<bool>(nullable: false),
                    SendPublicNotification = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoModerationConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AutoModerationEvents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GuildId = table.Column<string>(nullable: true),
                    AutoModerationType = table.Column<int>(nullable: false),
                    AutoModerationAction = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    Nickname = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: true),
                    MessageId = table.Column<string>(nullable: true),
                    MessageContent = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    AssociatedCaseId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoModerationEvents", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AutoModerationConfigs");

            migrationBuilder.DropTable(
                name: "AutoModerationEvents");
        }
    }
}
