using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TankerMade.Server.Data;

#nullable disable

namespace TankerMade.Server.Migrations;

[DbContext(typeof(TankerMadeDbContext))]
[Migration("20260522170000_CraftingProjectStepProgress")]
public partial class CraftingProjectStepProgress : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "CraftingProjectStepProgress",
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
                table.PrimaryKey("PK_CraftingProjectStepProgress", x => x.Id);
                table.ForeignKey(
                    name: "FK_CraftingProjectStepProgress_CraftingPatternSteps_PatternStepId",
                    column: x => x.PatternStepId,
                    principalTable: "CraftingPatternSteps",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_CraftingProjectStepProgress_CraftingProjects_ProjectId",
                    column: x => x.ProjectId,
                    principalTable: "CraftingProjects",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_CraftingProjectStepProgress_PatternStepId",
            table: "CraftingProjectStepProgress",
            column: "PatternStepId");

        migrationBuilder.CreateIndex(
            name: "IX_CraftingProjectStepProgress_ProjectId",
            table: "CraftingProjectStepProgress",
            column: "ProjectId");

        migrationBuilder.CreateIndex(
            name: "IX_CraftingProjectStepProgress_ProjectId_PatternStepId",
            table: "CraftingProjectStepProgress",
            columns: new[] { "ProjectId", "PatternStepId" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "CraftingProjectStepProgress");
    }
}
