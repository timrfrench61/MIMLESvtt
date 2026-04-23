using Microsoft.AspNetCore.Components;

namespace MIMLESvtt.Components.Layout;

public partial class MainLayout
{
    [Inject] public NavigationManager Navigation { get; set; } = default!;

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
