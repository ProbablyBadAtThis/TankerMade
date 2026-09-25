using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TankerMade.Contracts.DTOs.Reference;
using TankerMade.Core.Entities;
using TankerMade.Server.Controllers;
using TankerMade.Server.Data;

namespace TankerMade.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/reference")]
public class ReferenceDataController : ControllerBase
{
    private readonly TankerMadeDbContext _context;

    public ReferenceDataController(TankerMadeDbContext context)
    {
        _context = context;
    }

    [HttpGet("{category}")]
    public async Task<ActionResult<IReadOnlyList<CoreReferenceItemDto>>> List(string category, [FromQuery] string? search = null)
    {
        var normalized = NormalizeCategory(category);
        if (normalized == null)
        {
            return BadRequest("Unsupported reference category.");
        }

        return Ok(await ListCategoryAsync(normalized, search));
    }

    [HttpPost("{category}")]
    public async Task<ActionResult<CoreReferenceItemDto>> Create(string category, CreateCoreReferenceItemRequest request)
    {
        var normalized = NormalizeCategory(category);
        if (normalized == null)
        {
            return BadRequest("Unsupported reference category.");
        }

        var name = request.Name?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest("Name is required.");
        }

        var created = await CreateCategoryItemAsync(normalized, name);
        if (created == null)
        {
            return Conflict("A reference item with that name already exists.");
        }

        return CreatedAtAction(nameof(List), new { category = normalized }, created);
    }

    private static string? NormalizeCategory(string category)
    {
        return category.Trim().ToLowerInvariant() switch
        {
            "themes" => "themes",
            "colors" => "colors",
            "sources" => "sources",
            "brands" => "brands",
            _ => null
        };
    }

    private async Task<IReadOnlyList<CoreReferenceItemDto>> ListCategoryAsync(string category, string? search)
    {
        var terms = string.IsNullOrWhiteSpace(search)
            ? []
            : search.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return category switch
        {
            "themes" => await QueryThemesAsync(terms),
            "colors" => await QueryColorsAsync(terms),
            "sources" => await QuerySourcesAsync(terms),
            "brands" => await QueryBrandsAsync(terms),
            _ => []
        };
    }

    private async Task<CoreReferenceItemDto?> CreateCategoryItemAsync(string category, string name)
    {
        return category switch
        {
            "themes" => await CreateThemeAsync(name),
            "colors" => await CreateColorAsync(name),
            "sources" => await CreateSourceAsync(name),
            "brands" => await CreateBrandAsync(name),
            _ => null
        };
    }

    private async Task<IReadOnlyList<CoreReferenceItemDto>> QueryThemesAsync(IReadOnlyList<string> terms)
    {
        var query = _context.Themes.AsNoTracking();
        foreach (var term in terms)
        {
            var like = $"%{term.ToLowerInvariant()}%";
            query = query.Where(item => EF.Functions.Like(item.Name.ToLower(), like));
        }

        return await query
            .OrderBy(item => item.Name)
            .Select(item => new CoreReferenceItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Slug = item.Slug,
                CreatedAt = item.CreatedAt
            })
            .ToListAsync();
    }

    private async Task<IReadOnlyList<CoreReferenceItemDto>> QueryColorsAsync(IReadOnlyList<string> terms)
    {
        var query = _context.Colors.AsNoTracking();
        foreach (var term in terms)
        {
            var like = $"%{term.ToLowerInvariant()}%";
            query = query.Where(item => EF.Functions.Like(item.Name.ToLower(), like));
        }

        return await query
            .OrderBy(item => item.Name)
            .Select(item => new CoreReferenceItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Slug = item.Slug,
                CreatedAt = item.CreatedAt
            })
            .ToListAsync();
    }

    private async Task<IReadOnlyList<CoreReferenceItemDto>> QuerySourcesAsync(IReadOnlyList<string> terms)
    {
        var query = _context.Sources.AsNoTracking();
        foreach (var term in terms)
        {
            var like = $"%{term.ToLowerInvariant()}%";
            query = query.Where(item => EF.Functions.Like(item.Name.ToLower(), like));
        }

        return await query
            .OrderBy(item => item.Name)
            .Select(item => new CoreReferenceItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Slug = item.Slug,
                CreatedAt = item.CreatedAt
            })
            .ToListAsync();
    }

    private async Task<IReadOnlyList<CoreReferenceItemDto>> QueryBrandsAsync(IReadOnlyList<string> terms)
    {
        var query = _context.Brands.AsNoTracking();
        foreach (var term in terms)
        {
            var like = $"%{term.ToLowerInvariant()}%";
            query = query.Where(item => EF.Functions.Like(item.Name.ToLower(), like));
        }

        return await query
            .OrderBy(item => item.Name)
            .Select(item => new CoreReferenceItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Slug = item.Slug,
                CreatedAt = item.CreatedAt
            })
            .ToListAsync();
    }

    private async Task<CoreReferenceItemDto?> CreateThemeAsync(string name)
    {
        if (await _context.Themes.AnyAsync(item => item.Name.ToLower() == name.ToLower()))
        {
            return null;
        }

        var entity = new Theme(Guid.NewGuid(), name);
        _context.Themes.Add(entity);
        await _context.SaveChangesAsync();
        return Map(entity.Id, entity.Name, entity.Slug, entity.CreatedAt);
    }

    private async Task<CoreReferenceItemDto?> CreateColorAsync(string name)
    {
        if (await _context.Colors.AnyAsync(item => item.Name.ToLower() == name.ToLower()))
        {
            return null;
        }

        var entity = new Color(Guid.NewGuid(), name);
        _context.Colors.Add(entity);
        await _context.SaveChangesAsync();
        return Map(entity.Id, entity.Name, entity.Slug, entity.CreatedAt);
    }

    private async Task<CoreReferenceItemDto?> CreateSourceAsync(string name)
    {
        if (await _context.Sources.AnyAsync(item => item.Name.ToLower() == name.ToLower()))
        {
            return null;
        }

        var entity = new Source(Guid.NewGuid(), name);
        _context.Sources.Add(entity);
        await _context.SaveChangesAsync();
        return Map(entity.Id, entity.Name, entity.Slug, entity.CreatedAt);
    }

    private async Task<CoreReferenceItemDto?> CreateBrandAsync(string name)
    {
        if (await _context.Brands.AnyAsync(item => item.Name.ToLower() == name.ToLower()))
        {
            return null;
        }

        var entity = new Brand(Guid.NewGuid(), name);
        _context.Brands.Add(entity);
        await _context.SaveChangesAsync();
        return Map(entity.Id, entity.Name, entity.Slug, entity.CreatedAt);
    }

    private static CoreReferenceItemDto Map(Guid id, string name, string slug, DateTime createdAt)
    {
        return new CoreReferenceItemDto
        {
            Id = id,
            Name = name,
            Slug = slug,
            CreatedAt = createdAt
        };
    }
}
