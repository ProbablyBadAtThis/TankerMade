using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TankerMade.Server.Migrations
{
    /// <inheritdoc />
    public partial class PhaseN_RetireCraftingModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CraftingInventoryPurchases");

            migrationBuilder.DropTable(
                name: "CraftingInventoryReferenceItems");

            migrationBuilder.DropTable(
                name: "CraftingKitSupplies");

            migrationBuilder.DropTable(
                name: "CraftingNotionPurchases");

            migrationBuilder.DropTable(
                name: "CraftingProjectInventoryLinks");

            migrationBuilder.DropTable(
                name: "CraftingProjectStepProgress");

            migrationBuilder.DropTable(
                name: "CraftingProjectTimers");

            migrationBuilder.DropTable(
                name: "CraftingToolPurchases");

            migrationBuilder.DropTable(
                name: "CraftingYarnLots");

            migrationBuilder.DropTable(
                name: "CraftingNotionInventoryItems");

            migrationBuilder.DropTable(
                name: "CraftingPatternSteps");

            migrationBuilder.DropTable(
                name: "CraftingProjects");

            migrationBuilder.DropTable(
                name: "CraftingToolInventoryItems");

            migrationBuilder.DropTable(
                name: "CraftingYarnInventoryItems");

            migrationBuilder.DropTable(
                name: "CraftingPatternPieces");

            migrationBuilder.DropTable(
                name: "CraftingKitPieces");

            migrationBuilder.DropTable(
                name: "CraftingKits");

            migrationBuilder.DropTable(
                name: "CraftingPatterns");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CraftingInventoryReferenceItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 170, nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CraftingInventoryReferenceItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CraftingKits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Difficulty = table.Column<int>(type: "INTEGER", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Progress = table.Column<int>(type: "INTEGER", nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 220, nullable: false),
                    ThemeId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false)
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
                name: "CraftingNotionInventoryItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BrandName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    ColorName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    NormalizedBrandName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    NormalizedTypeName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    RegularPrice = table.Column<decimal>(type: "TEXT", nullable: true),
                    Size = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TypeName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false)
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
                name: "CraftingPatterns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Difficulty = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Form = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 220, nullable: false),
                    SourceId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ThemeId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false)
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
                name: "CraftingToolInventoryItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BrandName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    NormalizedBrandName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    NormalizedTypeName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    RegularPrice = table.Column<decimal>(type: "TEXT", nullable: true),
                    Size = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TypeName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false)
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
                name: "CraftingYarnInventoryItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BrandName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    ColorName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CurrentWeight = table.Column<decimal>(type: "TEXT", nullable: true),
                    EstimatedRemainingLength = table.Column<decimal>(type: "TEXT", nullable: true),
                    FiberContent = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    FiberTag = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    LengthUnit = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    MainColor = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NormalizedBrandName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    NormalizedColorName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    RegularPrice = table.Column<decimal>(type: "TEXT", nullable: true),
                    TotalSkeins = table.Column<decimal>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    WeightName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
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
                name: "CraftingKitSupplies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    KitId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Quantity = table.Column<decimal>(type: "TEXT", nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    SupplyType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
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

            migrationBuilder.CreateTable(
                name: "CraftingNotionPurchases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsSalePrice = table.Column<bool>(type: "INTEGER", nullable: false),
                    NotionInventoryItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: true),
                    PurchasedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SourceName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false)
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
                name: "CraftingKitPieces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    KitId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    PatternId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
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
                name: "CraftingPatternPieces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PatternId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
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
                name: "CraftingToolPurchases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsSalePrice = table.Column<bool>(type: "INTEGER", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: true),
                    PurchasedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SourceName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    ToolInventoryItemId = table.Column<Guid>(type: "TEXT", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "CraftingInventoryPurchases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsSalePrice = table.Column<bool>(type: "INTEGER", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: true),
                    PurchasedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SourceName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    YarnInventoryItemId = table.Column<Guid>(type: "TEXT", nullable: false)
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
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CurrentWeight = table.Column<decimal>(type: "TEXT", nullable: true),
                    LotNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    RemainingLength = table.Column<decimal>(type: "TEXT", nullable: true),
                    Skeins = table.Column<decimal>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    YarnInventoryItemId = table.Column<Guid>(type: "TEXT", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "CraftingProjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Difficulty = table.Column<int>(type: "INTEGER", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    KitId = table.Column<Guid>(type: "TEXT", nullable: true),
                    KitPieceId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PatternId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Progress = table.Column<int>(type: "INTEGER", nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 220, nullable: false),
                    ThemeId = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CraftingProjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CraftingProjects_CraftingKitPieces_KitPieceId",
                        column: x => x.KitPieceId,
                        principalTable: "CraftingKitPieces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CraftingProjects_CraftingKits_KitId",
                        column: x => x.KitId,
                        principalTable: "CraftingKits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
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

            migrationBuilder.CreateTable(
                name: "CraftingPatternSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Instructions = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    Label = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PatternPieceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RangeEnd = table.Column<int>(type: "INTEGER", nullable: true),
                    RangeStart = table.Column<int>(type: "INTEGER", nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "CraftingProjectInventoryLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InventoryItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                    InventoryItemType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    QuantityPlanned = table.Column<decimal>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CraftingProjectInventoryLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CraftingProjectInventoryLinks_CraftingProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "CraftingProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CraftingProjectStepProgress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsComplete = table.Column<bool>(type: "INTEGER", nullable: false),
                    PatternStepId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CraftingProjectStepProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CraftingProjectStepProgress_CraftingPatternSteps_PatternStepId",
                        column: x => x.PatternStepId,
                        principalTable: "CraftingPatternSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CraftingProjectStepProgress_CraftingProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "CraftingProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CraftingProjectTimers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ElapsedSeconds = table.Column<long>(type: "INTEGER", nullable: false),
                    IsRunning = table.Column<bool>(type: "INTEGER", nullable: false),
                    PatternStepId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CraftingProjectTimers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CraftingProjectTimers_CraftingPatternSteps_PatternStepId",
                        column: x => x.PatternStepId,
                        principalTable: "CraftingPatternSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CraftingProjectTimers_CraftingProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "CraftingProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CraftingInventoryReferenceItems",
                columns: new[] { "Id", "Category", "CreatedAt", "Name", "Slug", "SortOrder" },
                values: new object[,]
                {
                    { new Guid("66666666-6666-6666-6666-666666666601"), "yarn-weight", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Lace", "lace", 1 },
                    { new Guid("66666666-6666-6666-6666-666666666602"), "yarn-weight", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Fingering", "fingering", 2 },
                    { new Guid("66666666-6666-6666-6666-666666666603"), "yarn-weight", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "DK", "dk", 3 },
                    { new Guid("66666666-6666-6666-6666-666666666604"), "yarn-weight", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Worsted", "worsted", 4 },
                    { new Guid("66666666-6666-6666-6666-666666666605"), "yarn-weight", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Bulky", "bulky", 5 },
                    { new Guid("66666666-6666-6666-6666-666666666606"), "fiber-tag", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Synthetic", "synthetic", 1 },
                    { new Guid("66666666-6666-6666-6666-666666666607"), "fiber-tag", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Natural", "natural", 2 },
                    { new Guid("66666666-6666-6666-6666-666666666608"), "fiber-tag", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Blended", "blended", 3 },
                    { new Guid("66666666-6666-6666-6666-666666666609"), "tool-type", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Hook", "hook", 1 },
                    { new Guid("66666666-6666-6666-6666-66666666660a"), "tool-type", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Needle", "needle", 2 },
                    { new Guid("66666666-6666-6666-6666-66666666660b"), "tool-type", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Gauge Ruler", "gauge-ruler", 3 },
                    { new Guid("66666666-6666-6666-6666-66666666660c"), "tool-type", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Stitch Holder", "stitch-holder", 4 },
                    { new Guid("66666666-6666-6666-6666-66666666660d"), "notion-type", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Button", "button", 1 },
                    { new Guid("66666666-6666-6666-6666-66666666660e"), "notion-type", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Stitch Marker", "stitch-marker", 2 },
                    { new Guid("66666666-6666-6666-6666-66666666660f"), "notion-type", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Tapestry Needle", "tapestry-needle", 3 },
                    { new Guid("66666666-6666-6666-6666-666666666610"), "notion-type", new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Zipper", "zipper", 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CraftingInventoryPurchases_YarnInventoryItemId",
                table: "CraftingInventoryPurchases",
                column: "YarnInventoryItemId");

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
                name: "IX_CraftingInventoryReferenceItems_Category_SortOrder",
                table: "CraftingInventoryReferenceItems",
                columns: new[] { "Category", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_CraftingKitPieces_KitId",
                table: "CraftingKitPieces",
                column: "KitId");

            migrationBuilder.CreateIndex(
                name: "IX_CraftingKitPieces_KitId_SortOrder",
                table: "CraftingKitPieces",
                columns: new[] { "KitId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_CraftingKitPieces_PatternId",
                table: "CraftingKitPieces",
                column: "PatternId");

            migrationBuilder.CreateIndex(
                name: "IX_CraftingKits_IsArchived",
                table: "CraftingKits",
                column: "IsArchived");

            migrationBuilder.CreateIndex(
                name: "IX_CraftingKits_Slug",
                table: "CraftingKits",
                column: "Slug");

            migrationBuilder.CreateIndex(
                name: "IX_CraftingKits_ThemeId",
                table: "CraftingKits",
                column: "ThemeId");

            migrationBuilder.CreateIndex(
                name: "IX_CraftingKits_UserId",
                table: "CraftingKits",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CraftingKits_UserId_IsArchived_Name",
                table: "CraftingKits",
                columns: new[] { "UserId", "IsArchived", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_CraftingKitSupplies_KitId",
                table: "CraftingKitSupplies",
                column: "KitId");

            migrationBuilder.CreateIndex(
                name: "IX_CraftingKitSupplies_KitId_SortOrder",
                table: "CraftingKitSupplies",
                columns: new[] { "KitId", "SortOrder" });

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
                name: "IX_CraftingPatternPieces_PatternId",
                table: "CraftingPatternPieces",
                column: "PatternId");

            migrationBuilder.CreateIndex(
                name: "IX_CraftingPatternPieces_PatternId_SortOrder",
                table: "CraftingPatternPieces",
                columns: new[] { "PatternId", "SortOrder" });

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
                name: "IX_CraftingPatterns_UserId_ThemeId_Name",
                table: "CraftingPatterns",
                columns: new[] { "UserId", "ThemeId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_CraftingPatternSteps_PatternPieceId",
                table: "CraftingPatternSteps",
                column: "PatternPieceId");

            migrationBuilder.CreateIndex(
                name: "IX_CraftingPatternSteps_PatternPieceId_SortOrder",
                table: "CraftingPatternSteps",
                columns: new[] { "PatternPieceId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_CraftingProjectInventoryLinks_ProjectId",
                table: "CraftingProjectInventoryLinks",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CraftingProjectInventoryLinks_ProjectId_InventoryItemType_InventoryItemId",
                table: "CraftingProjectInventoryLinks",
                columns: new[] { "ProjectId", "InventoryItemType", "InventoryItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CraftingProjects_IsArchived",
                table: "CraftingProjects",
                column: "IsArchived");

            migrationBuilder.CreateIndex(
                name: "IX_CraftingProjects_KitId",
                table: "CraftingProjects",
                column: "KitId");

            migrationBuilder.CreateIndex(
                name: "IX_CraftingProjects_KitPieceId",
                table: "CraftingProjects",
                column: "KitPieceId",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_CraftingProjects_UserId_IsArchived_Name",
                table: "CraftingProjects",
                columns: new[] { "UserId", "IsArchived", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_CraftingProjects_UserId_PatternId",
                table: "CraftingProjects",
                columns: new[] { "UserId", "PatternId" });

            migrationBuilder.CreateIndex(
                name: "IX_CraftingProjectStepProgress_PatternStepId",
                table: "CraftingProjectStepProgress",
                column: "PatternStepId");

            migrationBuilder.CreateIndex(
                name: "IX_CraftingProjectStepProgress_ProjectId",
                table: "CraftingProjectStepProgress",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CraftingProjectStepProgress_ProjectId_PatternStepId",
                table: "CraftingProjectStepProgress",
                columns: new[] { "ProjectId", "PatternStepId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CraftingProjectTimers_PatternStepId",
                table: "CraftingProjectTimers",
                column: "PatternStepId");

            migrationBuilder.CreateIndex(
                name: "IX_CraftingProjectTimers_ProjectId",
                table: "CraftingProjectTimers",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CraftingProjectTimers_ProjectId_PatternStepId",
                table: "CraftingProjectTimers",
                columns: new[] { "ProjectId", "PatternStepId" },
                unique: true);

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
    }
}
