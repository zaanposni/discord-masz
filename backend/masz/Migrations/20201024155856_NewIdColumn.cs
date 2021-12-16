using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MASZ.Migrations
{
    public partial class NewIdColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModCaseComments");

            migrationBuilder.AddColumn<int>(
                name: "CaseId",
                table: "ModCases",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaseId",
                table: "ModCases");

            migrationBuilder.CreateTable(
                name: "ModCaseComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Content = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastEditedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModCaseGuildId = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ModCaseId = table.Column<int>(type: "int", nullable: false),
                    ModId = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModCaseComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModCaseComments_ModCases_ModCaseId",
                        column: x => x.ModCaseId,
                        principalTable: "ModCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModCaseComments_ModCaseId",
                table: "ModCaseComments",
                column: "ModCaseId");
        }
    }
}
