using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MASZ.Migrations
{
    public partial class ModCaseComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModCaseComments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ModCaseId = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModCaseComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModCaseComments_ModCases_ModCaseId",
                        column: x => x.ModCaseId,
                        principalTable: "ModCases",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModCaseComments_ModCaseId",
                table: "ModCaseComments",
                column: "ModCaseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModCaseComments");
        }
    }
}
