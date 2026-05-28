using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TankerMade.Server.Migrations
{
    /// <inheritdoc />
    public partial class PhaseH_Slice1_CommonFilterIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PrintingInventoryReferenceItems_Category_SortOrder",
                table: "PrintingInventoryReferenceItems",
                columns: new[] { "Category", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_CraftingProjects_UserId_IsArchived_Name",
                table: "CraftingProjects",
                columns: new[] { "UserId", "IsArchived", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_CraftingProjects_UserId_PatternId",
                table: "CraftingProjects",
                columns: new[] { "UserId", "PatternId" });

            migrationBuilder.CreateIndex(
                name: "IX_CraftingPatterns_UserId_ThemeId_Name",
                table: "CraftingPatterns",
                columns: new[] { "UserId", "ThemeId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_CraftingKits_UserId_IsArchived_Name",
                table: "CraftingKits",
                columns: new[] { "UserId", "IsArchived", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_CraftingInventoryReferenceItems_Category_SortOrder",
                table: "CraftingInventoryReferenceItems",
                columns: new[] { "Category", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_CoreAssetRecords_UserId_ModuleKey_IsDeleted_CreatedAt",
                table: "CoreAssetRecords",
                columns: new[] { "UserId", "ModuleKey", "IsDeleted", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PrintingInventoryReferenceItems_Category_SortOrder",
                table: "PrintingInventoryReferenceItems");

            migrationBuilder.DropIndex(
                name: "IX_CraftingProjects_UserId_IsArchived_Name",
                table: "CraftingProjects");

            migrationBuilder.DropIndex(
                name: "IX_CraftingProjects_UserId_PatternId",
                table: "CraftingProjects");

            migrationBuilder.DropIndex(
                name: "IX_CraftingPatterns_UserId_ThemeId_Name",
                table: "CraftingPatterns");

            migrationBuilder.DropIndex(
                name: "IX_CraftingKits_UserId_IsArchived_Name",
                table: "CraftingKits");

            migrationBuilder.DropIndex(
                name: "IX_CraftingInventoryReferenceItems_Category_SortOrder",
                table: "CraftingInventoryReferenceItems");

            migrationBuilder.DropIndex(
                name: "IX_CoreAssetRecords_UserId_ModuleKey_IsDeleted_CreatedAt",
                table: "CoreAssetRecords");
        }
    }
}
