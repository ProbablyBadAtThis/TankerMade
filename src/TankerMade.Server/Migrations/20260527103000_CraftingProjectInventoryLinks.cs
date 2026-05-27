using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TankerMade.Server.Data;

#nullable disable

namespace TankerMade.Server.Migrations;

[DbContext(typeof(TankerMadeDbContext))]
[Migration("20260527103000_CraftingProjectInventoryLinks")]
public partial class CraftingProjectInventoryLinks : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "CraftingProjectInventoryLinks",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                InventoryItemType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                InventoryItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                QuantityPlanned = table.Column<decimal>(type: "TEXT", nullable: true),
                Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
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

        migrationBuilder.CreateIndex(
            name: "IX_CraftingProjectInventoryLinks_ProjectId",
            table: "CraftingProjectInventoryLinks",
            column: "ProjectId");

        migrationBuilder.CreateIndex(
            name: "IX_CraftingProjectInventoryLinks_ProjectId_InventoryItemType_InventoryItemId",
            table: "CraftingProjectInventoryLinks",
            columns: new[] { "ProjectId", "InventoryItemType", "InventoryItemId" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "CraftingProjectInventoryLinks");
    }
}
