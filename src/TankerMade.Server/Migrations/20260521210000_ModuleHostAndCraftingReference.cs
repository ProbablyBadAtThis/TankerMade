using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TankerMade.Server.Migrations;

public partial class ModuleHostAndCraftingReference : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Projects");
        migrationBuilder.DropTable(name: "Patterns");

        migrationBuilder.CreateTable(
            name: "ModuleDefinitions",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                ModuleKey = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                Name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                Version = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                IsBundled = table.Column<bool>(type: "INTEGER", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ModuleDefinitions", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "CraftingPatterns",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                Slug = table.Column<string>(type: "TEXT", maxLength: 220, nullable: false),
                Type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                Form = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                Difficulty = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                ThemeId = table.Column<Guid>(type: "TEXT", nullable: true),
                SourceId = table.Column<Guid>(type: "TEXT", nullable: true),
                UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CraftingPatterns", x => x.Id);
                table.ForeignKey(
                    name: "FK_CraftingPatterns_Sources_SourceId",
                    column: x => x.SourceId,
                    principalTable: "Sources",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_CraftingPatterns_Themes_ThemeId",
                    column: x => x.ThemeId,
                    principalTable: "Themes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_CraftingPatterns_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "UserModuleActivations",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                ModuleDefinitionId = table.Column<Guid>(type: "TEXT", nullable: false),
                IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                ActivatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserModuleActivations", x => x.Id);
                table.ForeignKey(
                    name: "FK_UserModuleActivations_ModuleDefinitions_ModuleDefinitionId",
                    column: x => x.ModuleDefinitionId,
                    principalTable: "ModuleDefinitions",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_UserModuleActivations_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

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

        migrationBuilder.InsertData(
            table: "ModuleDefinitions",
            columns: new[] { "Id", "ModuleKey", "Name", "Description", "Version", "IsBundled", "CreatedAt" },
            values: new object[]
            {
                new Guid("55555555-5555-5555-5555-555555555551"),
                "crafting",
                "Crafting",
                "Reference maker module for pattern-based crafting workflows.",
                "0.1.0",
                true,
                new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc)
            });

        migrationBuilder.CreateIndex(
            name: "IX_ModuleDefinitions_ModuleKey",
            table: "ModuleDefinitions",
            column: "ModuleKey",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_UserModuleActivations_ModuleDefinitionId",
            table: "UserModuleActivations",
            column: "ModuleDefinitionId");

        migrationBuilder.CreateIndex(
            name: "IX_UserModuleActivations_UserId_ModuleDefinitionId",
            table: "UserModuleActivations",
            columns: new[] { "UserId", "ModuleDefinitionId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_CraftingPatterns_Slug",
            table: "CraftingPatterns",
            column: "Slug");

        migrationBuilder.CreateIndex(
            name: "IX_CraftingPatterns_SourceId",
            table: "CraftingPatterns",
            column: "SourceId");

        migrationBuilder.CreateIndex(
            name: "IX_CraftingPatterns_ThemeId",
            table: "CraftingPatterns",
            column: "ThemeId");

        migrationBuilder.CreateIndex(
            name: "IX_CraftingPatterns_UserId",
            table: "CraftingPatterns",
            column: "UserId");

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

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "CraftingProjects");
        migrationBuilder.DropTable(name: "UserModuleActivations");
        migrationBuilder.DropTable(name: "CraftingPatterns");
        migrationBuilder.DropTable(name: "ModuleDefinitions");

        migrationBuilder.CreateTable(
            name: "Patterns",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                Slug = table.Column<string>(type: "TEXT", maxLength: 220, nullable: false),
                Type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                Form = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                Difficulty = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                ThemeId = table.Column<Guid>(type: "TEXT", nullable: true),
                SourceId = table.Column<Guid>(type: "TEXT", nullable: true),
                UserId = table.Column<Guid>(type: "TEXT", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Patterns", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Projects",
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
                table.PrimaryKey("PK_Projects", x => x.Id);
            });
    }
}
