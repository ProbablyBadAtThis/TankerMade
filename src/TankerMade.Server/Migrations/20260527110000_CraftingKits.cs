using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TankerMade.Server.Data;

#nullable disable

namespace TankerMade.Server.Migrations;

[DbContext(typeof(TankerMadeDbContext))]
[Migration("20260527110000_CraftingKits")]
public partial class CraftingKits : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "CraftingKits",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                Slug = table.Column<string>(type: "TEXT", maxLength: 220, nullable: false),
                Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                Type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                ThemeId = table.Column<Guid>(type: "TEXT", nullable: true),
                Difficulty = table.Column<int>(type: "INTEGER", nullable: false),
                Progress = table.Column<int>(type: "INTEGER", nullable: false),
                IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CraftingKits", x => x.Id);
                table.ForeignKey(
                    name: "FK_CraftingKits_Themes_ThemeId",
                    column: x => x.ThemeId,
                    principalTable: "Themes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_CraftingKits_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "CraftingKitPieces",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                KitId = table.Column<Guid>(type: "TEXT", nullable: false),
                Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                PatternId = table.Column<Guid>(type: "TEXT", nullable: true),
                Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CraftingKitPieces", x => x.Id);
                table.ForeignKey(
                    name: "FK_CraftingKitPieces_CraftingKits_KitId",
                    column: x => x.KitId,
                    principalTable: "CraftingKits",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_CraftingKitPieces_CraftingPatterns_PatternId",
                    column: x => x.PatternId,
                    principalTable: "CraftingPatterns",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateTable(
            name: "CraftingKitSupplies",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                KitId = table.Column<Guid>(type: "TEXT", nullable: false),
                SupplyType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                Quantity = table.Column<decimal>(type: "TEXT", nullable: true),
                Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CraftingKitSupplies", x => x.Id);
                table.ForeignKey(
                    name: "FK_CraftingKitSupplies_CraftingKits_KitId",
                    column: x => x.KitId,
                    principalTable: "CraftingKits",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(name: "IX_CraftingKits_IsArchived", table: "CraftingKits", column: "IsArchived");
        migrationBuilder.CreateIndex(name: "IX_CraftingKits_Slug", table: "CraftingKits", column: "Slug");
        migrationBuilder.CreateIndex(name: "IX_CraftingKits_ThemeId", table: "CraftingKits", column: "ThemeId");
        migrationBuilder.CreateIndex(name: "IX_CraftingKits_UserId", table: "CraftingKits", column: "UserId");
        migrationBuilder.CreateIndex(name: "IX_CraftingKitPieces_KitId", table: "CraftingKitPieces", column: "KitId");
        migrationBuilder.CreateIndex(name: "IX_CraftingKitPieces_KitId_SortOrder", table: "CraftingKitPieces", columns: new[] { "KitId", "SortOrder" });
        migrationBuilder.CreateIndex(name: "IX_CraftingKitPieces_PatternId", table: "CraftingKitPieces", column: "PatternId");
        migrationBuilder.CreateIndex(name: "IX_CraftingKitSupplies_KitId", table: "CraftingKitSupplies", column: "KitId");
        migrationBuilder.CreateIndex(name: "IX_CraftingKitSupplies_KitId_SortOrder", table: "CraftingKitSupplies", columns: new[] { "KitId", "SortOrder" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "CraftingKitPieces");
        migrationBuilder.DropTable(name: "CraftingKitSupplies");
        migrationBuilder.DropTable(name: "CraftingKits");
    }
}
