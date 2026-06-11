using TankerMade.Contracts.DTOs.ModulePatterns;
using TankerMade.Contracts.Services.ModuleCapabilities;
using TankerMade.Modules.Knitting;
using TankerMade.Modules.Knitting.DTOs.Patterns;
using TankerMade.Modules.Knitting.Services;

namespace TankerMade.Server.Services.ModuleCapabilities;

public class KnittingPatternCapabilityHandler : IModulePatternCapabilityHandler
{
    private readonly IKnittingPatternService _service;

    public KnittingPatternCapabilityHandler(IKnittingPatternService service)
    {
        _service = service;
    }

    public string ModuleKey => KnittingModule.ModuleKey;

    public async Task<IReadOnlyList<ModulePatternDto>> GetAllAsync(Guid userId, int page = 1, int pageSize = 50)
        => (await _service.GetAllAsync(userId, page, pageSize)).Select(Map).ToList();

    public async Task<IReadOnlyList<ModulePatternDto>> SearchAsync(string query, Guid userId, int page = 1, int pageSize = 50)
        => (await _service.SearchAsync(query, userId, page, pageSize)).Select(Map).ToList();

    public async Task<ModulePatternDto?> GetByIdAsync(Guid id, Guid userId)
    {
        var result = await _service.GetByIdAsync(id, userId);
        return result == null ? null : Map(result);
    }

    public async Task<ModulePatternDto> CreateAsync(CreateModulePatternRequest request, Guid userId)
        => Map(await _service.CreateAsync(new CreateKnittingPatternDto
        {
            Name = request.Name,
            Type = request.Type,
            Form = request.Form,
            Difficulty = request.Difficulty,
            ThemeId = request.ThemeId,
            ColorId = request.ColorId,
            SourceId = request.SourceId,
            SuggestedYarnWeight = request.SuggestedYarnWeight,
            SuggestedNeedleSizes = request.SuggestedNeedleSizes,
            RequiredNotions = request.RequiredNotions
        }, userId));

    public async Task<ModulePatternDto?> UpdateAsync(UpdateModulePatternRequest request, Guid userId)
    {
        var updated = await _service.UpdateAsync(new UpdateKnittingPatternDto
        {
            Id = request.Id,
            Name = request.Name,
            Type = request.Type,
            Form = request.Form,
            Difficulty = request.Difficulty,
            ThemeId = request.ThemeId,
            ColorId = request.ColorId,
            SourceId = request.SourceId,
            SuggestedYarnWeight = request.SuggestedYarnWeight,
            SuggestedNeedleSizes = request.SuggestedNeedleSizes,
            RequiredNotions = request.RequiredNotions
        }, userId);

        return updated == null ? null : Map(updated);
    }

    public Task<bool> DeleteAsync(Guid id, Guid userId) => _service.DeleteAsync(id, userId);

    public async Task<ModulePatternPieceDto?> AddPieceAsync(Guid patternId, CreateModulePatternPieceRequest request, Guid userId)
    {
        var piece = await _service.AddPieceAsync(patternId, new CreateKnittingPatternPieceDto { Name = request.Name }, userId);
        return piece == null ? null : Map(piece);
    }

    public async Task<ModulePatternPieceDto?> UpdatePieceAsync(Guid patternId, UpdateModulePatternPieceRequest request, Guid userId)
    {
        var piece = await _service.UpdatePieceAsync(patternId, new UpdateKnittingPatternPieceDto
        {
            Id = request.Id,
            Name = request.Name ?? string.Empty
        }, userId);

        return piece == null ? null : Map(piece);
    }

    public Task<bool> DeletePieceAsync(Guid patternId, Guid pieceId, Guid userId)
        => _service.DeletePieceAsync(patternId, pieceId, userId);

    public Task<bool> ReorderPiecesAsync(Guid patternId, ReorderModulePatternItemsRequest request, Guid userId)
        => _service.ReorderPiecesAsync(patternId, new ReorderKnittingPatternItemsDto { OrderedIds = request.OrderedIds }, userId);

    public async Task<ModulePatternStepDto?> AddStepAsync(Guid patternId, Guid pieceId, CreateModulePatternStepRequest request, Guid userId)
    {
        var step = await _service.AddStepAsync(patternId, pieceId, new CreateKnittingPatternStepDto
        {
            RangeStart = request.StartIndex,
            RangeEnd = request.EndIndex,
            Label = request.Label,
            StitchCount = request.StitchCount,
            Instructions = request.Notes
        }, userId);

        return step == null ? null : Map(step);
    }

