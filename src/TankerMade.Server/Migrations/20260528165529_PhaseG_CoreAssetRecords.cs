using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TankerMade.Server.Migrations
{
    /// <inheritdoc />
    public partial class PhaseG_CoreAssetRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CoreAssetRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModuleKey = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    RecordType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    RecordId = table.Column<Guid>(type: "TEXT", nullable: true),
                    OriginalFileName = table.Column<string>(type: "TEXT", maxLength: 260, nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    StorageProvider = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    StoragePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoreAssetRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoreAssetRecords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoreAssetRecords_ModuleKey",
                table: "CoreAssetRecords",
                column: "ModuleKey");

            migrationBuilder.CreateIndex(
                name: "IX_CoreAssetRecords_ModuleKey_RecordType_RecordId",
                table: "CoreAssetRecords",
                columns: new[] { "ModuleKey", "RecordType", "RecordId" });

            migrationBuilder.CreateIndex(
                name: "IX_CoreAssetRecords_StoragePath",
                table: "CoreAssetRecords",
                column: "StoragePath",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoreAssetRecords_UserId",
                table: "CoreAssetRecords",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoreAssetRecords");
        }
    }
}
