using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TankerMade.Server.Data;

#nullable disable

namespace TankerMade.Server.Migrations;

[DbContext(typeof(TankerMadeDbContext))]
[Migration("20260526130000_CraftingProjectArchive")]
public partial class CraftingProjectArchive : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "ArchivedAt",
            table: "CraftingProjects",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "IsArchived",
            table: "CraftingProjects",
            type: "INTEGER",
            nullable: false,
            defaultValue: false);

        migrationBuilder.CreateIndex(
            name: "IX_CraftingProjects_IsArchived",
            table: "CraftingProjects",
            column: "IsArchived");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_CraftingProjects_IsArchived",
            table: "CraftingProjects");

        migrationBuilder.DropColumn(
            name: "ArchivedAt",
            table: "CraftingProjects");

        migrationBuilder.DropColumn(
            name: "IsArchived",
            table: "CraftingProjects");
    }
}
