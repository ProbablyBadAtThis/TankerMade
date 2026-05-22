using TankerMade.Core.Entities;
using TankerMade.Modules.Crafting.DTOs.Patterns;
using TankerMade.Server.Services.Crafting;
using Xunit;

namespace TankerMade.Tests;

public class CraftingPatternServiceTests
{
    [Fact]
    public async Task UpdateAsync_preserves_existing_values_when_fields_are_omitted_or_blank()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new CraftingPatternService(context);
        var created = await service.CreateAsync(new CreateCraftingPatternDto
        {
            Name = "Original Pattern",
            Type = "Knitting",
            Form = "Wearable",
            Difficulty = "Intermediate"
        }, user.Id);

        var updated = await service.UpdateAsync(new UpdateCraftingPatternDto
        {
            Id = created.Id,
            Name = "Renamed Pattern",
            Type = "",
            Form = "   "
        }, user.Id);

        Assert.NotNull(updated);
        Assert.Equal("Renamed Pattern", updated.Name);
        Assert.Equal("renamed-pattern", updated.Slug);
        Assert.Equal("Knitting", updated.Type);
        Assert.Equal("Wearable", updated.Form);
        Assert.Equal("Intermediate", updated.Difficulty);
    }
}
