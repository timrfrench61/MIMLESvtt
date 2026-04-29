namespace VttMvuView.WorkspaceLaunch;

public interface IWorkspaceLaunchEffect;

public sealed record NavigateToWorkspaceEffect(string LaunchPath) : IWorkspaceLaunchEffect;

public sealed record WorkspaceLaunchUpdateResult(WorkspaceLaunchViewState State, IReadOnlyList<IWorkspaceLaunchEffect> Effects);
