namespace TankerMade.Modules.Printing3D.ReferenceData;

public static class PrintingReferenceCategories
{
    public const string MaterialType = "material-type";
    public const string Diameter = "diameter";
    public const string PrinterTooling = "printer-tooling";

    private static readonly HashSet<string> ModuleOwnedCategories = new(StringComparer.OrdinalIgnoreCase)
    {
        MaterialType,
        Diameter,
        PrinterTooling
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
