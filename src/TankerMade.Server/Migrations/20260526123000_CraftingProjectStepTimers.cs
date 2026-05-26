using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TankerMade.Server.Data;

#nullable disable

namespace TankerMade.Server.Migrations;

[DbContext(typeof(TankerMadeDbContext))]
[Migration("20260526123000_CraftingProjectStepTimers")]
public partial class CraftingProjectStepTimers : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "CraftingProjectTimers");

        migrationBuilder.CreateTable(
            name: "CraftingProjectTimers",
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
                table.PrimaryKey("PK_CraftingProjectTimers", x => x.Id);
                table.ForeignKey(
                    name: "FK_CraftingProjectTimers_CraftingPatternSteps_PatternStepId",
                    column: x => x.PatternStepId,
                    principalTable: "CraftingPatternSteps",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_CraftingProjectTimers_CraftingProjects_ProjectId",
                    column: x => x.ProjectId,
                    principalTable: "CraftingProjects",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_CraftingProjectTimers_PatternStepId",
            table: "CraftingProjectTimers",
            column: "PatternStepId");

        migrationBuilder.CreateIndex(
            name: "IX_CraftingProjectTimers_ProjectId",
            table: "CraftingProjectTimers",
            column: "ProjectId");

        migrationBuilder.CreateIndex(
            name: "IX_CraftingProjectTimers_ProjectId_PatternStepId",
            table: "CraftingProjectTimers",
            columns: new[] { "ProjectId", "PatternStepId" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "CraftingProjectTimers");

        migrationBuilder.CreateTable(
            name: "CraftingProjectTimers",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                PatternPieceId = table.Column<Guid>(type: "TEXT", nullable: true),
                ElapsedSeconds = table.Column<long>(type: "INTEGER", nullable: false),
                IsRunning = table.Column<bool>(type: "INTEGER", nullable: false),
                StartedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CraftingProjectTimers", x => x.Id);
                table.ForeignKey(
                    name: "FK_CraftingProjectTimers_CraftingPatternPieces_PatternPieceId",
                    column: x => x.PatternPieceId,
                    principalTable: "CraftingPatternPieces",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_CraftingProjectTimers_CraftingProjects_ProjectId",
                    column: x => x.ProjectId,
                    principalTable: "CraftingProjects",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_CraftingProjectTimers_PatternPieceId",
            table: "CraftingProjectTimers",
            column: "PatternPieceId");

        migrationBuilder.CreateIndex(
            name: "IX_CraftingProjectTimers_ProjectId",
            table: "CraftingProjectTimers",
            column: "ProjectId");

        migrationBuilder.CreateIndex(
            name: "IX_CraftingProjectTimers_ProjectId_PatternPieceId",
            table: "CraftingProjectTimers",
            columns: new[] { "ProjectId", "PatternPieceId" });
    }
}
