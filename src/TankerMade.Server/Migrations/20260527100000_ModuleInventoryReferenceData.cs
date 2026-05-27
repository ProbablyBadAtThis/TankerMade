using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TankerMade.Server.Data;

#nullable disable

namespace TankerMade.Server.Migrations;

[DbContext(typeof(TankerMadeDbContext))]
[Migration("20260527100000_ModuleInventoryReferenceData")]
public partial class ModuleInventoryReferenceData : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "CraftingInventoryReferenceItems",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                Name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                Slug = table.Column<string>(type: "TEXT", maxLength: 170, nullable: false),
                SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CraftingInventoryReferenceItems", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "PrintingInventoryReferenceItems",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                Name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                Slug = table.Column<string>(type: "TEXT", maxLength: 170, nullable: false),
                SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PrintingInventoryReferenceItems", x => x.Id);
            });

        migrationBuilder.InsertData(
            table: "CraftingInventoryReferenceItems",
            columns: new[] { "Id", "Category", "Name", "Slug", "SortOrder", "CreatedAt" },
            columnTypes: new[] { "TEXT", "TEXT", "TEXT", "TEXT", "INTEGER", "TEXT" },
            values: new object[,]
            {
                { new Guid("66666666-6666-6666-6666-666666666601"), "yarn-weight", "Lace", "lace", 1, new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                { new Guid("66666666-6666-6666-6666-666666666602"), "yarn-weight", "Fingering", "fingering", 2, new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                { new Guid("66666666-6666-6666-6666-666666666603"), "yarn-weight", "DK", "dk", 3, new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                { new Guid("66666666-6666-6666-6666-666666666604"), "yarn-weight", "Worsted", "worsted", 4, new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                { new Guid("66666666-6666-6666-6666-666666666605"), "yarn-weight", "Bulky", "bulky", 5, new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                { new Guid("66666666-6666-6666-6666-666666666606"), "fiber-tag", "Synthetic", "synthetic", 1, new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                { new Guid("66666666-6666-6666-6666-666666666607"), "fiber-tag", "Natural", "natural", 2, new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                { new Guid("66666666-6666-6666-6666-666666666608"), "fiber-tag", "Blended", "blended", 3, new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                { new Guid("66666666-6666-6666-6666-666666666609"), "tool-type", "Hook", "hook", 1, new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                { new Guid("66666666-6666-6666-6666-66666666660a"), "tool-type", "Needle", "needle", 2, new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                { new Guid("66666666-6666-6666-6666-66666666660b"), "tool-type", "Gauge Ruler", "gauge-ruler", 3, new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                { new Guid("66666666-6666-6666-6666-66666666660c"), "tool-type", "Stitch Holder", "stitch-holder", 4, new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                { new Guid("66666666-6666-6666-6666-66666666660d"), "notion-type", "Button", "button", 1, new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                { new Guid("66666666-6666-6666-6666-66666666660e"), "notion-type", "Stitch Marker", "stitch-marker", 2, new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                { new Guid("66666666-6666-6666-6666-66666666660f"), "notion-type", "Tapestry Needle", "tapestry-needle", 3, new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                { new Guid("66666666-6666-6666-6666-666666666610"), "notion-type", "Zipper", "zipper", 4, new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc) }
            });

        migrationBuilder.InsertData(
            table: "PrintingInventoryReferenceItems",
            columns: new[] { "Id", "Category", "Name", "Slug", "SortOrder", "CreatedAt" },
            columnTypes: new[] { "TEXT", "TEXT", "TEXT", "TEXT", "INTEGER", "TEXT" },
            values: new object[,]
            {
                { new Guid("77777777-7777-7777-7777-777777777701"), "material-type", "PLA", "pla", 1, new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                { new Guid("77777777-7777-7777-7777-777777777702"), "material-type", "PETG", "petg", 2, new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                { new Guid("77777777-7777-7777-7777-777777777703"), "material-type", "ABS", "abs", 3, new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                { new Guid("77777777-7777-7777-7777-777777777704"), "material-type", "TPU", "tpu", 4, new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                { new Guid("77777777-7777-7777-7777-777777777705"), "diameter", "1.75mm", "1-75mm", 1, new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                { new Guid("77777777-7777-7777-7777-777777777706"), "diameter", "2.85mm", "2-85mm", 2, new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                { new Guid("77777777-7777-7777-7777-777777777707"), "printer-tooling", "Nozzle", "nozzle", 1, new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                { new Guid("77777777-7777-7777-7777-777777777708"), "printer-tooling", "Build Plate", "build-plate", 2, new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                { new Guid("77777777-7777-7777-7777-777777777709"), "printer-tooling", "Filament Dryer", "filament-dryer", 3, new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                { new Guid("77777777-7777-7777-7777-77777777770a"), "printer-tooling", "Scraper", "scraper", 4, new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc) }
            });

        migrationBuilder.CreateIndex(
            name: "IX_CraftingInventoryReferenceItems_Category",
            table: "CraftingInventoryReferenceItems",
            column: "Category");

        migrationBuilder.CreateIndex(
            name: "IX_CraftingInventoryReferenceItems_Category_Slug",
            table: "CraftingInventoryReferenceItems",
            columns: new[] { "Category", "Slug" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_PrintingInventoryReferenceItems_Category",
            table: "PrintingInventoryReferenceItems",
            column: "Category");

        migrationBuilder.CreateIndex(
            name: "IX_PrintingInventoryReferenceItems_Category_Slug",
            table: "PrintingInventoryReferenceItems",
            columns: new[] { "Category", "Slug" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "CraftingInventoryReferenceItems");
        migrationBuilder.DropTable(name: "PrintingInventoryReferenceItems");
    }
}
