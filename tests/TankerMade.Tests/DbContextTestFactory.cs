using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TankerMade.Server.Data;

namespace TankerMade.Tests;

internal sealed class DbContextTestFactory : IDisposable
{
    private readonly SqliteConnection _connection;

    public DbContextTestFactory()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
    }

    public TankerMadeDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TankerMadeDbContext>()
            .UseSqlite(_connection)
            .Options;

        var context = new TankerMadeDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
