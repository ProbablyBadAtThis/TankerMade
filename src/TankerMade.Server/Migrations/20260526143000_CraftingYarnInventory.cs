using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TankerMade.Server.Data;

#nullable disable

namespace TankerMade.Server.Migrations;

[DbContext(typeof(TankerMadeDbContext))]
[Migration("20260526143000_CraftingYarnInventory")]
public partial class CraftingYarnInventory : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "CraftingYarnInventoryItems",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                BrandName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                ColorName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                NormalizedBrandName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                NormalizedColorName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                MainColor = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                WeightName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                FiberContent = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                FiberTag = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                TotalSkeins = table.Column<decimal>(type: "TEXT", nullable: false),
                EstimatedRemainingLength = table.Column<decimal>(type: "TEXT", nullable: true),
                LengthUnit = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                CurrentWeight = table.Column<decimal>(type: "TEXT", nullable: true),
                RegularPrice = table.Column<decimal>(type: "TEXT", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CraftingYarnInventoryItems", x => x.Id);
                table.ForeignKey(
                    name: "FK_CraftingYarnInventoryItems_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "CraftingInventoryPurchases",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                YarnInventoryItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                SourceName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                Price = table.Column<decimal>(type: "TEXT", nullable: true),
                IsSalePrice = table.Column<bool>(type: "INTEGER", nullable: false),
                PurchasedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CraftingInventoryPurchases", x => x.Id);
                table.ForeignKey(
                    name: "FK_CraftingInventoryPurchases_CraftingYarnInventoryItems_YarnInventoryItemId",
                    column: x => x.YarnInventoryItemId,
                    principalTable: "CraftingYarnInventoryItems",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "CraftingYarnLots",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                YarnInventoryItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                LotNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                Skeins = table.Column<decimal>(type: "TEXT", nullable: false),
                RemainingLength = table.Column<decimal>(type: "TEXT", nullable: true),
                CurrentWeight = table.Column<decimal>(type: "TEXT", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CraftingYarnLots", x => x.Id);
                table.ForeignKey(
                    name: "FK_CraftingYarnLots_CraftingYarnInventoryItems_YarnInventoryItemId",
                    column: x => x.YarnInventoryItemId,
                    principalTable: "CraftingYarnInventoryItems",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_CraftingInventoryPurchases_YarnInventoryItemId",
            table: "CraftingInventoryPurchases",
            column: "YarnInventoryItemId");

        migrationBuilder.CreateIndex(
            name: "IX_CraftingYarnInventoryItems_UserId",
            table: "CraftingYarnInventoryItems",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_CraftingYarnInventoryItems_UserId_NormalizedBrandName_NormalizedColorName",
            table: "CraftingYarnInventoryItems",
            columns: new[] { "UserId", "NormalizedBrandName", "NormalizedColorName" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_CraftingYarnLots_YarnInventoryItemId",
            table: "CraftingYarnLots",
            column: "YarnInventoryItemId");

        migrationBuilder.CreateIndex(
            name: "IX_CraftingYarnLots_YarnInventoryItemId_LotNumber",
            table: "CraftingYarnLots",
            columns: new[] { "YarnInventoryItemId", "LotNumber" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "CraftingInventoryPurchases");
        migrationBuilder.DropTable(name: "CraftingYarnLots");
        migrationBuilder.DropTable(name: "CraftingYarnInventoryItems");
    }
}
