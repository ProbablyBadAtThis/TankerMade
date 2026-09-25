using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TankerMade.Server.Migrations
{
    /// <inheritdoc />
    public partial class PhaseK7_KnittingColorAndYarnRemaining : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ColorId",
                table: "KnittingProjects",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ColorId",
                table: "KnittingPatterns",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_KnittingProjects_ColorId",
                table: "KnittingProjects",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_KnittingPatterns_ColorId",
                table: "KnittingPatterns",
                column: "ColorId");

            migrationBuilder.AddForeignKey(
                name: "FK_KnittingPatterns_Colors_ColorId",
                table: "KnittingPatterns",
                column: "ColorId",
                principalTable: "Colors",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_KnittingProjects_Colors_ColorId",
                table: "KnittingProjects",
                column: "ColorId",
                principalTable: "Colors",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KnittingPatterns_Colors_ColorId",
                table: "KnittingPatterns");

            migrationBuilder.DropForeignKey(
                name: "FK_KnittingProjects_Colors_ColorId",
                table: "KnittingProjects");

            migrationBuilder.DropIndex(
                name: "IX_KnittingProjects_ColorId",
                table: "KnittingProjects");

            migrationBuilder.DropIndex(
                name: "IX_KnittingPatterns_ColorId",
                table: "KnittingPatterns");

            migrationBuilder.DropColumn(
                name: "ColorId",
                table: "KnittingProjects");

            migrationBuilder.DropColumn(
                name: "ColorId",
                table: "KnittingPatterns");
        }
    }
}
