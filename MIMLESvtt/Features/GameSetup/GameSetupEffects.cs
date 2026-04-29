namespace VttMvuView.GameSetup;

public interface IGameSetupEffect;

public sealed record ExecuteLaunchEffect(
    string LaunchPath,
    GameSetupSessionMode SessionMode,
    string? SelectedGameSystemId,
    string? SelectedSessionSourceId,
    string CampaignName) : IGameSetupEffect;

public sealed record GameSetupUpdateResult(GameSetupState State, IReadOnlyList<IGameSetupEffect> Effects);
