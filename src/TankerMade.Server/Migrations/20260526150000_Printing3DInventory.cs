using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TankerMade.Server.Data;

#nullable disable

namespace TankerMade.Server.Migrations;

[DbContext(typeof(TankerMadeDbContext))]
[Migration("20260526150000_Printing3DInventory")]
public partial class Printing3DInventory : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "ModuleDefinitions",
            columns: new[] { "Id", "ModuleKey", "Name", "Description", "Version", "IsBundled", "CreatedAt" },
            columnTypes: new[] { "TEXT", "TEXT", "TEXT", "TEXT", "TEXT", "INTEGER", "TEXT" },
            values: new object[]
            {
                new Guid("55555555-5555-5555-5555-555555555552"),
                "printing-3d",
                "3D Printing",
                "Reference maker module for 3D printing inventory and workflow proofs.",
                "0.1.0",
                true,
                new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc)
            });

        migrationBuilder.CreateTable(
            name: "PrintingMaterialInventoryItems",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                MaterialType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                BrandName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                ColorName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                NormalizedMaterialType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                NormalizedBrandName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                NormalizedColorName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                TotalSpoolWeightGrams = table.Column<decimal>(type: "TEXT", nullable: false),
                RemainingWeightGrams = table.Column<decimal>(type: "TEXT", nullable: true),
                Diameter = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                StorageLocation = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                RegularPrice = table.Column<decimal>(type: "TEXT", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PrintingMaterialInventoryItems", x => x.Id);
                table.ForeignKey(
                    name: "FK_PrintingMaterialInventoryItems_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "PrintingInventoryPurchases",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                MaterialInventoryItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                SourceName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                Price = table.Column<decimal>(type: "TEXT", nullable: true),
                IsSalePrice = table.Column<bool>(type: "INTEGER", nullable: false),
                PurchasedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PrintingInventoryPurchases", x => x.Id);
                table.ForeignKey(
                    name: "FK_PrintingInventoryPurchases_PrintingMaterialInventoryItems_MaterialInventoryItemId",
                    column: x => x.MaterialInventoryItemId,
                    principalTable: "PrintingMaterialInventoryItems",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "PrintingSpools",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                MaterialInventoryItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                SpoolCode = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                StartingWeightGrams = table.Column<decimal>(type: "TEXT", nullable: false),
                RemainingWeightGrams = table.Column<decimal>(type: "TEXT", nullable: true),
                PrinterCompatibility = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PrintingSpools", x => x.Id);
                table.ForeignKey(
                    name: "FK_PrintingSpools_PrintingMaterialInventoryItems_MaterialInventoryItemId",
                    column: x => x.MaterialInventoryItemId,
                    principalTable: "PrintingMaterialInventoryItems",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_PrintingInventoryPurchases_MaterialInventoryItemId",
            table: "PrintingInventoryPurchases",
            column: "MaterialInventoryItemId");

        migrationBuilder.CreateIndex(
            name: "IX_PrintingMaterialInventoryItems_UserId",
            table: "PrintingMaterialInventoryItems",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_PrintingMaterialInventoryItems_UserId_NormalizedMaterialType_NormalizedBrandName_NormalizedColorName",
            table: "PrintingMaterialInventoryItems",
            columns: new[] { "UserId", "NormalizedMaterialType", "NormalizedBrandName", "NormalizedColorName" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_PrintingSpools_MaterialInventoryItemId",
            table: "PrintingSpools",
            column: "MaterialInventoryItemId");

        migrationBuilder.CreateIndex(
            name: "IX_PrintingSpools_MaterialInventoryItemId_SpoolCode",
            table: "PrintingSpools",
            columns: new[] { "MaterialInventoryItemId", "SpoolCode" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "PrintingInventoryPurchases");
        migrationBuilder.DropTable(name: "PrintingSpools");
        migrationBuilder.DropTable(name: "PrintingMaterialInventoryItems");

        migrationBuilder.DeleteData(
            table: "ModuleDefinitions",
            keyColumn: "Id",
            keyValue: new Guid("55555555-5555-5555-5555-555555555552"));
    }
}
