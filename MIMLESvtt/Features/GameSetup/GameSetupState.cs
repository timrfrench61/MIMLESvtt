namespace VttMvuView.GameSetup;

public enum GameSetupStep
{
    ChooseGameSystem,
    ChooseSessionMode,
    ChooseSessionSource,
    ConfirmLaunch
}

public enum GameSetupSessionMode
{
    NewGame,
    LoadSavedGame
}

public sealed record GameSetupGameSystemOption(string GameSystemId, string Name, string Description);

public sealed record GameSetupSessionSourceOption(string SourceId, string Name, string Description);

public sealed record GameSetupState(
    IReadOnlyList<GameSetupGameSystemOption> AvailableGameSystems,
    string? SelectedGameSystemId,
    GameSetupSessionMode SessionMode,
    IReadOnlyList<GameSetupSessionSourceOption> SessionSources,
    bool IsLoading,
    string? SelectedSessionSourceId,
    GameSetupStep CurrentStep,
    string? ErrorMessage,
    string? LaunchPath);
