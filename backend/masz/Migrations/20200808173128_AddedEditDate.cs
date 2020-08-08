using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace masz.Migrations
{
    public partial class AddedEditDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastEditedAt",
                table: "ModCases",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastEditedAt",
                table: "ModCaseComments",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastEditedAt",
                table: "ModCases");

            migrationBuilder.DropColumn(
                name: "LastEditedAt",
                table: "ModCaseComments");
        }
    }
}
