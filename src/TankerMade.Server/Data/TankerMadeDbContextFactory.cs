using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TankerMade.Server.Data;

/// <summary>
/// Allows dotnet-ef tools to instantiate TankerMadeDbContext at design time
/// (i.e. when running migrations) without needing the full DI container.
/// </summary>
public class TankerMadeDbContextFactory : IDesignTimeDbContextFactory<TankerMadeDbContext>
{
    public TankerMadeDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TankerMadeDbContext>();
        optionsBuilder.UseSqlite("Data Source=App_Data/tankermade.db");

        return new TankerMadeDbContext(optionsBuilder.Options);
    }
}
