using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TankerMade.Server.Migrations
{
    /// <inheritdoc />
    public partial class PhaseL_CoreRecentWorkAccess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CoreUserRecentWorkAccesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModuleKey = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    WorkItemType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    WorkItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LastAccessedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoreUserRecentWorkAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoreUserRecentWorkAccesses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoreUserRecentWorkAccesses_UserId_LastAccessedAtUtc",
                table: "CoreUserRecentWorkAccesses",
                columns: new[] { "UserId", "LastAccessedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_CoreUserRecentWorkAccesses_UserId_ModuleKey_WorkItemType_WorkItemId",
                table: "CoreUserRecentWorkAccesses",
                columns: new[] { "UserId", "ModuleKey", "WorkItemType", "WorkItemId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoreUserRecentWorkAccesses");
        }
    }
}
