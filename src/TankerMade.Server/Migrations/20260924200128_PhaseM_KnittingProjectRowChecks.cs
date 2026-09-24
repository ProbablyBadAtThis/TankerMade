using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TankerMade.Server.Migrations
{
    /// <inheritdoc />
    public partial class PhaseM_KnittingProjectRowChecks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KnittingProjectRowChecks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PatternStepId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RowNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnittingProjectRowChecks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnittingProjectRowChecks_KnittingPatternSteps_PatternStepId",
                        column: x => x.PatternStepId,
                        principalTable: "KnittingPatternSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KnittingProjectRowChecks_KnittingProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "KnittingProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KnittingProjectRowChecks_PatternStepId",
                table: "KnittingProjectRowChecks",
                column: "PatternStepId");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingProjectRowChecks_ProjectId",
                table: "KnittingProjectRowChecks",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingProjectRowChecks_ProjectId_PatternStepId_RowNumber",
                table: "KnittingProjectRowChecks",
                columns: new[] { "ProjectId", "PatternStepId", "RowNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KnittingProjectRowChecks");
        }
    }
}
