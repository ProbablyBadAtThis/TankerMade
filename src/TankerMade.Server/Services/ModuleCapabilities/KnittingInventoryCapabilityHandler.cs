using TankerMade.Contracts.DTOs.ModuleInventory;
using TankerMade.Contracts.Services.ModuleCapabilities;
using TankerMade.Modules.Knitting;
using TankerMade.Modules.Knitting.DTOs.Inventory;
using TankerMade.Modules.Knitting.Services;

namespace TankerMade.Server.Services.ModuleCapabilities;

public class KnittingInventoryCapabilityHandler : IModuleCraftInventoryCapabilityHandler
{
    private readonly IKnittingInventoryService _service;

    public KnittingInventoryCapabilityHandler(IKnittingInventoryService service)
    {
        _service = service;
    }

    public string ModuleKey => KnittingModule.ModuleKey;

    public async Task<IReadOnlyList<ModuleSupplyItemDto>> GetSuppliesAsync(Guid userId, string? search = null, string? category = null)
        => (await _service.GetSuppliesAsync(userId, search, category)).Select(MapLegacySupply).ToList();

    public async Task<ModuleSupplyItemDto> CreateSupplyAsync(CreateModuleSupplyItemRequest request, Guid userId)
        => MapLegacySupply(await _service.CreateSupplyAsync(new CreateKnittingSupplyItemDto
        {
            Category = request.Category,
            Name = request.Name,
            Brand = request.Brand,
            Color = request.Color,
            Size = request.Size,
            Quantity = request.Quantity,
            Unit = request.Unit,
            Notes = request.Notes
        }, userId));

    public async Task<ModuleSupplyItemDto?> UpdateSupplyAsync(Guid supplyId, UpdateModuleSupplyItemRequest request, Guid userId)
    {
        var supply = await _service.UpdateSupplyAsync(supplyId, new CreateKnittingSupplyItemDto
        {
            Category = request.Category,
            Name = request.Name,
            Brand = request.Brand,
            Color = request.Color,
            Size = request.Size,
            Quantity = request.Quantity,
            Unit = request.Unit,
            Notes = request.Notes
        }, userId);

        return supply == null ? null : MapLegacySupply(supply);
    }

    public Task<bool> DeleteSupplyAsync(Guid supplyId, Guid userId)
        => _service.DeleteSupplyAsync(supplyId, userId);

    public async Task<IReadOnlyList<ModuleYarnInventoryItemDto>> GetYarnsAsync(Guid userId, ModuleYarnInventoryFilterRequest? filter = null)
        => (await _service.GetYarnsAsync(userId, MapYarnFilter(filter))).Select(MapYarn).ToList();

    public async Task<ModuleYarnInventoryItemDto> CreateOrMergeYarnAsync(CreateModuleYarnInventoryItemDto request, Guid userId)
        => MapYarn(await _service.CreateOrMergeYarnAsync(MapCreateYarn(request), userId));

    public async Task<ModuleYarnInventoryItemDto?> GetYarnByIdAsync(Guid id, Guid userId)
    {
        var yarn = await _service.GetYarnByIdAsync(id, userId);
        return yarn == null ? null : MapYarn(yarn);
    }

    public async Task<ModuleYarnInventoryItemDto?> UpdateYarnRemainingAsync(
        Guid id,
        UpdateModuleYarnRemainingRequest request,
        Guid userId)
    {
        var yarn = await _service.UpdateYarnRemainingAsync(id, new UpdateKnittingYarnRemainingDto
        {
            EstimatedRemainingLength = request.EstimatedRemainingLength,
            LengthUnit = request.LengthUnit,
            CurrentWeight = request.CurrentWeight
        }, userId);

        return yarn == null ? null : MapYarn(yarn);
    }

    public async Task<ModuleYarnInventoryItemDto?> UpdateYarnLotRemainingAsync(
        Guid yarnId,
        Guid lotId,
        UpdateModuleYarnLotRemainingRequest request,
        Guid userId)
    {
        var yarn = await _service.UpdateYarnLotRemainingAsync(yarnId, lotId, new UpdateKnittingYarnLotRemainingDto
        {
            RemainingLength = request.RemainingLength,
            CurrentWeight = request.CurrentWeight
        }, userId);

        return yarn == null ? null : MapYarn(yarn);
    }

    public async Task<IReadOnlyList<ModuleToolInventoryItemDto>> GetToolsAsync(Guid userId, ModuleToolInventoryFilterRequest? filter = null)
        => (await _service.GetToolsAsync(userId, MapToolFilter(filter))).Select(MapTool).ToList();

