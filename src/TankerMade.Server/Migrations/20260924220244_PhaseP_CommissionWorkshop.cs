using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TankerMade.Server.Migrations
{
    /// <inheritdoc />
    public partial class PhaseP_CommissionWorkshop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TargetHourlyRate",
                table: "Users",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HostedAt",
                table: "CoreCommissionPublications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CoreCommissionWorkspaces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModuleKey = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    QuotePrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    DueDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    DepositReceived = table.Column<bool>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoreCommissionWorkspaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoreCommissionWorkspaces_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoreCommissionWorkspaces_UserId_ModuleKey_ProjectId",
                table: "CoreCommissionWorkspaces",
                columns: new[] { "UserId", "ModuleKey", "ProjectId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoreCommissionWorkspaces");

            migrationBuilder.DropColumn(
                name: "TargetHourlyRate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HostedAt",
                table: "CoreCommissionPublications");
        }
    }
}
