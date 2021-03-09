using Microsoft.EntityFrameworkCore.Migrations;

namespace masz.Migrations
{
    public partial class MultiplePermissionRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(name: "ModRoleId", "GuildConfigs", "ModRoles");
            migrationBuilder.RenameColumn(name: "AdminRoleId", "GuildConfigs", "AdminRoles");
            migrationBuilder.RenameColumn(name: "MutedRoleId", "GuildConfigs", "MutedRoles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(name: "ModRoles", "GuildConfigs", "ModRoleId");
            migrationBuilder.RenameColumn(name: "AdminRoles", "GuildConfigs", "AdminRoleId");
            migrationBuilder.RenameColumn(name: "MutedRoles", "GuildConfigs", "MutedRoleId");
        }
    }
}
