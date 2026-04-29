namespace MIMLESvtt.Services;

public static class SettingsShellClassProjectionService
{
    public static SettingsShellClassProjection Project(SettingsPreferenceSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var themeClass = snapshot.ThemeName.Trim().ToLowerInvariant() switch
        {
            "dark" => "theme-dark",
            "highcontrast" => "theme-high-contrast",
            _ => "theme-default"
        };

        var contentClass = snapshot.UseCompactLayout ? "compact-layout" : string.Empty;

        return new SettingsShellClassProjection(themeClass, contentClass);
    }
}

public sealed record SettingsShellClassProjection(
    string ThemeClass,
    string ContentClass);
