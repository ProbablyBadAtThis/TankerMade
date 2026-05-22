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
            Assert.Contains("CraftingProjects", tables);
            Assert.Contains("CraftingPatterns", tables);
            Assert.Contains("CraftingPatternPieces", tables);
            Assert.Contains("CraftingPatternSteps", tables);
        }
        finally
        {
            File.Delete(databasePath);
        }
    }
}
