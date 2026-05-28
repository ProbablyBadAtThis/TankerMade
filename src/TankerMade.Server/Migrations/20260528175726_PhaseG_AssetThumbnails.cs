using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TankerMade.Server.Migrations
{
    /// <inheritdoc />
    public partial class PhaseG_AssetThumbnails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CoreAssetThumbnails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AssetRecordId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SizeKey = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Width = table.Column<int>(type: "INTEGER", nullable: false),
                    Height = table.Column<int>(type: "INTEGER", nullable: false),
                    StorageProvider = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    StoragePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoreAssetThumbnails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoreAssetThumbnails_CoreAssetRecords_AssetRecordId",
                        column: x => x.AssetRecordId,
                        principalTable: "CoreAssetRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoreAssetThumbnails_AssetRecordId",
                table: "CoreAssetThumbnails",
                column: "AssetRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_CoreAssetThumbnails_AssetRecordId_SizeKey",
                table: "CoreAssetThumbnails",
                columns: new[] { "AssetRecordId", "SizeKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoreAssetThumbnails_StoragePath",
                table: "CoreAssetThumbnails",
                column: "StoragePath",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoreAssetThumbnails");
        }
    }
}
