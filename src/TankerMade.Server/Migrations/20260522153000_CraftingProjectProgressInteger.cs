using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TankerMade.Server.Data;

#nullable disable

namespace TankerMade.Server.Migrations;

[DbContext(typeof(TankerMadeDbContext))]
[Migration("20260522153000_CraftingProjectProgressInteger")]
public partial class CraftingProjectProgressInteger : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_CraftingProjects_PatternId",
            table: "CraftingProjects");

        migrationBuilder.DropIndex(
            name: "IX_CraftingProjects_Slug",
            table: "CraftingProjects");

        migrationBuilder.DropIndex(
            name: "IX_CraftingProjects_ThemeId",
            table: "CraftingProjects");

        migrationBuilder.DropIndex(
            name: "IX_CraftingProjects_UserId",
            table: "CraftingProjects");

        migrationBuilder.RenameTable(
            name: "CraftingProjects",
            newName: "CraftingProjects_old");

        migrationBuilder.CreateTable(
            name: "CraftingProjects",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                Slug = table.Column<string>(type: "TEXT", maxLength: 220, nullable: false),
                Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                PatternId = table.Column<Guid>(type: "TEXT", nullable: true),
                ThemeId = table.Column<Guid>(type: "TEXT", nullable: true),
                Difficulty = table.Column<int>(type: "INTEGER", nullable: false),
                Progress = table.Column<int>(type: "INTEGER", nullable: false),
                UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CraftingProjects", x => x.Id);
                table.ForeignKey(
                    name: "FK_CraftingProjects_CraftingPatterns_PatternId",
                    column: x => x.PatternId,
                    principalTable: "CraftingPatterns",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_CraftingProjects_Themes_ThemeId",
                    column: x => x.ThemeId,
                    principalTable: "Themes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_CraftingProjects_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.Sql("""
            INSERT INTO CraftingProjects
                (Id, Name, Slug, Description, PatternId, ThemeId, Difficulty, Progress, UserId, CreatedAt, UpdatedAt)
            SELECT
                Id, Name, Slug, Description, PatternId, ThemeId, Difficulty, CAST(Progress AS INTEGER), UserId, CreatedAt, UpdatedAt
            FROM CraftingProjects_old;
            """);

        migrationBuilder.DropTable(name: "CraftingProjects_old");

        CreateIndexes(migrationBuilder);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        DropIndexes(migrationBuilder);

        migrationBuilder.RenameTable(
            name: "CraftingProjects",
            newName: "CraftingProjects_old");

        migrationBuilder.CreateTable(
            name: "CraftingProjects",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                Slug = table.Column<string>(type: "TEXT", maxLength: 220, nullable: false),
                Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                PatternId = table.Column<Guid>(type: "TEXT", nullable: true),
                ThemeId = table.Column<Guid>(type: "TEXT", nullable: true),
                Difficulty = table.Column<int>(type: "INTEGER", nullable: false),
                Progress = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: false),
                UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CraftingProjects", x => x.Id);
                table.ForeignKey(
                    name: "FK_CraftingProjects_CraftingPatterns_PatternId",
                    column: x => x.PatternId,
                    principalTable: "CraftingPatterns",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_CraftingProjects_Themes_ThemeId",
                    column: x => x.ThemeId,
                    principalTable: "Themes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_CraftingProjects_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.Sql("""
            INSERT INTO CraftingProjects
                (Id, Name, Slug, Description, PatternId, ThemeId, Difficulty, Progress, UserId, CreatedAt, UpdatedAt)
            SELECT
                Id, Name, Slug, Description, PatternId, ThemeId, Difficulty, CAST(Progress AS TEXT), UserId, CreatedAt, UpdatedAt
            FROM CraftingProjects_old;
            """);

        migrationBuilder.DropTable(name: "CraftingProjects_old");

        CreateIndexes(migrationBuilder);
    }

    private static void CreateIndexes(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_CraftingProjects_PatternId",
            table: "CraftingProjects",
            column: "PatternId");

        migrationBuilder.CreateIndex(
            name: "IX_CraftingProjects_Slug",
            table: "CraftingProjects",
            column: "Slug");

        migrationBuilder.CreateIndex(
            name: "IX_CraftingProjects_ThemeId",
            table: "CraftingProjects",
            column: "ThemeId");

        migrationBuilder.CreateIndex(
            name: "IX_CraftingProjects_UserId",
            table: "CraftingProjects",
            column: "UserId");
    }

    private static void DropIndexes(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_CraftingProjects_PatternId",
            table: "CraftingProjects");

        migrationBuilder.DropIndex(
            name: "IX_CraftingProjects_Slug",
            table: "CraftingProjects");

        migrationBuilder.DropIndex(
            name: "IX_CraftingProjects_ThemeId",
            table: "CraftingProjects");

        migrationBuilder.DropIndex(
            name: "IX_CraftingProjects_UserId",
            table: "CraftingProjects");
    }
}
