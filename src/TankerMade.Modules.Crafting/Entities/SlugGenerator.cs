namespace TankerMade.Modules.Crafting.Entities;

internal static class SlugGenerator
{
    public static string Generate(string value)
    {
        return value.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("'", string.Empty);
    }
}