    public async Task<ModuleToolInventoryItemDto> CreateOrMergeToolAsync(CreateModuleToolInventoryItemDto request, Guid userId)
        => MapTool(await _service.CreateOrMergeToolAsync(MapCreateTool(request), userId));

    public async Task<ModuleToolInventoryItemDto?> GetToolByIdAsync(Guid id, Guid userId)
    {
        var tool = await _service.GetToolByIdAsync(id, userId);
        return tool == null ? null : MapTool(tool);
    }

    public async Task<IReadOnlyList<ModuleNotionInventoryItemDto>> GetNotionsAsync(Guid userId, ModuleNotionInventoryFilterRequest? filter = null)
        => (await _service.GetNotionsAsync(userId, MapNotionFilter(filter))).Select(MapNotion).ToList();

    public async Task<ModuleNotionInventoryItemDto> CreateOrMergeNotionAsync(CreateModuleNotionInventoryItemDto request, Guid userId)
        => MapNotion(await _service.CreateOrMergeNotionAsync(MapCreateNotion(request), userId));

    public async Task<ModuleNotionInventoryItemDto?> GetNotionByIdAsync(Guid id, Guid userId)
    {
        var notion = await _service.GetNotionByIdAsync(id, userId);
        return notion == null ? null : MapNotion(notion);
    }

    public async Task<IReadOnlyList<ModuleInventoryReferenceItemDto>> GetReferenceItemsAsync(string category)
        => (await _service.GetReferenceItemsAsync(category)).Select(MapReference).ToList();

    public async Task<ModuleInventoryReferenceItemDto> CreateReferenceItemAsync(string category, CreateModuleInventoryReferenceItemDto request)
        => MapReference(await _service.CreateReferenceItemAsync(category, new CreateKnittingInventoryReferenceItemDto { Name = request.Name }));

