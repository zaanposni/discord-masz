using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MASZ.Migrations
{
    public partial class RefactoredVerifiedEvidence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReportedDiscriminator",
                table: "VerifiedEvidence");

            migrationBuilder.DropColumn(
                name: "ReportedNickname",
                table: "VerifiedEvidence");

            migrationBuilder.DropColumn(
                name: "ReporterDiscriminator",
                table: "VerifiedEvidence");

            migrationBuilder.RenameColumn(
                name: "ReporterUsername",
                table: "VerifiedEvidence",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "ReporterUserId",
                table: "VerifiedEvidence",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "ReporterNickname",
                table: "VerifiedEvidence",
                newName: "Nickname");

            migrationBuilder.RenameColumn(
                name: "ReportedUsername",
                table: "VerifiedEvidence",
                newName: "Discriminator");

            migrationBuilder.RenameColumn(
                name: "ReportedUserId",
                table: "VerifiedEvidence",
                newName: "ModId");

            migrationBuilder.AddColumn<ulong>(
                name: "ChannelId",
                table: "VerifiedEvidence",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChannelId",
                table: "VerifiedEvidence");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "VerifiedEvidence",
                newName: "ReporterUsername");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "VerifiedEvidence",
                newName: "ReporterUserId");

            migrationBuilder.RenameColumn(
                name: "Nickname",
                table: "VerifiedEvidence",
                newName: "ReporterNickname");

            migrationBuilder.RenameColumn(
                name: "ModId",
                table: "VerifiedEvidence",
                newName: "ReportedUserId");

            migrationBuilder.RenameColumn(
                name: "Discriminator",
                table: "VerifiedEvidence",
                newName: "ReportedUsername");

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

            migrationBuilder.AddColumn<int>(
                name: "ReporterDiscriminator",
                table: "VerifiedEvidence",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
