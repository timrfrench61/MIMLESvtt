namespace VttMvuView.WorkspaceLaunch;

public interface IWorkspaceLaunchAction;

public sealed record SelectGameSystemAction(string GameSystemId) : IWorkspaceLaunchAction;

public sealed record ChooseScenarioModeAction(ScenarioPickerMode Mode) : IWorkspaceLaunchAction;

public sealed record SelectScenarioAction(string ScenarioId) : IWorkspaceLaunchAction;

public sealed record ContinueAction : IWorkspaceLaunchAction;

public sealed record ExecuteLaunchAction : IWorkspaceLaunchAction;

public sealed record GoBackAction : IWorkspaceLaunchAction;

public sealed record ClearErrorAction : IWorkspaceLaunchAction;
