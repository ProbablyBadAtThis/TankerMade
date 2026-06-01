using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TankerMade.Server.Migrations
{
    /// <inheritdoc />
    public partial class PhaseK2_KnittingProjectCapability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KnittingProjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 220, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    PatternId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ThemeId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Difficulty = table.Column<int>(type: "INTEGER", nullable: false),
                    Progress = table.Column<int>(type: "INTEGER", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnittingProjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnittingProjects_KnittingPatterns_PatternId",
                        column: x => x.PatternId,
                        principalTable: "KnittingPatterns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_KnittingProjects_Themes_ThemeId",
                        column: x => x.ThemeId,
                        principalTable: "Themes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_KnittingProjects_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KnittingProjects_IsArchived",
                table: "KnittingProjects",
                column: "IsArchived");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingProjects_PatternId",
                table: "KnittingProjects",
                column: "PatternId");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingProjects_Slug",
                table: "KnittingProjects",
                column: "Slug");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingProjects_ThemeId",
                table: "KnittingProjects",
                column: "ThemeId");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingProjects_UserId",
                table: "KnittingProjects",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingProjects_UserId_IsArchived_Name",
                table: "KnittingProjects",
                columns: new[] { "UserId", "IsArchived", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_KnittingProjects_UserId_PatternId",
                table: "KnittingProjects",
                columns: new[] { "UserId", "PatternId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KnittingProjects");
        }
    }
}
