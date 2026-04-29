using Microsoft.AspNetCore.Components;
using MIMLESvtt.Services;

namespace MIMLESvtt.Components.Layout;

public partial class MainLayout
{
    [Inject] public NavigationManager Navigation { get; set; } = default!;
    [Inject] public SettingsPreferenceService SettingsPreferenceService { get; set; } = default!;

    private string _themeClass = "theme-default";
    private string _contentClass = string.Empty;

    protected override void OnInitialized()
    {
        var snapshot = SettingsPreferenceService.Load();
        var classes = SettingsShellClassProjectionService.Project(snapshot);
        _themeClass = classes.ThemeClass;
        _contentClass = classes.ContentClass;
    }

    private string PageClass => string.IsNullOrWhiteSpace(_themeClass) ? "page" : $"page {_themeClass}";

    private string ArticleClass => string.IsNullOrWhiteSpace(_contentClass)
        ? "content px-4"
        : $"content px-4 {_contentClass}";

    private string CurrentRouteDisplay
    {
        get
        {
            var relativePath = Navigation.ToBaseRelativePath(Navigation.Uri);
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return "Home";
            }

            var segments = relativePath
                .Split('/', StringSplitOptions.RemoveEmptyEntries)
                .Select(segment => char.ToUpperInvariant(segment[0]) + segment[1..])
                .ToList();

            return $"Home / {string.Join(" / ", segments)}";
        }
    }
}
