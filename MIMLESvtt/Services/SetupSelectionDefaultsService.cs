namespace MIMLESvtt.Services;

public static class SetupSelectionDefaultsService
{
    public static string? ResolveSelectedId(string? currentSelectedId, IReadOnlyList<string> availableIds)
    {
        ArgumentNullException.ThrowIfNull(availableIds);

        if (availableIds.Count == 0)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(currentSelectedId)
            && availableIds.Any(id => string.Equals(id, currentSelectedId, StringComparison.OrdinalIgnoreCase)))
        {
            return currentSelectedId;
        }

        return availableIds[0];
    }
}
