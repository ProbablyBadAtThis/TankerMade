using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TankerMade.Server.Migrations
{
    /// <inheritdoc />
    public partial class PhaseO_CommissionPublications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CoreCommissionPublications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModuleKey = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TokenHash = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastPublishedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SnapshotJson = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoreCommissionPublications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoreCommissionPublications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoreCommissionRevisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PublicationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Summary = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    PriceChanged = table.Column<bool>(type: "INTEGER", nullable: false),
                    DueDateChanged = table.Column<bool>(type: "INTEGER", nullable: false),
                    PreviousPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    NewPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    PreviousDueDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    NewDueDate = table.Column<DateOnly>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoreCommissionRevisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoreCommissionRevisions_CoreCommissionPublications_PublicationId",
                        column: x => x.PublicationId,
                        principalTable: "CoreCommissionPublications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoreCommissionPublications_TokenHash",
                table: "CoreCommissionPublications",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoreCommissionPublications_UserId_ModuleKey_ProjectId",
                table: "CoreCommissionPublications",
                columns: new[] { "UserId", "ModuleKey", "ProjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoreCommissionRevisions_PublicationId_OccurredAt",
                table: "CoreCommissionRevisions",
                columns: new[] { "PublicationId", "OccurredAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoreCommissionRevisions");

            migrationBuilder.DropTable(
                name: "CoreCommissionPublications");
        }
    }
}
