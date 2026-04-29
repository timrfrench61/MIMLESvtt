namespace VttMvuModel.Workspace;

public sealed record SetupNavigationReadiness(
    bool IsLoading,
    bool HasGameSystemSelection,
    bool HasSourceSelection,
    bool IsFirstStep,
    bool RequiresSourceSelection);
