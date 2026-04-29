namespace VttMvuView.GameSetup;

public interface IGameSetupAction;

public sealed record SelectGameSystemAction(string GameSystemId) : IGameSetupAction;

public sealed record ChooseSessionModeAction(GameSetupSessionMode SessionMode) : IGameSetupAction;

public sealed record SelectSessionSourceAction(string SessionSourceId) : IGameSetupAction;

public sealed record ContinueAction : IGameSetupAction;

public sealed record ExecuteLaunchAction : IGameSetupAction;

public sealed record GoBackAction : IGameSetupAction;

public sealed record ClearErrorAction : IGameSetupAction;
