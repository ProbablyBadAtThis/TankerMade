using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TankerMade.Contracts.DTOs.Modules;
using TankerMade.Contracts.Services;
using TankerMade.Core.Entities;
using TankerMade.Core.Modules;
using TankerMade.Server.Data;
using TankerMade.Server.Modules;

namespace TankerMade.Server.Services;

public class ModuleService : IModuleService
{
    private static readonly TimeSpan ModuleDiscoveryCacheTtl = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan ModuleListCacheTtl = TimeSpan.FromMinutes(1);
    private const string DiscoveryCacheKey = "modules:discovery";
    private readonly TankerMadeDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly IReadOnlyList<IModuleDiscoveryProvider> _discoveryProviders;

    public ModuleService(
        TankerMadeDbContext context,
        IMemoryCache cache,
        IEnumerable<IModuleDiscoveryProvider> discoveryProviders)
    {
        _context = context;
        _cache = cache;
        _discoveryProviders = discoveryProviders.ToList();
    }

    public ModuleService(
        TankerMadeDbContext context,
        IEnumerable<IModuleDiscoveryProvider> discoveryProviders)
        : this(context, new MemoryCache(new MemoryCacheOptions()), discoveryProviders)
    {
    }

    public async Task<IReadOnlyList<ModuleDto>> GetAvailableModulesAsync(Guid userId)
    {
        var cacheKey = GetAvailableModulesCacheKey(userId);
        if (_cache.TryGetValue<IReadOnlyList<ModuleDto>>(cacheKey, out var cached) && cached != null)
        {
            return cached;
        }

        var discoveredByKey = await DiscoverByModuleKeyAsync();
        var activeModuleIds = await _context.UserModuleActivations
            .Where(a => a.UserId == userId && a.IsActive)
            .Select(a => a.ModuleDefinitionId)
            .ToListAsync();

        var modules = await _context.ModuleDefinitions
            .OrderBy(m => m.Name)
            .ToListAsync();

        var result = modules
            .Select(m => ToDto(m, activeModuleIds.Contains(m.Id), discoveredByKey))
            .ToList();
        _cache.Set(cacheKey, result, ModuleListCacheTtl);
        return result;
    }

    public async Task<IReadOnlyList<ModuleDto>> GetActiveModulesAsync(Guid userId)
    {
        var cacheKey = GetActiveModulesCacheKey(userId);
        if (_cache.TryGetValue<IReadOnlyList<ModuleDto>>(cacheKey, out var cached) && cached != null)
        {
            return cached;
        }

        var discoveredByKey = await DiscoverByModuleKeyAsync();
        var active = await _context.UserModuleActivations
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
                    IsActive = activation.IsActive,
                    NavigationLabel = string.Empty,
                    NavigationRoute = string.Empty,
                    NavigationOrder = 0
                })
            .ToListAsync();

        var result = active
            .Select(dto =>
            {
                if (discoveredByKey.TryGetValue(dto.ModuleKey, out var discovered)
                    && discovered is IModuleNavigation navigation)
                {
                    dto.NavigationLabel = navigation.NavigationLabel;
                    dto.NavigationRoute = navigation.NavigationRoute;
                    dto.NavigationOrder = navigation.NavigationOrder;
                }

                return dto;
            })
            .OrderBy(m => m.Name)
            .ToList();
        _cache.Set(cacheKey, result, ModuleListCacheTtl);
        return result;
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
        InvalidateUserModuleListCache(userId);

        var discoveredByKey = await DiscoverByModuleKeyAsync();
        return ToDto(module, activation.IsActive, discoveredByKey);
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
        InvalidateUserModuleListCache(userId);
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

    private static ModuleDto ToDto(
        ModuleDefinition module,
        bool isActive,
        IReadOnlyDictionary<string, IModule> discoveredByKey)
    {
        var navigationLabel = string.Empty;
        var navigationRoute = string.Empty;
        var navigationOrder = 0;
        if (discoveredByKey.TryGetValue(module.ModuleKey, out var discovered)
            && discovered is IModuleNavigation navigation)
        {
            navigationLabel = navigation.NavigationLabel;
            navigationRoute = navigation.NavigationRoute;
            navigationOrder = navigation.NavigationOrder;
        }

        return new ModuleDto
        {
            Id = module.Id,
            ModuleKey = module.ModuleKey,
            Name = module.Name,
            Description = module.Description,
            Version = module.Version,
            IsBundled = module.IsBundled,
            IsActive = isActive,
            NavigationLabel = navigationLabel,
            NavigationRoute = navigationRoute,
            NavigationOrder = navigationOrder
        };
    }

    private async Task<IReadOnlyDictionary<string, IModule>> DiscoverByModuleKeyAsync()
    {
        if (_cache.TryGetValue<IReadOnlyDictionary<string, IModule>>(DiscoveryCacheKey, out var cached)
            && cached != null)
        {
            return cached;
        }

        var map = new Dictionary<string, IModule>(StringComparer.OrdinalIgnoreCase);

        foreach (var provider in _discoveryProviders)
        {
            var discovered = await provider.DiscoverAsync();
            foreach (var registration in discovered)
            {
                map[registration.Module.ModuleKey] = registration.Module;
            }
        }

        _cache.Set(DiscoveryCacheKey, map, ModuleDiscoveryCacheTtl);
        return map;
    }

    private static string GetAvailableModulesCacheKey(Guid userId) => $"modules:available:{userId}";
    private static string GetActiveModulesCacheKey(Guid userId) => $"modules:active:{userId}";

    private void InvalidateUserModuleListCache(Guid userId)
    {
        _cache.Remove(GetAvailableModulesCacheKey(userId));
        _cache.Remove(GetActiveModulesCacheKey(userId));
    }
}
