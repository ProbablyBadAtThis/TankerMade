using MudBlazor;

namespace TankerMade.Client.Theme;

public static class TankerMadeMudTheme
{
    public static MudTheme Theme { get; } = new()
    {
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "12px",
        },
        PaletteLight = new PaletteLight
        {
            Primary = "#1b6ec2",
            Secondary = "#5c6b57",
            AppbarBackground = "#fffdf8",
            Background = "#faf8f1",
            Surface = "#ffffff",
            DrawerBackground = "#ffffff",
            TextPrimary = "#213127",
            TextSecondary = "#5c6b57",
            ActionDefault = "#5c6b57",
            Divider = "#d9e2d2",
            LinesDefault = "#d9e2d2",
            TableLines = "#d9e2d2",
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#b8832f",
            Secondary = "#d4a054",
            AppbarBackground = "#221c17",
            Background = "#1a1612",
            Surface = "#2c261f",
            DrawerBackground = "#241f1a",
            TextPrimary = "#f4ede3",
            TextSecondary = "#b9aa96",
            ActionDefault = "#b9aa96",
            Divider = "#4a4035",
            LinesDefault = "#4a4035",
            TableLines = "#4a4035",
        },
    };
}
