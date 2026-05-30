using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TankerMade.Server.Data;

#nullable disable

namespace TankerMade.Server.Migrations
{
    [DbContext(typeof(TankerMadeDbContext))]
    [Migration("20260530172000_PhaseJ_LiveModulesSeedSwap")]
    public partial class PhaseJ_LiveModulesSeedSwap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DELETE FROM ModuleDefinitions
                WHERE Id = '55555555-5555-5555-5555-555555555551';
                """);

            migrationBuilder.Sql(
                """
                UPDATE ModuleDefinitions
                SET Description = 'Live module for 3D printing workflows and data.'
                WHERE Id = '55555555-5555-5555-5555-555555555552';
                """);

            migrationBuilder.Sql(
                """
                INSERT INTO ModuleDefinitions (Id, CreatedAt, Description, IsBundled, ModuleKey, Name, Version)
                VALUES
                ('55555555-5555-5555-5555-555555555553', '2025-10-18 00:00:00Z', 'Live module for crochet workflows and data.', 1, 'crochet', 'Crochet', '0.1.0'),
                ('55555555-5555-5555-5555-555555555554', '2025-10-18 00:00:00Z', 'Live module for embroidery workflows and data.', 1, 'embroidery', 'Embroidery', '0.1.0'),
                ('55555555-5555-5555-5555-555555555555', '2025-10-18 00:00:00Z', 'Live module for knitting workflows and data.', 1, 'knitting', 'Knitting', '0.1.0'),
                ('55555555-5555-5555-5555-555555555556', '2025-10-18 00:00:00Z', 'Live module for quilting workflows and data.', 1, 'quilting', 'Quilting', '0.1.0'),
                ('55555555-5555-5555-5555-555555555557', '2025-10-18 00:00:00Z', 'Live module for sewing workflows and data.', 1, 'sewing', 'Sewing', '0.1.0');
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DELETE FROM ModuleDefinitions
                WHERE Id IN (
                    '55555555-5555-5555-5555-555555555553',
                    '55555555-5555-5555-5555-555555555554',
                    '55555555-5555-5555-5555-555555555555',
                    '55555555-5555-5555-5555-555555555556',
                    '55555555-5555-5555-5555-555555555557'
                );
                """);

            migrationBuilder.Sql(
                """
                UPDATE ModuleDefinitions
                SET Description = 'Reference maker module for 3D printing inventory and workflow proofs.'
                WHERE Id = '55555555-5555-5555-5555-555555555552';
                """);

            migrationBuilder.Sql(
                """
                INSERT INTO ModuleDefinitions (Id, CreatedAt, Description, IsBundled, ModuleKey, Name, Version)
                VALUES ('55555555-5555-5555-5555-555555555551', '2025-10-18 00:00:00Z', 'Reference maker module for pattern-based crafting workflows.', 1, 'crafting', 'Crafting', '0.1.0');
                """);
        }
    }
}
