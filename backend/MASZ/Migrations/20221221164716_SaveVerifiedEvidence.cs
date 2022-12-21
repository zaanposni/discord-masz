using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace MASZ.Migrations
{
    public partial class SaveVerifiedEvidence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VerifiedEvidence",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GuildId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    MessageId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    ReportedContent = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerifiedEvidence", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ModCaseEvidenceMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EvidenceId = table.Column<int>(type: "int", nullable: false),
                    CaseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModCaseEvidenceMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModCaseEvidenceMappings_EvidenceId",
                        column: x => x.EvidenceId,
                        principalTable: "VerifiedEvidence",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                        );
                    table.ForeignKey(
                        name: "FK_ModCaseEvidenceMappings_CaseId",
                        column: x => x.CaseId,
                        principalTable: "ModCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                        );
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ModCaseEvidenceMappings_EvidenceId",
                table: "ModCaseEvidenceMappings",
                column: "EvidenceId"
                );
            migrationBuilder.CreateIndex(
                name: "IX_ModCaseEvidenceMappings_CaseId",
                table: "ModCaseEvidenceMappings",
                column: "CaseId"
                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name:"VerifiedEvidence");
            migrationBuilder.DropTable(name: "ModCaseEvidenceMappings");
        }
    }
}
