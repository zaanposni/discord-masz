using Microsoft.EntityFrameworkCore.Migrations;

namespace MASZ.Migrations
{
    public partial class MultiplePermissionRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn("ModRoleId", "GuildConfigs", "ModRoles");
            migrationBuilder.RenameColumn("AdminRoleId", "GuildConfigs", "AdminRoles");
            migrationBuilder.RenameColumn("MutedRoleId", "GuildConfigs", "MutedRoles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(name: "ModRoles", "GuildConfigs", "ModRoleId");
            migrationBuilder.RenameColumn(name: "AdminRoles", "GuildConfigs", "AdminRoleId");
            migrationBuilder.RenameColumn(name: "MutedRoles", "GuildConfigs", "MutedRoleId");
        }
    }
}
