using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TankerMade.Server.Data;

#nullable disable

namespace TankerMade.Server.Migrations;

[DbContext(typeof(TankerMadeDbContext))]
[Migration("20260527113000_CraftingKitProjectLinks")]
public partial class CraftingKitProjectLinks : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "KitId",
            table: "CraftingProjects",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AddColumn<Guid>(
            name: "KitPieceId",
            table: "CraftingProjects",
            type: "TEXT",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_CraftingProjects_KitId",
            table: "CraftingProjects",
            column: "KitId");

        migrationBuilder.CreateIndex(
            name: "IX_CraftingProjects_KitPieceId",
            table: "CraftingProjects",
            column: "KitPieceId",
            unique: true);

        // SQLite cannot add foreign keys to an existing table without rebuilding it.
        // The optional kit backlinks are still represented in the EF model; this
        // migration keeps the data change lightweight by adding columns and indexes.
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(name: "IX_CraftingProjects_KitId", table: "CraftingProjects");
        migrationBuilder.DropIndex(name: "IX_CraftingProjects_KitPieceId", table: "CraftingProjects");
        migrationBuilder.DropColumn(name: "KitId", table: "CraftingProjects");
        migrationBuilder.DropColumn(name: "KitPieceId", table: "CraftingProjects");
    }
}
