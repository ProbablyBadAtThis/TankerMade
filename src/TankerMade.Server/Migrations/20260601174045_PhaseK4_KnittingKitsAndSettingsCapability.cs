using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TankerMade.Server.Migrations
{
    /// <inheritdoc />
    public partial class PhaseK4_KnittingKitsAndSettingsCapability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KnittingKits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnittingKits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnittingKits_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KnittingSettingItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnittingSettingItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnittingSettingItems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KnittingKitPieces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    KitId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnittingKitPieces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnittingKitPieces_KnittingKits_KitId",
                        column: x => x.KitId,
                        principalTable: "KnittingKits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KnittingKitPieces_KnittingProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "KnittingProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "KnittingKitSupplies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    KitId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SupplyType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Quantity = table.Column<decimal>(type: "TEXT", nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnittingKitSupplies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnittingKitSupplies_KnittingKits_KitId",
                        column: x => x.KitId,
                        principalTable: "KnittingKits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KnittingKitPieces_KitId",
                table: "KnittingKitPieces",
                column: "KitId");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingKitPieces_KitId_SortOrder",
                table: "KnittingKitPieces",
                columns: new[] { "KitId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_KnittingKitPieces_ProjectId",
                table: "KnittingKitPieces",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingKits_UserId",
                table: "KnittingKits",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingKits_UserId_IsArchived_Name",
                table: "KnittingKits",
                columns: new[] { "UserId", "IsArchived", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_KnittingKitSupplies_KitId",
                table: "KnittingKitSupplies",
                column: "KitId");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingKitSupplies_KitId_SortOrder",
                table: "KnittingKitSupplies",
                columns: new[] { "KitId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_KnittingSettingItems_UserId",
                table: "KnittingSettingItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingSettingItems_UserId_Category",
                table: "KnittingSettingItems",
                columns: new[] { "UserId", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_KnittingSettingItems_UserId_Key",
                table: "KnittingSettingItems",
                columns: new[] { "UserId", "Key" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KnittingKitPieces");

            migrationBuilder.DropTable(
                name: "KnittingKitSupplies");

            migrationBuilder.DropTable(
                name: "KnittingSettingItems");

            migrationBuilder.DropTable(
                name: "KnittingKits");
        }
    }
}
