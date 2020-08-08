using Microsoft.EntityFrameworkCore.Migrations;

namespace masz.Migrations
{
    public partial class FKConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ModCaseComments_ModCaseId",
                table: "ModCaseComments",
                column: "ModCaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_ModCaseComments_ModCases_ModCaseId",
                table: "ModCaseComments",
                column: "ModCaseId",
                principalTable: "ModCases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModCaseComments_ModCases_ModCaseId",
                table: "ModCaseComments");

            migrationBuilder.DropIndex(
                name: "IX_ModCaseComments_ModCaseId",
                table: "ModCaseComments");
        }
    }
}
