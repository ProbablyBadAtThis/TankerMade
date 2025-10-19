using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TankerMade.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Colors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Themes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Themes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Patterns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(220)", maxLength: 220, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Form = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Difficulty = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PatternId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ThemeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patterns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Patterns_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Patterns_Themes_ThemeId",
                        column: x => x.ThemeId,
                        principalTable: "Themes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Patterns_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(220)", maxLength: 220, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    PatternId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ThemeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Difficulty = table.Column<int>(type: "int", nullable: false),
                    Progress = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Patterns_PatternId",
                        column: x => x.PatternId,
                        principalTable: "Patterns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Projects_Themes_ThemeId",
                        column: x => x.ThemeId,
                        principalTable: "Themes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Projects_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Brands",
                columns: new[] { "Id", "CreatedAt", "Name", "Slug" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444441"), new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Red Heart", "red-heart" },
                    { new Guid("44444444-4444-4444-4444-444444444442"), new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Lion Brand", "lion-brand" },
                    { new Guid("44444444-4444-4444-4444-444444444443"), new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Bernat", "bernat" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Caron", "caron" }
                });

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "Id", "CreatedAt", "Name", "Slug" },
                values: new object[,]
                {
                    { new Guid("22222222-2222-2222-2222-222222222221"), new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Red", "red" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Blue", "blue" },
                    { new Guid("22222222-2222-2222-2222-222222222223"), new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Green", "green" },
                    { new Guid("22222222-2222-2222-2222-222222222224"), new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Yellow", "yellow" },
                    { new Guid("22222222-2222-2222-2222-222222222225"), new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Black", "black" },
                    { new Guid("22222222-2222-2222-2222-222222222226"), new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "White", "white" }
                });

            migrationBuilder.InsertData(
                table: "Sources",
                columns: new[] { "Id", "CreatedAt", "Name", "Slug" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333331"), new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Ravelry", "ravelry" },
                    { new Guid("33333333-3333-3333-3333-333333333332"), new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Etsy", "etsy" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "YouTube", "youtube" },
                    { new Guid("33333333-3333-3333-3333-333333333334"), new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Book", "book" },
                    { new Guid("33333333-3333-3333-3333-333333333335"), new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Custom", "custom" }
                });

            migrationBuilder.InsertData(
                table: "Themes",
                columns: new[] { "Id", "CreatedAt", "Name", "Slug" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Animals", "animals" },
                    { new Guid("11111111-1111-1111-1111-111111111112"), new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Baby", "baby" },
                    { new Guid("11111111-1111-1111-1111-111111111113"), new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Home Decor", "home-decor" },
                    { new Guid("11111111-1111-1111-1111-111111111114"), new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Toys", "toys" },
                    { new Guid("11111111-1111-1111-1111-111111111115"), new DateTime(2025, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Clothing", "clothing" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Brands_Slug",
                table: "Brands",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Colors_Slug",
                table: "Colors",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patterns_Slug",
                table: "Patterns",
                column: "Slug");

            migrationBuilder.CreateIndex(
                name: "IX_Patterns_SourceId",
                table: "Patterns",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Patterns_ThemeId",
                table: "Patterns",
                column: "ThemeId");

            migrationBuilder.CreateIndex(
                name: "IX_Patterns_UserId",
                table: "Patterns",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_PatternId",
                table: "Projects",
                column: "PatternId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Slug",
                table: "Projects",
                column: "Slug");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ThemeId",
                table: "Projects",
                column: "ThemeId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_UserId",
                table: "Projects",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Sources_Slug",
                table: "Sources",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Themes_Slug",
                table: "Themes",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropTable(
                name: "Colors");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Patterns");

            migrationBuilder.DropTable(
                name: "Sources");

            migrationBuilder.DropTable(
                name: "Themes");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
