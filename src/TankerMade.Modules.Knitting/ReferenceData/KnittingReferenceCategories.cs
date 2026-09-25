namespace TankerMade.Modules.Knitting.ReferenceData;

public static class KnittingReferenceCategories
{
    public const string YarnWeight = "yarn-weight";
    public const string FiberTag = "fiber-tag";
    public const string ToolType = "tool-type";
    public const string NotionType = "notion-type";

    private static readonly HashSet<string> ModuleOwnedCategories = new(StringComparer.OrdinalIgnoreCase)
    {
        YarnWeight,
        FiberTag,
        ToolType,
        NotionType
    };

    public static bool IsModuleOwned(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return false;
        }

        return ModuleOwnedCategories.Contains(category.Trim());
    }
}
