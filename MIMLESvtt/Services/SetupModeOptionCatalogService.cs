using MIMLESvtt.Components.Shared;
using VttMvuView.GameSetup;
using VttMvuView.WorkspaceLaunch;

namespace MIMLESvtt.Services;

public static class SetupModeOptionCatalogService
{
    private static readonly IReadOnlyList<SetupOptionButtonItem> GameSetupSessionModeOptions =
    [
        new SetupOptionButtonItem(GameSetupSessionMode.NewGame.ToString(), "New Game"),
        new SetupOptionButtonItem(GameSetupSessionMode.LoadSavedGame.ToString(), "Load Saved Game")
    ];

    private static readonly IReadOnlyList<SetupOptionButtonItem> WorkspaceScenarioModeOptions =
    [
        new SetupOptionButtonItem(ScenarioPickerMode.NewScenario.ToString(), "New Scenario"),
        new SetupOptionButtonItem(ScenarioPickerMode.SavedGame.ToString(), "Saved Game")
    ];

    public static IReadOnlyList<SetupOptionButtonItem> GameSetupSessionModes()
    {
        return GameSetupSessionModeOptions;
    }

    public static IReadOnlyList<SetupOptionButtonItem> WorkspaceScenarioModes()
    {
        return WorkspaceScenarioModeOptions;
    }
}
