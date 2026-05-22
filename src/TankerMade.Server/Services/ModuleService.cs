using Microsoft.EntityFrameworkCore;
using TankerMade.Contracts.DTOs.Modules;
using TankerMade.Contracts.Services;
using TankerMade.Core.Entities;
using TankerMade.Server.Data;

namespace TankerMade.Server.Services;

public class ModuleService : IModuleService
{
    private readonly TankerMadeDbContext _context;

    public ModuleService(TankerMadeDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ModuleDto>> GetAvailableModulesAsync(Guid userId)
    {
        var activeModuleIds = await _context.UserModuleActivations
            .Where(a => a.UserId == userId && a.IsActive)
            .Select(a => a.ModuleDefinitionId)
            .ToListAsync();

        return await _context.ModuleDefinitions
            .OrderBy(m => m.Name)
            .Select(m => new ModuleDto
            {
                Id = m.Id,
                ModuleKey = m.ModuleKey,
                Name = m.Name,
                Description = m.Description,
                Version = m.Version,
                IsBundled = m.IsBundled,
                IsActive = activeModuleIds.Contains(m.Id)
            })
            .ToListAsync();
    }

    public async Task<IReadOnlyList<ModuleDto>> GetActiveModulesAsync(Guid userId)
    {
        return await _context.UserModuleActivations
            .Where(a => a.UserId == userId && a.IsActive)
            .Join(
                _context.ModuleDefinitions,
                activation => activation.ModuleDefinitionId,
                module => module.Id,
                (activation, module) => new ModuleDto
                {
                    Id = module.Id,
                    ModuleKey = module.ModuleKey,
                    Name = module.Name,
                    Description = module.Description,
                    Version = module.Version,
                    IsBundled = module.IsBundled,
                    IsActive = activation.IsActive
                })
            .OrderBy(m => m.Name)
            .ToListAsync();
    }

    public async Task<ModuleDto?> ActivateAsync(string moduleKey, Guid userId)
    {
        var module = await _context.ModuleDefinitions
            .SingleOrDefaultAsync(m => m.ModuleKey == moduleKey);

        if (module == null)
        {
            return null;
        }

        var activation = await _context.UserModuleActivations
            .SingleOrDefaultAsync(a => a.UserId == userId && a.ModuleDefinitionId == module.Id);

        if (activation == null)
        {
            activation = new UserModuleActivation(Guid.NewGuid(), userId, module.Id);
            _context.UserModuleActivations.Add(activation);
        }
        else
        {
            activation.Activate();
        }

        await _context.SaveChangesAsync();

        return ToDto(module, activation.IsActive);
    }

    public async Task<bool> DeactivateAsync(string moduleKey, Guid userId)
    {
        var activation = await _context.UserModuleActivations
            .Join(
                _context.ModuleDefinitions.Where(m => m.ModuleKey == moduleKey),
                activation => activation.ModuleDefinitionId,
                module => module.Id,
                (activation, module) => activation)
            .SingleOrDefaultAsync(a => a.UserId == userId);

        if (activation == null)
        {
            return false;
        }

        activation.Deactivate();
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsActiveAsync(string moduleKey, Guid userId)
    {
        return await _context.UserModuleActivations
            .Join(
                _context.ModuleDefinitions.Where(m => m.ModuleKey == moduleKey),
                activation => activation.ModuleDefinitionId,
                module => module.Id,
                (activation, module) => activation)
            .AnyAsync(a => a.UserId == userId && a.IsActive);
    }

    private static ModuleDto ToDto(ModuleDefinition module, bool isActive)
    {
        return new ModuleDto
        {
            Id = module.Id,
            ModuleKey = module.ModuleKey,
            Name = module.Name,
            Description = module.Description,
            Version = module.Version,
            IsBundled = module.IsBundled,
            IsActive = isActive
        };
    }
}
