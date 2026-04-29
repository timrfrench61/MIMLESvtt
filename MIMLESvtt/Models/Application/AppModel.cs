namespace VttMvuModel.Application;

using VttMvuModel.Workspace;
using VttMvuModel.Table;

public sealed record AppModel(WorkspaceModel Workspace, TableSession? ActiveSession, AppStatus Status, string? LastError)
{
    public static AppModel Empty => new(WorkspaceModel.Empty, null, AppStatus.NotStarted, null);
}
