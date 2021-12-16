using Microsoft.EntityFrameworkCore.Migrations;

namespace MASZ.Migrations
{
    public partial class ModCaseDeleteFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModCaseComments_ModCases_ModCaseId",
                table: "ModCaseComments");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ModCaseComments_ModCases_ModCaseId",
                table: "ModCaseComments",
                column: "ModCaseId",
                principalTable: "ModCases",
                principalColumn: "Id");
        }
    }
}
