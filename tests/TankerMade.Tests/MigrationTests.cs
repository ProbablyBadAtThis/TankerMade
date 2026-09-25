using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TankerMade.Server.Data;
using Xunit;

namespace TankerMade.Tests;

public class MigrationTests
{
    [Fact]
    public void Migrations_include_module_host_migration()
    {
        var options = new DbContextOptionsBuilder<TankerMadeDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        using var context = new TankerMadeDbContext(options);

        Assert.Contains(
            "20260521210000_ModuleHostAndCraftingReference",
            context.Database.GetMigrations());
        Assert.Contains(
            "20260522140000_CraftingPatternPiecesAndSteps",
            context.Database.GetMigrations());
        Assert.Contains(
            "20260522170000_CraftingProjectStepProgress",
            context.Database.GetMigrations());
        Assert.Contains(
            "20260526143000_CraftingYarnInventory",
            context.Database.GetMigrations());
        Assert.Contains(
            "20260526150000_Printing3DInventory",
            context.Database.GetMigrations());
        Assert.Contains(
            "20260526153000_CraftingToolAndNotionInventory",
            context.Database.GetMigrations());
        Assert.Contains(
            "20260527100000_ModuleInventoryReferenceData",
            context.Database.GetMigrations());
        Assert.Contains(
            "20260527103000_CraftingProjectInventoryLinks",
            context.Database.GetMigrations());
        Assert.Contains(
            "20260527110000_CraftingKits",
            context.Database.GetMigrations());
        Assert.Contains(
            "20260527113000_CraftingKitProjectLinks",
            context.Database.GetMigrations());
    }

    [Fact]
    public async Task Migrations_create_module_host_tables()
    {
        var databasePath = Path.Combine(
            Path.GetTempPath(),
            $"tankermade-migration-test-{Guid.NewGuid():N}.db");

        try
        {
            var options = new DbContextOptionsBuilder<TankerMadeDbContext>()
                .UseSqlite($"Data Source={databasePath}")
                .Options;

            await using var context = new TankerMadeDbContext(options);
            await context.Database.MigrateAsync();

            await using var connection = new SqliteConnection($"Data Source={databasePath}");
            await connection.OpenAsync();

            var tables = new HashSet<string>();
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table'";
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                tables.Add(reader.GetString(0));
            }

            Assert.Contains("ModuleDefinitions", tables);
            Assert.Contains("UserModuleActivations", tables);
            Assert.Contains("KnittingProjects", tables);
            Assert.Contains("KnittingPatterns", tables);
            Assert.Contains("KnittingPatternPieces", tables);
            Assert.Contains("KnittingPatternSteps", tables);
            Assert.Contains("KnittingProjectStepProgress", tables);
            Assert.Contains("KnittingProjectRowChecks", tables);
            Assert.Contains("CoreCommissionPublications", tables);
            Assert.Contains("CoreCommissionRevisions", tables);
            Assert.Contains("CoreCommissionWorkspaces", tables);
            Assert.Contains("KnittingYarnInventoryItems", tables);
            Assert.Contains("KnittingKits", tables);
            Assert.Contains("PrintingMaterialInventoryItems", tables);
            Assert.Contains("PrintingSpools", tables);
            Assert.Contains("PrintingInventoryPurchases", tables);
            Assert.Contains("PrintingInventoryReferenceItems", tables);
            Assert.DoesNotContain("CraftingProjects", tables);
            Assert.DoesNotContain("CraftingPatterns", tables);
            Assert.DoesNotContain("CraftingKits", tables);
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            if (File.Exists(databasePath))
            {
                File.Delete(databasePath);
            }
        }
    }
}
