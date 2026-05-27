using Microsoft.EntityFrameworkCore;
using TankerMade.Server.Data;

namespace TankerMade.Server.Services.ReferenceData;

internal static class CoreReferenceLookup
{
    public static async Task<IReadOnlyList<CoreReferenceItem>?> TryGetItemsAsync(
        TankerMadeDbContext context,
        string category)
    {
        var normalized = NormalizeCategory(category);
        if (normalized == null)
        {
            return null;
        }

        return normalized switch
        {
            "theme" => await context.Themes
                .OrderBy(item => item.Name)
                .Select(item => new CoreReferenceItem(item.Id, "theme", item.Name, item.Slug))
                .ToListAsync(),
            "color" => await context.Colors
                .OrderBy(item => item.Name)
                .Select(item => new CoreReferenceItem(item.Id, "color", item.Name, item.Slug))
                .ToListAsync(),
            "source" => await context.Sources
                .OrderBy(item => item.Name)
                .Select(item => new CoreReferenceItem(item.Id, "source", item.Name, item.Slug))
                .ToListAsync(),
            "brand" => await context.Brands
                .OrderBy(item => item.Name)
                .Select(item => new CoreReferenceItem(item.Id, "brand", item.Name, item.Slug))
                .ToListAsync(),
            _ => null
        };
    }

    private static string? NormalizeCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return null;
        }

        return category.Trim().ToLowerInvariant() switch
        {
            "theme" or "themes" => "theme",
            "color" or "colors" => "color",
            "source" or "sources" => "source",
            "brand" or "brands" => "brand",
            _ => null
        };
    }
}

internal sealed record CoreReferenceItem(
    Guid Id,
    string Category,
    string Name,
    string Slug);
