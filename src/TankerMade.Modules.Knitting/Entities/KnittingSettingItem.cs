namespace TankerMade.Modules.Knitting.Entities;

public class KnittingSettingItem
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    protected KnittingSettingItem() { }

    public KnittingSettingItem(Guid id, Guid userId, string key)
    {
        Id = id;
        UserId = userId;
        Key = key?.Trim() ?? throw new ArgumentNullException(nameof(key));
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string value, string category)
    {
        Value = value?.Trim() ?? string.Empty;
        Category = category?.Trim() ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }
}
