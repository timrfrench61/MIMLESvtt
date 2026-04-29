using VttMvuModel.Workspace;

namespace MIMLESvtt.Services;

public class WorkspaceSetupOptionsService
{
    private readonly ModuleImportService _moduleImportService;
    private readonly LoadGameService _loadGameService;

    public WorkspaceSetupOptionsService(
        ModuleImportService moduleImportService,
        LoadGameService loadGameService)
    {
        _moduleImportService = moduleImportService ?? throw new ArgumentNullException(nameof(moduleImportService));
        _loadGameService = loadGameService ?? throw new ArgumentNullException(nameof(loadGameService));
    }

    public IReadOnlyList<GameSystemSummary> ListGameSystems()
    {
        return _moduleImportService.ListAvailableGameSystems()
            .Select(id => new GameSystemSummary(
                GameSystemId: id,
                Name: id,
                Version: "local",
                Description: "Discovered from available gameboxes."))
            .OrderBy(s => s.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public IReadOnlyList<ScenarioSummary> ListSavedScenarios()
    {
        return _loadGameService.ListKnownSessions()
            .Select(session => new ScenarioSummary(
                ScenarioId: session.FilePath,
                GameSystemId: "SAVED-SESSION",
                Name: session.FileName,
                Description: session.JoinCode,
                LastPlayedAt: null))
            .OrderBy(s => s.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public IReadOnlyList<ScenarioSummary> ListNewScenarios(IReadOnlyList<GameSystemSummary> gameSystems)
    {
        ArgumentNullException.ThrowIfNull(gameSystems);

        return gameSystems
            .Select(system => new ScenarioSummary(
                ScenarioId: $"NEW:{system.GameSystemId}",
                GameSystemId: system.GameSystemId,
                Name: $"New {system.Name} Scenario",
                Description: "Start a new scenario with this game system.",
                LastPlayedAt: null))
            .ToList();
    }
}
