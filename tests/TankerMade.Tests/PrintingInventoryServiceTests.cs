using TankerMade.Core.Entities;
using TankerMade.Modules.Printing3D.DTOs.Inventory;
using TankerMade.Server.Services.Printing3D;
using Xunit;

namespace TankerMade.Tests;

public class PrintingInventoryServiceTests
{
    [Fact]
    public async Task CreateOrMergeMaterialAsync_merges_by_user_material_brand_and_color()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new PrintingInventoryService(context);

        var first = await service.CreateOrMergeMaterialAsync(new CreatePrintingMaterialInventoryItemDto
        {
            MaterialType = "PLA",
            BrandName = "PrintCo",
            ColorName = "Matte Black",
            SpoolWeightGrams = 1000,
            RemainingWeightGrams = 1000,
            Diameter = "1.75mm",
            StorageLocation = "Dry box",
            SpoolCode = "PLA-BLK-001",
            PrinterCompatibility = "MK4",
            SourceName = "Filament Store",
            Price = 24.99m
        }, user.Id);

        var merged = await service.CreateOrMergeMaterialAsync(new CreatePrintingMaterialInventoryItemDto
        {
            MaterialType = " pla ",
            BrandName = "printco",
            ColorName = "matte black",
            SpoolWeightGrams = 1000,
            RemainingWeightGrams = 850,
            SpoolCode = "PLA-BLK-001",
            SourceName = "Filament Store",
            Price = 18.99m,
            IsSalePrice = true
        }, user.Id);

        Assert.Equal(first.Id, merged.Id);
        Assert.Equal(2000, merged.TotalSpoolWeightGrams);
        Assert.Equal(1850, merged.RemainingWeightGrams);
        Assert.Equal(24.99m, merged.RegularPrice);
        Assert.Single(merged.Spools);
        Assert.Equal(2000, merged.Spools[0].StartingWeightGrams);
        Assert.Equal(1850, merged.Spools[0].RemainingWeightGrams);
        Assert.Equal(2, merged.Purchases.Count);
        Assert.Contains(merged.Purchases, p => p.IsSalePrice && p.Price == 18.99m);
    }

    [Fact]
    public async Task CreateOrMergeMaterialAsync_keeps_users_inventory_separate()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var owner = new User(Guid.NewGuid(), "owner", "owner@example.test", "hash");
        var otherUser = new User(Guid.NewGuid(), "other", "other@example.test", "hash");
        context.Users.AddRange(owner, otherUser);
        await context.SaveChangesAsync();

        var service = new PrintingInventoryService(context);
        var request = new CreatePrintingMaterialInventoryItemDto
        {
            MaterialType = "PETG",
            BrandName = "Shared Brand",
            ColorName = "Clear",
            SpoolWeightGrams = 1000
        };

        var ownerMaterial = await service.CreateOrMergeMaterialAsync(request, owner.Id);
        var otherMaterial = await service.CreateOrMergeMaterialAsync(request, otherUser.Id);
        var ownerList = await service.GetMaterialsAsync(owner.Id);
        var otherList = await service.GetMaterialsAsync(otherUser.Id);

        Assert.NotEqual(ownerMaterial.Id, otherMaterial.Id);
        Assert.Single(ownerList);
        Assert.Single(otherList);
        Assert.Equal(ownerMaterial.Id, ownerList[0].Id);
        Assert.Equal(otherMaterial.Id, otherList[0].Id);
        Assert.Null(await service.GetMaterialByIdAsync(ownerMaterial.Id, otherUser.Id));
    }

    [Fact]
    public async Task CreateOrMergeMaterialAsync_rejects_missing_identity_or_quantity()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new PrintingInventoryService(context);

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateOrMergeMaterialAsync(new CreatePrintingMaterialInventoryItemDto
        {
            MaterialType = "",
            BrandName = "PrintCo",
            ColorName = "Black",
            SpoolWeightGrams = 1000
        }, user.Id));

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateOrMergeMaterialAsync(new CreatePrintingMaterialInventoryItemDto
        {
            MaterialType = "PLA",
            BrandName = "PrintCo",
            ColorName = "Black",
            SpoolWeightGrams = 0
        }, user.Id));
    }

    [Fact]
    public async Task GetMaterialsAsync_applies_inventory_filters()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new PrintingInventoryService(context);

        var pla = await service.CreateOrMergeMaterialAsync(new CreatePrintingMaterialInventoryItemDto
        {
            MaterialType = "PLA",
            BrandName = "PrintCo",
            ColorName = "Matte Black",
            SpoolWeightGrams = 1000,
            RemainingWeightGrams = 900,
            Diameter = "1.75mm",
            StorageLocation = "Dry box",
            SourceName = "Filament Store"
        }, user.Id);

        await service.CreateOrMergeMaterialAsync(new CreatePrintingMaterialInventoryItemDto
        {
            MaterialType = "PETG",
            BrandName = "Other Brand",
            ColorName = "Clear",
            SpoolWeightGrams = 1000,
            Diameter = "2.85mm",
            StorageLocation = "Shelf",
            SourceName = "Online"
        }, user.Id);

        var byMaterial = await service.GetMaterialsAsync(user.Id, new PrintingMaterialInventoryFilterDto
        {
            MaterialType = "pla"
        });
        var bySource = await service.GetMaterialsAsync(user.Id, new PrintingMaterialInventoryFilterDto
        {
            SourceName = "Filament"
        });
        var bySearch = await service.GetMaterialsAsync(user.Id, new PrintingMaterialInventoryFilterDto
        {
            Search = "Dry"
        });

        Assert.Single(byMaterial);
        Assert.Equal(pla.Id, byMaterial[0].Id);
        Assert.Single(bySource);
        Assert.Equal(pla.Id, bySource[0].Id);
        Assert.Single(bySearch);
        Assert.Equal(pla.Id, bySearch[0].Id);
    }

    [Fact]
    public async Task GetReferenceItemsAsync_returns_seeded_module_reference_values_by_category()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();

        var service = new PrintingInventoryService(context);

        var materialTypes = await service.GetReferenceItemsAsync("material-type");
        var tooling = await service.GetReferenceItemsAsync("printer-tooling");
        var unknown = await service.GetReferenceItemsAsync("missing-category");

        Assert.Contains(materialTypes, item => item.Name == "PLA" && item.Slug == "pla");
        Assert.Equal(materialTypes.OrderBy(item => item.SortOrder).Select(item => item.Id), materialTypes.Select(item => item.Id));
        Assert.Contains(tooling, item => item.Name == "Nozzle");
        Assert.Empty(unknown);
    }
}
