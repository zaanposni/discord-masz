using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace masz.Migrations
{
    public partial class ManagedPunishments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ManagedPunishment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CaseDbId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    GuildId = table.Column<string>(nullable: true),
                    PunishmentType = table.Column<int>(nullable: false),
                    UntilDate = table.Column<DateTime>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagedPunishment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManagedPunishment_ModCases_CaseDbId",
                        column: x => x.CaseDbId,
                        principalTable: "ModCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ManagedPunishment_CaseDbId",
                table: "ManagedPunishment",
                column: "CaseDbId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManagedPunishment");
        }
    }
}
