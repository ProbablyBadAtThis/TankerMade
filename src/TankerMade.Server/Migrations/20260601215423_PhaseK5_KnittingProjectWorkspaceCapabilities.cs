using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TankerMade.Server.Migrations
{
    /// <inheritdoc />
    public partial class PhaseK5_KnittingProjectWorkspaceCapabilities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KnittingProjectInventoryLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SupplyItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                    QuantityPlanned = table.Column<decimal>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnittingProjectInventoryLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnittingProjectInventoryLinks_KnittingProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "KnittingProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KnittingProjectInventoryLinks_KnittingSupplyItems_SupplyItemId",
                        column: x => x.SupplyItemId,
                        principalTable: "KnittingSupplyItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KnittingProjectStepProgress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PatternStepId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsComplete = table.Column<bool>(type: "INTEGER", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnittingProjectStepProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnittingProjectStepProgress_KnittingPatternSteps_PatternStepId",
                        column: x => x.PatternStepId,
                        principalTable: "KnittingPatternSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KnittingProjectStepProgress_KnittingProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "KnittingProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KnittingProjectTimers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PatternStepId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ElapsedSeconds = table.Column<long>(type: "INTEGER", nullable: false),
                    IsRunning = table.Column<bool>(type: "INTEGER", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnittingProjectTimers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnittingProjectTimers_KnittingPatternSteps_PatternStepId",
                        column: x => x.PatternStepId,
                        principalTable: "KnittingPatternSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KnittingProjectTimers_KnittingProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "KnittingProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KnittingProjectInventoryLinks_ProjectId",
                table: "KnittingProjectInventoryLinks",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingProjectInventoryLinks_ProjectId_SupplyItemId",
                table: "KnittingProjectInventoryLinks",
                columns: new[] { "ProjectId", "SupplyItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KnittingProjectInventoryLinks_SupplyItemId",
                table: "KnittingProjectInventoryLinks",
                column: "SupplyItemId");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingProjectStepProgress_PatternStepId",
                table: "KnittingProjectStepProgress",
                column: "PatternStepId");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingProjectStepProgress_ProjectId",
                table: "KnittingProjectStepProgress",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingProjectStepProgress_ProjectId_PatternStepId",
                table: "KnittingProjectStepProgress",
                columns: new[] { "ProjectId", "PatternStepId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KnittingProjectTimers_PatternStepId",
                table: "KnittingProjectTimers",
                column: "PatternStepId");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingProjectTimers_ProjectId",
                table: "KnittingProjectTimers",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingProjectTimers_ProjectId_PatternStepId",
                table: "KnittingProjectTimers",
                columns: new[] { "ProjectId", "PatternStepId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KnittingProjectInventoryLinks");

            migrationBuilder.DropTable(
                name: "KnittingProjectStepProgress");

            migrationBuilder.DropTable(
                name: "KnittingProjectTimers");
        }
    }
}
