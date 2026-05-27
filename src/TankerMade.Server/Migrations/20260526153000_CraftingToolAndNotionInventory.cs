using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TankerMade.Server.Data;

#nullable disable

namespace TankerMade.Server.Migrations;

[DbContext(typeof(TankerMadeDbContext))]
[Migration("20260526153000_CraftingToolAndNotionInventory")]
public partial class CraftingToolAndNotionInventory : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "CraftingNotionInventoryItems",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                BrandName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                TypeName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                NormalizedBrandName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                NormalizedTypeName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                Size = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                ColorName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                RegularPrice = table.Column<decimal>(type: "TEXT", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CraftingNotionInventoryItems", x => x.Id);
                table.ForeignKey(
                    name: "FK_CraftingNotionInventoryItems_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "CraftingToolInventoryItems",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                BrandName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                TypeName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                NormalizedBrandName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                NormalizedTypeName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                Size = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                RegularPrice = table.Column<decimal>(type: "TEXT", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CraftingToolInventoryItems", x => x.Id);
                table.ForeignKey(
                    name: "FK_CraftingToolInventoryItems_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "CraftingNotionPurchases",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                NotionInventoryItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                SourceName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                Price = table.Column<decimal>(type: "TEXT", nullable: true),
                IsSalePrice = table.Column<bool>(type: "INTEGER", nullable: false),
                PurchasedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CraftingNotionPurchases", x => x.Id);
                table.ForeignKey(
                    name: "FK_CraftingNotionPurchases_CraftingNotionInventoryItems_NotionInventoryItemId",
                    column: x => x.NotionInventoryItemId,
                    principalTable: "CraftingNotionInventoryItems",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "CraftingToolPurchases",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                ToolInventoryItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                SourceName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                Price = table.Column<decimal>(type: "TEXT", nullable: true),
                IsSalePrice = table.Column<bool>(type: "INTEGER", nullable: false),
                PurchasedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CraftingToolPurchases", x => x.Id);
                table.ForeignKey(
                    name: "FK_CraftingToolPurchases_CraftingToolInventoryItems_ToolInventoryItemId",
                    column: x => x.ToolInventoryItemId,
                    principalTable: "CraftingToolInventoryItems",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_CraftingNotionInventoryItems_UserId",
            table: "CraftingNotionInventoryItems",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_CraftingNotionInventoryItems_UserId_NormalizedBrandName_NormalizedTypeName",
            table: "CraftingNotionInventoryItems",
            columns: new[] { "UserId", "NormalizedBrandName", "NormalizedTypeName" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_CraftingNotionPurchases_NotionInventoryItemId",
            table: "CraftingNotionPurchases",
            column: "NotionInventoryItemId");

        migrationBuilder.CreateIndex(
            name: "IX_CraftingToolInventoryItems_UserId",
            table: "CraftingToolInventoryItems",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_CraftingToolInventoryItems_UserId_NormalizedBrandName_NormalizedTypeName",
            table: "CraftingToolInventoryItems",
            columns: new[] { "UserId", "NormalizedBrandName", "NormalizedTypeName" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_CraftingToolPurchases_ToolInventoryItemId",
            table: "CraftingToolPurchases",
            column: "ToolInventoryItemId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "CraftingNotionPurchases");
        migrationBuilder.DropTable(name: "CraftingToolPurchases");
        migrationBuilder.DropTable(name: "CraftingNotionInventoryItems");
        migrationBuilder.DropTable(name: "CraftingToolInventoryItems");
    }
}
