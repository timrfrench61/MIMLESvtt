using VttMvuView.Tabletop;

namespace MIMLESvtt.Services;

public static class TabletopLaunchStateFactory
{
    public static TabletopState CreateInitialState(string launchMode, string? launchSource)
    {
        var normalizedMode = string.Equals(launchMode, "play", StringComparison.OrdinalIgnoreCase)
            ? "play"
            : "edit";
        var normalizedSource = string.IsNullOrWhiteSpace(launchSource) ? null : launchSource.Trim();
        var tokens = ResolveTokens(normalizedSource);

        return new TabletopState(
            BoardWidth: 8,
            BoardHeight: 8,
            Tokens: tokens,
            SelectedTokenId: null,
            TurnNumber: 1,
            TurnPhase: "Main",
            ActiveToolId: normalizedMode == "play" ? "Move" : "Select",
            LastDiceRoll: null,
            StatusMessage: BuildLaunchStatus(normalizedMode, normalizedSource));
    }

    private static IReadOnlyList<TabletopTokenState> ResolveTokens(string? launchSource)
    {
        if (!string.IsNullOrWhiteSpace(launchSource)
            && launchSource.Contains("CHECKERS", StringComparison.OrdinalIgnoreCase))
        {
            return
            [
                new TabletopTokenState("B-1", "Blue Checker 1", 1, 5, 0),
                new TabletopTokenState("B-2", "Blue Checker 2", 3, 5, 0),
                new TabletopTokenState("R-1", "Red Checker 1", 4, 2, 180),
                new TabletopTokenState("R-2", "Red Checker 2", 6, 2, 180)
            ];
        }

        return
        [
            new TabletopTokenState("T-1", "Blue Unit", 2, 2, 0),
            new TabletopTokenState("T-2", "Red Unit", 5, 5, 180)
        ];
    }

    private static string BuildLaunchStatus(string mode, string? source)
    {
        var sourceLabel = string.IsNullOrWhiteSpace(source) ? "default source" : source;
        return $"Launched in {mode} mode from {sourceLabel}.";
    }
}
