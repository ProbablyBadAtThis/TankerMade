namespace TankerMade.Contracts.DTOs.ModuleSettings;

public class UpsertModuleSettingItemRequest
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}
