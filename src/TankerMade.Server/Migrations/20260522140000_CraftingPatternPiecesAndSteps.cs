using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TankerMade.Server.Data;

#nullable disable

namespace TankerMade.Server.Migrations;

[DbContext(typeof(TankerMadeDbContext))]
[Migration("20260522140000_CraftingPatternPiecesAndSteps")]
public partial class CraftingPatternPiecesAndSteps : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "CraftingPatternPieces",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                PatternId = table.Column<Guid>(type: "TEXT", nullable: false),
                Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CraftingPatternPieces", x => x.Id);
                table.ForeignKey(
                    name: "FK_CraftingPatternPieces_CraftingPatterns_PatternId",
                    column: x => x.PatternId,
                    principalTable: "CraftingPatterns",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "CraftingPatternSteps",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                PatternPieceId = table.Column<Guid>(type: "TEXT", nullable: false),
                RangeStart = table.Column<int>(type: "INTEGER", nullable: true),
                RangeEnd = table.Column<int>(type: "INTEGER", nullable: true),
                Label = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                Instructions = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CraftingPatternSteps", x => x.Id);
                table.ForeignKey(
                    name: "FK_CraftingPatternSteps_CraftingPatternPieces_PatternPieceId",
                    column: x => x.PatternPieceId,
                    principalTable: "CraftingPatternPieces",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_CraftingPatternPieces_PatternId",
            table: "CraftingPatternPieces",
            column: "PatternId");

        migrationBuilder.CreateIndex(
            name: "IX_CraftingPatternPieces_PatternId_SortOrder",
            table: "CraftingPatternPieces",
            columns: new[] { "PatternId", "SortOrder" });

        migrationBuilder.CreateIndex(
            name: "IX_CraftingPatternSteps_PatternPieceId",
            table: "CraftingPatternSteps",
            column: "PatternPieceId");

        migrationBuilder.CreateIndex(
            name: "IX_CraftingPatternSteps_PatternPieceId_SortOrder",
            table: "CraftingPatternSteps",
            columns: new[] { "PatternPieceId", "SortOrder" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "CraftingPatternSteps");
        migrationBuilder.DropTable(name: "CraftingPatternPieces");
    }
}
