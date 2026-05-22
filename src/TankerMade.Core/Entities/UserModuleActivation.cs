namespace TankerMade.Core.Entities;

public class UserModuleActivation
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ModuleDefinitionId { get; set; }
    public bool IsActive { get; set; }
    public DateTime ActivatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    protected UserModuleActivation()
    {
    }

    public UserModuleActivation(Guid id, Guid userId, Guid moduleDefinitionId)
    {
        Id = id;
        UserId = userId;
        ModuleDefinitionId = moduleDefinitionId;
        IsActive = true;
        ActivatedAt = DateTime.UtcNow;
        UpdatedAt = ActivatedAt;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
