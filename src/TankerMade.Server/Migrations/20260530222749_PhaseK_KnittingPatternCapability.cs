using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TankerMade.Server.Migrations
{
    /// <inheritdoc />
    public partial class PhaseK_KnittingPatternCapability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KnittingPatterns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 220, nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Form = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Difficulty = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ThemeId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SourceId = table.Column<Guid>(type: "TEXT", nullable: true),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnittingPatterns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnittingPatterns_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_KnittingPatterns_Themes_ThemeId",
                        column: x => x.ThemeId,
                        principalTable: "Themes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_KnittingPatterns_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KnittingPatternPieces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PatternId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnittingPatternPieces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnittingPatternPieces_KnittingPatterns_PatternId",
                        column: x => x.PatternId,
                        principalTable: "KnittingPatterns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KnittingPatternSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PatternPieceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RangeStart = table.Column<int>(type: "INTEGER", nullable: true),
                    RangeEnd = table.Column<int>(type: "INTEGER", nullable: true),
                    Label = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Instructions = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnittingPatternSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnittingPatternSteps_KnittingPatternPieces_PatternPieceId",
                        column: x => x.PatternPieceId,
                        principalTable: "KnittingPatternPieces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KnittingPatternPieces_PatternId",
                table: "KnittingPatternPieces",
                column: "PatternId");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingPatternPieces_PatternId_SortOrder",
                table: "KnittingPatternPieces",
                columns: new[] { "PatternId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_KnittingPatterns_Slug",
                table: "KnittingPatterns",
                column: "Slug");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingPatterns_SourceId",
                table: "KnittingPatterns",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingPatterns_ThemeId",
                table: "KnittingPatterns",
                column: "ThemeId");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingPatterns_UserId",
                table: "KnittingPatterns",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingPatterns_UserId_Name",
                table: "KnittingPatterns",
                columns: new[] { "UserId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_KnittingPatternSteps_PatternPieceId",
                table: "KnittingPatternSteps",
                column: "PatternPieceId");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingPatternSteps_PatternPieceId_SortOrder",
                table: "KnittingPatternSteps",
                columns: new[] { "PatternPieceId", "SortOrder" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KnittingPatternSteps");

            migrationBuilder.DropTable(
                name: "KnittingPatternPieces");

            migrationBuilder.DropTable(
                name: "KnittingPatterns");
        }
    }
}
