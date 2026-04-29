using VttMvuModel.Workspace;
using VttMvuView.WorkspaceLaunch;

namespace MIMLESvtt.Services;

public static class SetupGameSystemSummaryProjectionService
{
    public static IReadOnlyList<GameSystemSummary> FromSelectedGameSystem(string? selectedGameSystemId)
    {
        var resolvedId = string.IsNullOrWhiteSpace(selectedGameSystemId)
            ? "EMPTY-GAMEBOX"
            : selectedGameSystemId;

        return
        [
            new GameSystemSummary(resolvedId, resolvedId, "local", "Selected setup game system.")
        ];
    }

    public static IReadOnlyList<GameSystemSummary> FromWorkspaceLaunchCards(IReadOnlyList<GameSystemCardViewModel> gameSystems)
    {
        ArgumentNullException.ThrowIfNull(gameSystems);

        return gameSystems
            .Select(system => new GameSystemSummary(
                system.GameSystemId,
                system.Name,
                "local",
                string.IsNullOrWhiteSpace(system.Description) ? "Selected setup game system." : system.Description))
            .ToList();
    }
}
