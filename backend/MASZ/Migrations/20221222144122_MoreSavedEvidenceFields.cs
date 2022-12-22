using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MASZ.Migrations
{
    public partial class MoreSavedEvidenceFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReportedAt",
                table: "VerifiedEvidence",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ReportedDiscriminator",
                table: "VerifiedEvidence",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ReportedNickname",
                table: "VerifiedEvidence",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<ulong>(
                name: "ReportedUserId",
                table: "VerifiedEvidence",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<string>(
                name: "ReportedUsername",
                table: "VerifiedEvidence",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "ReporterDiscriminator",
                table: "VerifiedEvidence",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ReporterNickname",
                table: "VerifiedEvidence",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<ulong>(
                name: "ReporterUserId",
                table: "VerifiedEvidence",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<string>(
                name: "ReporterUsername",
                table: "VerifiedEvidence",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "SentAt",
                table: "VerifiedEvidence",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReportedAt",
                table: "VerifiedEvidence");

            migrationBuilder.DropColumn(
                name: "ReportedDiscriminator",
                table: "VerifiedEvidence");

            migrationBuilder.DropColumn(
                name: "ReportedNickname",
                table: "VerifiedEvidence");

            migrationBuilder.DropColumn(
                name: "ReportedUserId",
                table: "VerifiedEvidence");

            migrationBuilder.DropColumn(
                name: "ReportedUsername",
                table: "VerifiedEvidence");

            migrationBuilder.DropColumn(
                name: "ReporterDiscriminator",
                table: "VerifiedEvidence");

            migrationBuilder.DropColumn(
                name: "ReporterNickname",
                table: "VerifiedEvidence");

            migrationBuilder.DropColumn(
                name: "ReporterUserId",
                table: "VerifiedEvidence");

            migrationBuilder.DropColumn(
                name: "ReporterUsername",
                table: "VerifiedEvidence");

            migrationBuilder.DropColumn(
                name: "SentAt",
                table: "VerifiedEvidence");
        }
    }
}
