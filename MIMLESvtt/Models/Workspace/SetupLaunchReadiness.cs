namespace VttMvuModel.Workspace;

public sealed record SetupLaunchReadiness(
    SetupNavigationReadiness Navigation,
    bool IsConfirmStep,
    bool HasLaunchPath);
