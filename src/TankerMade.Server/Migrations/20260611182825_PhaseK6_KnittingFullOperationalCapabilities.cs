using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TankerMade.Server.Migrations
{
    /// <inheritdoc />
    public partial class PhaseK6_KnittingFullOperationalCapabilities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KnittingProjectInventoryLinks_KnittingSupplyItems_SupplyItemId",
                table: "KnittingProjectInventoryLinks");

            migrationBuilder.DropIndex(
                name: "IX_KnittingProjectInventoryLinks_ProjectId_SupplyItemId",
                table: "KnittingProjectInventoryLinks");

            migrationBuilder.DropIndex(
                name: "IX_KnittingProjectInventoryLinks_SupplyItemId",
                table: "KnittingProjectInventoryLinks");

            migrationBuilder.RenameColumn(
                name: "SupplyItemId",
                table: "KnittingProjectInventoryLinks",
                newName: "InventoryItemId");

            migrationBuilder.AddColumn<string>(
                name: "InventoryItemType",
                table: "KnittingProjectInventoryLinks",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StitchCount",
                table: "KnittingPatternSteps",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequiredNotions",
                table: "KnittingPatterns",
                type: "TEXT",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SuggestedNeedleSizes",
                table: "KnittingPatterns",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SuggestedYarnWeight",
                table: "KnittingPatterns",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "InventoryItemId",
                table: "KnittingKitSupplies",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "KnittingInventoryReferenceItems",
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
                    table.PrimaryKey("PK_KnittingInventoryReferenceItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KnittingNotionInventoryItems",
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
                    table.PrimaryKey("PK_KnittingNotionInventoryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnittingNotionInventoryItems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KnittingPatternSupplies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PatternId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SupplyType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    InventoryItemId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnittingPatternSupplies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnittingPatternSupplies_KnittingPatterns_PatternId",
                        column: x => x.PatternId,
                        principalTable: "KnittingPatterns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KnittingToolInventoryItems",
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
                    table.PrimaryKey("PK_KnittingToolInventoryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnittingToolInventoryItems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KnittingYarnInventoryItems",
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
                    table.PrimaryKey("PK_KnittingYarnInventoryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnittingYarnInventoryItems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KnittingNotionPurchases",
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
                    table.PrimaryKey("PK_KnittingNotionPurchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnittingNotionPurchases_KnittingNotionInventoryItems_NotionInventoryItemId",
                        column: x => x.NotionInventoryItemId,
                        principalTable: "KnittingNotionInventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KnittingToolPurchases",
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
                    table.PrimaryKey("PK_KnittingToolPurchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnittingToolPurchases_KnittingToolInventoryItems_ToolInventoryItemId",
                        column: x => x.ToolInventoryItemId,
                        principalTable: "KnittingToolInventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KnittingYarnLots",
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
                    table.PrimaryKey("PK_KnittingYarnLots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnittingYarnLots_KnittingYarnInventoryItems_YarnInventoryItemId",
                        column: x => x.YarnInventoryItemId,
                        principalTable: "KnittingYarnInventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KnittingYarnPurchases",
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
                    table.PrimaryKey("PK_KnittingYarnPurchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnittingYarnPurchases_KnittingYarnInventoryItems_YarnInventoryItemId",
                        column: x => x.YarnInventoryItemId,
                        principalTable: "KnittingYarnInventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "KnittingInventoryReferenceItems",
                columns: new[] { "Id", "Category", "CreatedAt", "Name", "Slug", "SortOrder" },
                values: new object[,]
                {
                    { new Guid("88888888-8888-8888-8888-888888888801"), "yarn-weight", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Lace", "lace", 1 },
                    { new Guid("88888888-8888-8888-8888-888888888802"), "yarn-weight", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Fingering", "fingering", 2 },
                    { new Guid("88888888-8888-8888-8888-888888888803"), "yarn-weight", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "DK", "dk", 3 },
                    { new Guid("88888888-8888-8888-8888-888888888804"), "yarn-weight", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Worsted", "worsted", 4 },
                    { new Guid("88888888-8888-8888-8888-888888888805"), "yarn-weight", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Bulky", "bulky", 5 },
                    { new Guid("88888888-8888-8888-8888-888888888806"), "fiber-tag", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Synthetic", "synthetic", 1 },
                    { new Guid("88888888-8888-8888-8888-888888888807"), "fiber-tag", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Natural", "natural", 2 },
                    { new Guid("88888888-8888-8888-8888-888888888808"), "fiber-tag", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Blended", "blended", 3 },
                    { new Guid("88888888-8888-8888-8888-888888888809"), "tool-type", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Hook", "hook", 1 },
                    { new Guid("88888888-8888-8888-8888-88888888880a"), "tool-type", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Needle", "needle", 2 },
                    { new Guid("88888888-8888-8888-8888-88888888880b"), "tool-type", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Gauge Ruler", "gauge-ruler", 3 },
                    { new Guid("88888888-8888-8888-8888-88888888880c"), "tool-type", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Stitch Holder", "stitch-holder", 4 },
                    { new Guid("88888888-8888-8888-8888-88888888880d"), "notion-type", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Button", "button", 1 },
                    { new Guid("88888888-8888-8888-8888-88888888880e"), "notion-type", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Stitch Marker", "stitch-marker", 2 },
                    { new Guid("88888888-8888-8888-8888-88888888880f"), "notion-type", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Tapestry Needle", "tapestry-needle", 3 },
                    { new Guid("88888888-8888-8888-8888-888888888810"), "notion-type", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Zipper", "zipper", 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_KnittingProjectInventoryLinks_ProjectId_InventoryItemType_InventoryItemId",
                table: "KnittingProjectInventoryLinks",
                columns: new[] { "ProjectId", "InventoryItemType", "InventoryItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KnittingKitSupplies_InventoryItemId",
                table: "KnittingKitSupplies",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingInventoryReferenceItems_Category",
                table: "KnittingInventoryReferenceItems",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingInventoryReferenceItems_Category_Slug",
                table: "KnittingInventoryReferenceItems",
                columns: new[] { "Category", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KnittingInventoryReferenceItems_Category_SortOrder",
                table: "KnittingInventoryReferenceItems",
                columns: new[] { "Category", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_KnittingNotionInventoryItems_UserId",
                table: "KnittingNotionInventoryItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingNotionInventoryItems_UserId_NormalizedBrandName_NormalizedTypeName",
                table: "KnittingNotionInventoryItems",
                columns: new[] { "UserId", "NormalizedBrandName", "NormalizedTypeName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KnittingNotionPurchases_NotionInventoryItemId",
                table: "KnittingNotionPurchases",
                column: "NotionInventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingPatternSupplies_PatternId",
                table: "KnittingPatternSupplies",
                column: "PatternId");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingPatternSupplies_PatternId_SortOrder",
                table: "KnittingPatternSupplies",
                columns: new[] { "PatternId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_KnittingToolInventoryItems_UserId",
                table: "KnittingToolInventoryItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingToolInventoryItems_UserId_NormalizedBrandName_NormalizedTypeName",
                table: "KnittingToolInventoryItems",
                columns: new[] { "UserId", "NormalizedBrandName", "NormalizedTypeName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KnittingToolPurchases_ToolInventoryItemId",
                table: "KnittingToolPurchases",
                column: "ToolInventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingYarnInventoryItems_UserId",
                table: "KnittingYarnInventoryItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingYarnInventoryItems_UserId_NormalizedBrandName_NormalizedColorName",
                table: "KnittingYarnInventoryItems",
                columns: new[] { "UserId", "NormalizedBrandName", "NormalizedColorName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KnittingYarnLots_YarnInventoryItemId",
                table: "KnittingYarnLots",
                column: "YarnInventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingYarnLots_YarnInventoryItemId_LotNumber",
                table: "KnittingYarnLots",
                columns: new[] { "YarnInventoryItemId", "LotNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KnittingYarnPurchases_YarnInventoryItemId",
                table: "KnittingYarnPurchases",
                column: "YarnInventoryItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KnittingInventoryReferenceItems");

            migrationBuilder.DropTable(
                name: "KnittingNotionPurchases");

            migrationBuilder.DropTable(
                name: "KnittingPatternSupplies");

            migrationBuilder.DropTable(
                name: "KnittingToolPurchases");

            migrationBuilder.DropTable(
                name: "KnittingYarnLots");

            migrationBuilder.DropTable(
                name: "KnittingYarnPurchases");

            migrationBuilder.DropTable(
                name: "KnittingNotionInventoryItems");

            migrationBuilder.DropTable(
                name: "KnittingToolInventoryItems");

            migrationBuilder.DropTable(
                name: "KnittingYarnInventoryItems");

            migrationBuilder.DropIndex(
                name: "IX_KnittingProjectInventoryLinks_ProjectId_InventoryItemType_InventoryItemId",
                table: "KnittingProjectInventoryLinks");

            migrationBuilder.DropIndex(
                name: "IX_KnittingKitSupplies_InventoryItemId",
                table: "KnittingKitSupplies");

            migrationBuilder.DropColumn(
                name: "InventoryItemType",
                table: "KnittingProjectInventoryLinks");

            migrationBuilder.DropColumn(
                name: "StitchCount",
                table: "KnittingPatternSteps");

            migrationBuilder.DropColumn(
                name: "RequiredNotions",
                table: "KnittingPatterns");

            migrationBuilder.DropColumn(
                name: "SuggestedNeedleSizes",
                table: "KnittingPatterns");

            migrationBuilder.DropColumn(
                name: "SuggestedYarnWeight",
                table: "KnittingPatterns");

            migrationBuilder.DropColumn(
                name: "InventoryItemId",
                table: "KnittingKitSupplies");

            migrationBuilder.RenameColumn(
                name: "InventoryItemId",
                table: "KnittingProjectInventoryLinks",
                newName: "SupplyItemId");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingProjectInventoryLinks_ProjectId_SupplyItemId",
                table: "KnittingProjectInventoryLinks",
                columns: new[] { "ProjectId", "SupplyItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KnittingProjectInventoryLinks_SupplyItemId",
                table: "KnittingProjectInventoryLinks",
                column: "SupplyItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_KnittingProjectInventoryLinks_KnittingSupplyItems_SupplyItemId",
                table: "KnittingProjectInventoryLinks",
                column: "SupplyItemId",
                principalTable: "KnittingSupplyItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