    public async Task<ModulePatternStepDto?> UpdateStepAsync(Guid patternId, Guid pieceId, UpdateModulePatternStepRequest request, Guid userId)
    {
        var step = await _service.UpdateStepAsync(patternId, pieceId, new UpdateKnittingPatternStepDto
        {
            Id = request.Id,
            RangeStart = request.StartIndex,
            RangeEnd = request.EndIndex,
            Label = request.Label ?? string.Empty,
            StitchCount = request.StitchCount,
            Instructions = request.Notes ?? string.Empty
        }, userId);

        return step == null ? null : Map(step);
    }

    public Task<bool> DeleteStepAsync(Guid patternId, Guid pieceId, Guid stepId, Guid userId)
        => _service.DeleteStepAsync(patternId, pieceId, stepId, userId);

    public Task<bool> ReorderStepsAsync(Guid patternId, Guid pieceId, ReorderModulePatternItemsRequest request, Guid userId)
        => _service.ReorderStepsAsync(patternId, pieceId, new ReorderKnittingPatternItemsDto { OrderedIds = request.OrderedIds }, userId);

    public async Task<ModulePatternSupplyDto?> AddSupplyAsync(Guid patternId, CreateModulePatternSupplyRequest request, Guid userId)
    {
        var supply = await _service.AddSupplyAsync(patternId, new CreateKnittingPatternSupplyDto
        {
            SupplyType = request.SupplyType,
            Name = request.Name,
            InventoryItemId = request.InventoryItemId
        }, userId);

        return supply == null ? null : Map(supply);
    }

    public Task<bool> DeleteSupplyAsync(Guid patternId, Guid supplyId, Guid userId)
        => _service.DeleteSupplyAsync(patternId, supplyId, userId);

    private static ModulePatternDto Map(KnittingPatternDto source)
    {
        var pieces = source.Pieces.Select(Map).ToList();

        return new ModulePatternDto
        {
            Id = source.Id,
            Name = source.Name,
            Slug = source.Slug,
            Type = source.Type,
            Form = source.Form,
            Difficulty = source.Difficulty,
            ThemeId = source.ThemeId,
            ThemeName = source.ThemeName,
            ColorId = source.ColorId,
            ColorName = source.ColorName,
            SourceId = source.SourceId,
            SourceName = source.SourceName,
            SuggestedYarnWeight = source.SuggestedYarnWeight,
            SuggestedNeedleSizes = source.SuggestedNeedleSizes,
            RequiredNotions = source.RequiredNotions,
            UserId = source.UserId,
            Username = source.Username,
            Supplies = source.Supplies.Select(Map).ToList(),
            Pieces = pieces,
            PieceCount = source.PieceCount,
            StepCount = source.StepCount,
            Progress = new ModulePatternProgressDto
            {
                TotalPieces = source.PieceCount,
                CompletedPieces = 0,
                TotalSteps = source.StepCount,
                CompletedSteps = 0,
                CompletionPercent = 0
            },
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }

    private static ModulePatternPieceDto Map(KnittingPatternPieceDto source)
    {
        var steps = source.Steps.Select(Map).ToList();
        return new ModulePatternPieceDto
        {
            Id = source.Id,
            Name = source.Name,
            SortOrder = source.SortOrder,
            StepCount = steps.Count,
            CompletedStepCount = 0,
            IsComplete = false,
            Steps = steps
        };
    }

    private static ModulePatternStepDto Map(KnittingPatternStepDto source)
    {
        return new ModulePatternStepDto
        {
            Id = source.Id,
            PieceId = source.PatternPieceId,
            SortOrder = source.SortOrder,
            Label = source.Label,
            StartIndex = source.RangeStart,
            EndIndex = source.RangeEnd,
            StitchCount = source.StitchCount,
            Notes = source.Instructions
        };
    }

    private static ModulePatternSupplyDto Map(KnittingPatternSupplyDto source)
    {
        return new ModulePatternSupplyDto
        {
            Id = source.Id,
            PatternId = source.PatternId,
            SupplyType = source.SupplyType,
            Name = source.Name,
            InventoryItemId = source.InventoryItemId,
            SortOrder = source.SortOrder,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }
}
