using TankerMade.Contracts.DTOs.ModuleSettings;
using TankerMade.Contracts.Services.ModuleCapabilities;
using TankerMade.Modules.Knitting;
using TankerMade.Modules.Knitting.DTOs.Settings;
using TankerMade.Modules.Knitting.Services;

namespace TankerMade.Server.Services.ModuleCapabilities;

public class KnittingSettingsCapabilityHandler : IModuleSettingsCapabilityHandler
{
    private readonly IKnittingSettingsService _service;

    public KnittingSettingsCapabilityHandler(IKnittingSettingsService service)
    {
        _service = service;
    }

    public string ModuleKey => KnittingModule.ModuleKey;

    public async Task<IReadOnlyList<ModuleSettingItemDto>> GetAllAsync(Guid userId, string? category = null)
        => (await _service.GetAllAsync(userId, category)).Select(Map).ToList();

    public async Task<ModuleSettingItemDto> UpsertAsync(UpsertModuleSettingItemRequest request, Guid userId)
        => Map(await _service.UpsertAsync(new UpsertKnittingSettingItemDto
        {
            Key = request.Key,
            Value = request.Value,
            Category = request.Category
        }, userId));

    private static ModuleSettingItemDto Map(KnittingSettingItemDto source)
    {
        return new ModuleSettingItemDto
        {
            Id = source.Id,
            Key = source.Key,
            Value = source.Value,
            Category = source.Category,
            UserId = source.UserId,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }
}
