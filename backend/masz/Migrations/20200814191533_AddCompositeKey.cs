using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace masz.Migrations
{
    public partial class AddCompositeKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModCaseComments_ModCases_ModCaseId",
                table: "ModCaseComments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ModCases",
                table: "ModCases");

            migrationBuilder.DropIndex(
                name: "IX_ModCaseComments_ModCaseId",
                table: "ModCaseComments");

            migrationBuilder.AlterColumn<string>(
                name: "GuildId",
                table: "ModCases",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ModCases",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<string>(
                name: "ModCaseGuildId",
                table: "ModCaseComments",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModCases",
                table: "ModCases",
                columns: new[] { "GuildId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_ModCaseComments_ModCaseGuildId_ModCaseId",
                table: "ModCaseComments",
                columns: new[] { "ModCaseGuildId", "ModCaseId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ModCaseComments_ModCases_ModCaseGuildId_ModCaseId",
                table: "ModCaseComments",
                columns: new[] { "ModCaseGuildId", "ModCaseId" },
                principalTable: "ModCases",
                principalColumns: new[] { "GuildId", "Id" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModCaseComments_ModCases_ModCaseGuildId_ModCaseId",
                table: "ModCaseComments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ModCases",
                table: "ModCases");

            migrationBuilder.DropIndex(
                name: "IX_ModCaseComments_ModCaseGuildId_ModCaseId",
                table: "ModCaseComments");

            migrationBuilder.DropColumn(
                name: "ModCaseGuildId",
                table: "ModCaseComments");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ModCases",
                type: "int",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "GuildId",
                table: "ModCases",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModCases",
                table: "ModCases",
                column: "Id");

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
    }
}
