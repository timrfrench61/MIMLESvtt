namespace VttMvuModel.Workspace;

public sealed record WorkspaceModel(IReadOnlyList<GameSystemSummary> AvailableGameSystems, GameSystemSummary? SelectedGameSystem, IReadOnlyList<ScenarioSummary> AvailableScenarios, ScenarioSummary? SelectedScenario, WorkspaceLaunchMode LaunchMode)
{
    public static WorkspaceModel Empty => new(Array.Empty<GameSystemSummary>(), null, Array.Empty<ScenarioSummary>(), null, WorkspaceLaunchMode.None);
}
