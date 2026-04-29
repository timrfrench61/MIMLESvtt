using VttMvuModel.Workspace;
using VttMvuView.GameSetup;
using VttMvuView.WorkspaceLaunch;

namespace MIMLESvtt.Services;

public static class SetupSelectionMappingService
{
    public static IReadOnlyList<GameSetupGameSystemOption> ToGameSetupGameSystemOptions(IReadOnlyList<GameSystemSummary> gameSystems)
    {
        ArgumentNullException.ThrowIfNull(gameSystems);

        return gameSystems
            .Select(system => new GameSetupGameSystemOption(system.GameSystemId, system.Name, system.Description))
            .ToList();
    }

    public static IReadOnlyList<GameSetupSessionSourceOption> ToGameSetupSessionSourceOptions(IReadOnlyList<ScenarioSummary> scenarios)
    {
        ArgumentNullException.ThrowIfNull(scenarios);

        return scenarios
            .Select(scenario => new GameSetupSessionSourceOption(scenario.ScenarioId, scenario.Name, scenario.Description))
            .ToList();
    }

    public static IReadOnlyList<GameSystemCardViewModel> ToWorkspaceLaunchGameSystemCards(IReadOnlyList<GameSystemSummary> gameSystems)
    {
        ArgumentNullException.ThrowIfNull(gameSystems);

        return gameSystems
            .Select(system => new GameSystemCardViewModel(
                GameSystemId: system.GameSystemId,
                Name: system.Name,
                Description: system.Description,
                IconAssetId: null,
                IsInstalled: true,
                IsEnabled: true))
            .ToList();
    }

    public static IReadOnlyList<ScenarioCardViewModel> ToWorkspaceLaunchScenarioCards(IReadOnlyList<ScenarioSummary> scenarios, string? fallbackGameSystemName = null)
    {
        ArgumentNullException.ThrowIfNull(scenarios);

        return scenarios
            .Select(scenario => new ScenarioCardViewModel(
                ScenarioId: scenario.ScenarioId,
                Name: scenario.Name,
                Description: scenario.Description,
                GameSystemName: string.IsNullOrWhiteSpace(scenario.GameSystemId) ? (fallbackGameSystemName ?? string.Empty) : scenario.GameSystemId,
                LastPlayedOn: scenario.LastPlayedAt))
            .ToList();
    }
}
