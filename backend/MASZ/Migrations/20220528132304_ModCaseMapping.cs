using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MASZ.Migrations
{
    public partial class ModCaseMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModCaseMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CaseAId = table.Column<int>(type: "int", nullable: false),
                    CaseBId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModCaseMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModCaseMappings_ModCases_CaseAId",
                        column: x => x.CaseAId,
                        principalTable: "ModCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModCaseMappings_ModCases_CaseBId",
                        column: x => x.CaseBId,
                        principalTable: "ModCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ModCaseMappings_CaseAId",
                table: "ModCaseMappings",
                column: "CaseAId");

            migrationBuilder.CreateIndex(
                name: "IX_ModCaseMappings_CaseBId",
                table: "ModCaseMappings",
                column: "CaseBId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModCaseMappings");
        }
    }
}
