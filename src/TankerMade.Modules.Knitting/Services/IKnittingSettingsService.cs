using TankerMade.Modules.Knitting.DTOs.Settings;

namespace TankerMade.Modules.Knitting.Services;

public interface IKnittingSettingsService
{
    Task<IReadOnlyList<KnittingSettingItemDto>> GetAllAsync(Guid userId, string? category = null);
    Task<KnittingSettingItemDto> UpsertAsync(UpsertKnittingSettingItemDto request, Guid userId);
}