    private static KnittingYarnInventoryFilterDto? MapYarnFilter(ModuleYarnInventoryFilterRequest? filter)
    {
        if (filter == null)
        {
            return null;
        }

        return new KnittingYarnInventoryFilterDto
        {
            Search = filter.Search,
            BrandName = filter.BrandName,
            ColorName = filter.ColorName,
            MainColor = filter.MainColor,
            WeightName = filter.WeightName,
            FiberTag = filter.FiberTag,
            SourceName = filter.SourceName,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    private static KnittingToolInventoryFilterDto? MapToolFilter(ModuleToolInventoryFilterRequest? filter)
    {
        if (filter == null)
        {
            return null;
        }

        return new KnittingToolInventoryFilterDto
        {
            Search = filter.Search,
            BrandName = filter.BrandName,
            TypeName = filter.TypeName,
            Size = filter.Size,
            SourceName = filter.SourceName,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    private static KnittingNotionInventoryFilterDto? MapNotionFilter(ModuleNotionInventoryFilterRequest? filter)
    {
        if (filter == null)
        {
            return null;
        }

        return new KnittingNotionInventoryFilterDto
        {
            Search = filter.Search,
            BrandName = filter.BrandName,
            TypeName = filter.TypeName,
            Size = filter.Size,
            ColorName = filter.ColorName,
            SourceName = filter.SourceName,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    private static CreateKnittingYarnInventoryItemDto MapCreateYarn(CreateModuleYarnInventoryItemDto request)
        => new()
        {
            BrandName = request.BrandName,
            ColorName = request.ColorName,
            MainColor = request.MainColor,
            WeightName = request.WeightName,
            FiberContent = request.FiberContent,
            FiberTag = request.FiberTag,
            Skeins = request.Skeins,
            EstimatedLength = request.EstimatedLength,
            LengthUnit = request.LengthUnit,
            CurrentWeight = request.CurrentWeight,
            LotNumber = request.LotNumber,
            SourceName = request.SourceName,
            Price = request.Price,
            IsSalePrice = request.IsSalePrice,
            PurchasedAt = request.PurchasedAt
        };

    private static CreateKnittingToolInventoryItemDto MapCreateTool(CreateModuleToolInventoryItemDto request)
        => new()
        {
            BrandName = request.BrandName,
            TypeName = request.TypeName,
            Size = request.Size,
            Description = request.Description,
            Quantity = request.Quantity,
            SourceName = request.SourceName,
            Price = request.Price,
            IsSalePrice = request.IsSalePrice,
            PurchasedAt = request.PurchasedAt
        };

    private static CreateKnittingNotionInventoryItemDto MapCreateNotion(CreateModuleNotionInventoryItemDto request)
        => new()
        {
            BrandName = request.BrandName,
            TypeName = request.TypeName,
            Size = request.Size,
            ColorName = request.ColorName,
            Description = request.Description,
            Quantity = request.Quantity,
            SourceName = request.SourceName,
            Price = request.Price,
            IsSalePrice = request.IsSalePrice,
            PurchasedAt = request.PurchasedAt
        };

    private static ModuleSupplyItemDto MapLegacySupply(KnittingSupplyItemDto source)
        => new()
        {
            Id = source.Id,
            Category = source.Category,
            Name = source.Name,
            Brand = source.Brand,
            Color = source.Color,
            Size = source.Size,
            Quantity = source.Quantity,
            Unit = source.Unit,
            Notes = source.Notes,
            UserId = source.UserId,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };

    private static ModuleYarnInventoryItemDto MapYarn(KnittingYarnInventoryItemDto source)
        => new()
        {
            Id = source.Id,
            UserId = source.UserId,
            BrandName = source.BrandName,
            ColorName = source.ColorName,
            MainColor = source.MainColor,
            WeightName = source.WeightName,
            FiberContent = source.FiberContent,
            FiberTag = source.FiberTag,
            TotalSkeins = source.TotalSkeins,
            EstimatedRemainingLength = source.EstimatedRemainingLength,
            LengthUnit = source.LengthUnit,
            CurrentWeight = source.CurrentWeight,
            RegularPrice = source.RegularPrice,
            Lots = source.Lots.Select(lot => new ModuleYarnLotDto
            {
                Id = lot.Id,
                YarnInventoryItemId = lot.YarnInventoryItemId,
                LotNumber = lot.LotNumber,
                Skeins = lot.Skeins,
                RemainingLength = lot.RemainingLength,
                CurrentWeight = lot.CurrentWeight,
                CreatedAt = lot.CreatedAt,
                UpdatedAt = lot.UpdatedAt
            }).ToList(),
            Purchases = source.Purchases.Select(purchase => new ModuleYarnPurchaseDto
            {
                Id = purchase.Id,
                YarnInventoryItemId = purchase.YarnInventoryItemId,
                SourceName = purchase.SourceName,
                Price = purchase.Price,
                IsSalePrice = purchase.IsSalePrice,
                PurchasedAt = purchase.PurchasedAt,
                CreatedAt = purchase.CreatedAt
            }).ToList(),
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };

    private static ModuleToolInventoryItemDto MapTool(KnittingToolInventoryItemDto source)
        => new()
        {
            Id = source.Id,
            UserId = source.UserId,
            BrandName = source.BrandName,
            TypeName = source.TypeName,
            Size = source.Size,
            Description = source.Description,
            Quantity = source.Quantity,
            RegularPrice = source.RegularPrice,
            Purchases = source.Purchases.Select(purchase => new ModuleToolPurchaseDto
            {
                Id = purchase.Id,
                ToolInventoryItemId = purchase.ToolInventoryItemId,
                SourceName = purchase.SourceName,
                Price = purchase.Price,
                IsSalePrice = purchase.IsSalePrice,
                PurchasedAt = purchase.PurchasedAt,
                CreatedAt = purchase.CreatedAt
            }).ToList(),
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };

    private static ModuleNotionInventoryItemDto MapNotion(KnittingNotionInventoryItemDto source)
        => new()
        {
            Id = source.Id,
            UserId = source.UserId,
            BrandName = source.BrandName,
            TypeName = source.TypeName,
            Size = source.Size,
            ColorName = source.ColorName,
            Description = source.Description,
            Quantity = source.Quantity,
            RegularPrice = source.RegularPrice,
            Purchases = source.Purchases.Select(purchase => new ModuleNotionPurchaseDto
            {
                Id = purchase.Id,
                NotionInventoryItemId = purchase.NotionInventoryItemId,
                SourceName = purchase.SourceName,
                Price = purchase.Price,
                IsSalePrice = purchase.IsSalePrice,
                PurchasedAt = purchase.PurchasedAt,
                CreatedAt = purchase.CreatedAt
            }).ToList(),
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };

    private static ModuleInventoryReferenceItemDto MapReference(KnittingInventoryReferenceItemDto source)
        => new()
        {
            Id = source.Id,
            Category = source.Category,
            Name = source.Name,
            Slug = source.Slug,
            SortOrder = source.SortOrder
        };
}
