using TankerMade.Core.Entities;
using TankerMade.Modules.Crafting.DTOs.Inventory;
using TankerMade.Server.Services.Crafting;
using Xunit;

namespace TankerMade.Tests;

public class CraftingInventoryServiceTests
{
    [Fact]
    public async Task CreateOrMergeYarnAsync_merges_by_user_brand_and_color()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new CraftingInventoryService(context);

        var first = await service.CreateOrMergeYarnAsync(new CreateCraftingYarnInventoryItemDto
        {
            BrandName = "Acme Yarn",
            ColorName = "Storm Blue",
            MainColor = "Blue",
            WeightName = "Worsted",
            FiberContent = "100% wool",
            FiberTag = "Natural",
            Skeins = 2,
            EstimatedLength = 440,
            LotNumber = "A1",
            SourceName = "Local Shop",
            Price = 12.50m
        }, user.Id);

        var merged = await service.CreateOrMergeYarnAsync(new CreateCraftingYarnInventoryItemDto
        {
            BrandName = " acme yarn ",
            ColorName = "storm blue",
            Skeins = 3,
            EstimatedLength = 660,
            LotNumber = "A1",
            SourceName = "Local Shop",
            Price = 9.99m,
            IsSalePrice = true
        }, user.Id);

        Assert.Equal(first.Id, merged.Id);
        Assert.Equal(5, merged.TotalSkeins);
        Assert.Equal(1100, merged.EstimatedRemainingLength);
        Assert.Equal(12.50m, merged.RegularPrice);
        Assert.Single(merged.Lots);
        Assert.Equal(5, merged.Lots[0].Skeins);
        Assert.Equal(1100, merged.Lots[0].RemainingLength);
        Assert.Equal(2, merged.Purchases.Count);
        Assert.Contains(merged.Purchases, p => p.IsSalePrice && p.Price == 9.99m);
    }

    [Fact]
    public async Task CreateOrMergeYarnAsync_keeps_users_inventory_separate()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var owner = new User(Guid.NewGuid(), "owner", "owner@example.test", "hash");
        var otherUser = new User(Guid.NewGuid(), "other", "other@example.test", "hash");
        context.Users.AddRange(owner, otherUser);
        await context.SaveChangesAsync();

        var service = new CraftingInventoryService(context);
        var request = new CreateCraftingYarnInventoryItemDto
        {
            BrandName = "Shared Brand",
            ColorName = "Same Color",
            Skeins = 1
        };

        var ownerYarn = await service.CreateOrMergeYarnAsync(request, owner.Id);
        var otherYarn = await service.CreateOrMergeYarnAsync(request, otherUser.Id);
        var ownerList = await service.GetYarnsAsync(owner.Id);
        var otherList = await service.GetYarnsAsync(otherUser.Id);

        Assert.NotEqual(ownerYarn.Id, otherYarn.Id);
        Assert.Single(ownerList);
        Assert.Single(otherList);
        Assert.Equal(ownerYarn.Id, ownerList[0].Id);
        Assert.Equal(otherYarn.Id, otherList[0].Id);
        Assert.Null(await service.GetYarnByIdAsync(ownerYarn.Id, otherUser.Id));
    }

    [Fact]
    public async Task CreateOrMergeYarnAsync_rejects_missing_identity_or_quantity()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new CraftingInventoryService(context);

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateOrMergeYarnAsync(new CreateCraftingYarnInventoryItemDto
        {
            BrandName = "",
            ColorName = "Blue",
            Skeins = 1
        }, user.Id));

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateOrMergeYarnAsync(new CreateCraftingYarnInventoryItemDto
        {
            BrandName = "Acme",
            ColorName = "Blue",
            Skeins = 0
        }, user.Id));
    }

    [Fact]
    public async Task CreateOrMergeToolAsync_merges_by_user_brand_and_type()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new CraftingInventoryService(context);

        var first = await service.CreateOrMergeToolAsync(new CreateCraftingToolInventoryItemDto
        {
            BrandName = "Acme Tools",
            TypeName = "Hook",
            Size = "5 mm",
            Description = "Aluminum hook",
            Quantity = 1,
            SourceName = "Local Shop",
            Price = 7.50m
        }, user.Id);

        var merged = await service.CreateOrMergeToolAsync(new CreateCraftingToolInventoryItemDto
        {
            BrandName = " acme tools ",
            TypeName = "hook",
            Quantity = 2,
            SourceName = "Local Shop",
            Price = 4.99m,
            IsSalePrice = true
        }, user.Id);

        Assert.Equal(first.Id, merged.Id);
        Assert.Equal(3, merged.Quantity);
        Assert.Equal("5 mm", merged.Size);
        Assert.Equal(7.50m, merged.RegularPrice);
        Assert.Equal(2, merged.Purchases.Count);
        Assert.Contains(merged.Purchases, p => p.IsSalePrice && p.Price == 4.99m);
    }

    [Fact]
    public async Task CreateOrMergeNotionAsync_merges_by_user_brand_and_type()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new CraftingInventoryService(context);

        var first = await service.CreateOrMergeNotionAsync(new CreateCraftingNotionInventoryItemDto
        {
            BrandName = "Acme Notions",
            TypeName = "Button",
            Size = "12 mm",
            ColorName = "Black",
            Description = "Matte buttons",
            Quantity = 6,
            SourceName = "Craft Store",
            Price = 3.25m
        }, user.Id);

        var merged = await service.CreateOrMergeNotionAsync(new CreateCraftingNotionInventoryItemDto
        {
            BrandName = "acme notions",
            TypeName = " button ",
            Quantity = 4,
            SourceName = "Craft Store",
            Price = 2.00m,
            IsSalePrice = true
        }, user.Id);

        Assert.Equal(first.Id, merged.Id);
        Assert.Equal(10, merged.Quantity);
        Assert.Equal("12 mm", merged.Size);
        Assert.Equal("Black", merged.ColorName);
        Assert.Equal(3.25m, merged.RegularPrice);
        Assert.Equal(2, merged.Purchases.Count);
        Assert.Contains(merged.Purchases, p => p.IsSalePrice && p.Price == 2.00m);
    }

    [Fact]
    public async Task ToolAndNotion_inventory_is_user_scoped()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var owner = new User(Guid.NewGuid(), "owner", "owner@example.test", "hash");
        var otherUser = new User(Guid.NewGuid(), "other", "other@example.test", "hash");
        context.Users.AddRange(owner, otherUser);
        await context.SaveChangesAsync();

        var service = new CraftingInventoryService(context);

        var ownerTool = await service.CreateOrMergeToolAsync(new CreateCraftingToolInventoryItemDto
        {
            BrandName = "Shared Brand",
            TypeName = "Needle",
            Quantity = 1
        }, owner.Id);
        var otherTool = await service.CreateOrMergeToolAsync(new CreateCraftingToolInventoryItemDto
        {
            BrandName = "Shared Brand",
            TypeName = "Needle",
            Quantity = 1
        }, otherUser.Id);

        var ownerNotion = await service.CreateOrMergeNotionAsync(new CreateCraftingNotionInventoryItemDto
        {
            BrandName = "Shared Brand",
            TypeName = "Marker",
            Quantity = 1
        }, owner.Id);
        var otherNotion = await service.CreateOrMergeNotionAsync(new CreateCraftingNotionInventoryItemDto
        {
            BrandName = "Shared Brand",
            TypeName = "Marker",
            Quantity = 1
        }, otherUser.Id);

        Assert.NotEqual(ownerTool.Id, otherTool.Id);
        Assert.NotEqual(ownerNotion.Id, otherNotion.Id);
        Assert.Null(await service.GetToolByIdAsync(ownerTool.Id, otherUser.Id));
        Assert.Null(await service.GetNotionByIdAsync(ownerNotion.Id, otherUser.Id));
    }

    [Fact]
    public async Task ToolAndNotion_inventory_rejects_missing_identity_or_quantity()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new CraftingInventoryService(context);

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateOrMergeToolAsync(new CreateCraftingToolInventoryItemDto
        {
            BrandName = "",
            TypeName = "Hook",
            Quantity = 1
        }, user.Id));

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateOrMergeNotionAsync(new CreateCraftingNotionInventoryItemDto
        {
            BrandName = "Acme",
            TypeName = "Marker",
            Quantity = 0
        }, user.Id));
    }

    [Fact]
    public async Task GetYarnsAsync_applies_inventory_filters()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new CraftingInventoryService(context);

        var blue = await service.CreateOrMergeYarnAsync(new CreateCraftingYarnInventoryItemDto
        {
            BrandName = "Acme Yarn",
            ColorName = "Storm Blue",
            MainColor = "Blue",
            WeightName = "Worsted",
            FiberContent = "100% wool",
            FiberTag = "Natural",
            Skeins = 2,
            SourceName = "Local Shop"
        }, user.Id);

        await service.CreateOrMergeYarnAsync(new CreateCraftingYarnInventoryItemDto
        {
            BrandName = "Bright Yarn",
            ColorName = "Sunset",
            MainColor = "Orange",
            WeightName = "DK",
            FiberTag = "Synthetic",
            Skeins = 1,
            SourceName = "Online"
        }, user.Id);

        var byBrand = await service.GetYarnsAsync(user.Id, new CraftingYarnInventoryFilterDto
        {
            BrandName = "acme"
        });
        var bySource = await service.GetYarnsAsync(user.Id, new CraftingYarnInventoryFilterDto
        {
            SourceName = "Local"
        });
        var bySearch = await service.GetYarnsAsync(user.Id, new CraftingYarnInventoryFilterDto
        {
            Search = "wool"
        });

        Assert.Single(byBrand);
        Assert.Equal(blue.Id, byBrand[0].Id);
        Assert.Single(bySource);
        Assert.Equal(blue.Id, bySource[0].Id);
        Assert.Single(bySearch);
        Assert.Equal(blue.Id, bySearch[0].Id);
    }

    [Fact]
    public async Task GetToolsAndNotionsAsync_apply_inventory_filters()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new CraftingInventoryService(context);

        var hook = await service.CreateOrMergeToolAsync(new CreateCraftingToolInventoryItemDto
        {
            BrandName = "Acme Tools",
            TypeName = "Hook",
            Size = "5 mm",
            Description = "Aluminum hook",
            Quantity = 1,
            SourceName = "Local Shop"
        }, user.Id);
        await service.CreateOrMergeToolAsync(new CreateCraftingToolInventoryItemDto
        {
            BrandName = "Other Tools",
            TypeName = "Needle",
            Size = "7 mm",
            Quantity = 1,
            SourceName = "Online"
        }, user.Id);

        var button = await service.CreateOrMergeNotionAsync(new CreateCraftingNotionInventoryItemDto
        {
            BrandName = "Acme Notions",
            TypeName = "Button",
            Size = "12 mm",
            ColorName = "Black",
            Description = "Matte buttons",
            Quantity = 6,
            SourceName = "Craft Store"
        }, user.Id);
        await service.CreateOrMergeNotionAsync(new CreateCraftingNotionInventoryItemDto
        {
            BrandName = "Other Notions",
            TypeName = "Marker",
            ColorName = "Green",
            Quantity = 2,
            SourceName = "Online"
        }, user.Id);

        var toolFilter = await service.GetToolsAsync(user.Id, new CraftingToolInventoryFilterDto
        {
            TypeName = "hook",
            SourceName = "Local"
        });
        var notionFilter = await service.GetNotionsAsync(user.Id, new CraftingNotionInventoryFilterDto
        {
            ColorName = "Black",
            Search = "Matte"
        });

        Assert.Single(toolFilter);
        Assert.Equal(hook.Id, toolFilter[0].Id);
        Assert.Single(notionFilter);
        Assert.Equal(button.Id, notionFilter[0].Id);
    }

    [Fact]
    public async Task GetReferenceItemsAsync_returns_seeded_module_reference_values_by_category()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();

        var service = new CraftingInventoryService(context);

        var yarnWeights = await service.GetReferenceItemsAsync("yarn-weight");
        var fiberTags = await service.GetReferenceItemsAsync("fiber-tag");
        var unknown = await service.GetReferenceItemsAsync("missing-category");

        Assert.Contains(yarnWeights, item => item.Name == "Worsted" && item.Slug == "worsted");
        Assert.Equal(yarnWeights.OrderBy(item => item.SortOrder).Select(item => item.Id), yarnWeights.Select(item => item.Id));
        Assert.Contains(fiberTags, item => item.Name == "Natural");
        Assert.Empty(unknown);
    }
}
