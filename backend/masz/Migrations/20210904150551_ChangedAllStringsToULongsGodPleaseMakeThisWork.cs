using Microsoft.EntityFrameworkCore.Migrations;

namespace MASZ.Migrations
{
    public partial class ChangedAllStringsToULongsGodPleaseMakeThisWork : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ulong>(
                name: "UserId",
                table: "UserNotes",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "GuildId",
                table: "UserNotes",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "CreatorId",
                table: "UserNotes",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "UserB",
                table: "UserMappings",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "UserA",
                table: "UserMappings",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "GuildId",
                table: "UserMappings",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "CreatorUserId",
                table: "UserMappings",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "TargetChannelId",
                table: "UserInvites",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "JoinedUserId",
                table: "UserInvites",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "InviteIssuerId",
                table: "UserInvites",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "GuildId",
                table: "UserInvites",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "UserId",
                table: "ModCases",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "ModId",
                table: "ModCases",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "LockedByUserId",
                table: "ModCases",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "LastEditedByModId",
                table: "ModCases",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "GuildId",
                table: "ModCases",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "DeletedByUserId",
                table: "ModCases",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "UserId",
                table: "ModCaseComments",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "UserId",
                table: "GuildMotds",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "GuildId",
                table: "GuildMotds",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "GuildId",
                table: "GuildConfigs",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "UserId",
                table: "CaseTemplates",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "CreatedForGuildId",
                table: "CaseTemplates",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "UserId",
                table: "AutoModerationEvents",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "MessageId",
                table: "AutoModerationEvents",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "GuildId",
                table: "AutoModerationEvents",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "GuildId",
                table: "AutoModerationConfigs",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserNotes",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<string>(
                name: "GuildId",
                table: "UserNotes",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<string>(
                name: "CreatorId",
                table: "UserNotes",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<string>(
                name: "UserB",
                table: "UserMappings",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<string>(
                name: "UserA",
                table: "UserMappings",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<string>(
                name: "GuildId",
                table: "UserMappings",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<string>(
                name: "CreatorUserId",
                table: "UserMappings",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<string>(
                name: "TargetChannelId",
                table: "UserInvites",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<string>(
                name: "JoinedUserId",
                table: "UserInvites",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<string>(
                name: "InviteIssuerId",
                table: "UserInvites",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<string>(
                name: "GuildId",
                table: "UserInvites",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ModCases",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<string>(
                name: "ModId",
                table: "ModCases",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<string>(
                name: "LockedByUserId",
                table: "ModCases",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<string>(
                name: "LastEditedByModId",
                table: "ModCases",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<string>(
                name: "GuildId",
                table: "ModCases",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<string>(
                name: "DeletedByUserId",
                table: "ModCases",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ModCaseComments",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "GuildMotds",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<string>(
                name: "GuildId",
                table: "GuildMotds",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<string>(
                name: "GuildId",
                table: "GuildConfigs",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "CaseTemplates",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<string>(
                name: "CreatedForGuildId",
                table: "CaseTemplates",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AutoModerationEvents",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<string>(
                name: "MessageId",
                table: "AutoModerationEvents",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<string>(
                name: "GuildId",
                table: "AutoModerationEvents",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<string>(
                name: "GuildId",
                table: "AutoModerationConfigs",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(ulong));
        }
    }
}
