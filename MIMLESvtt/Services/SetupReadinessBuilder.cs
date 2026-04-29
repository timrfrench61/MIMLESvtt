using VttMvuModel.Workspace;

namespace MIMLESvtt.Services;

public static class SetupReadinessBuilder
{
    public static SetupNavigationReadiness CreateNavigationReadiness(
        bool isLoading,
        bool hasGameSystemSelection,
        bool hasSourceSelection,
        bool isFirstStep,
        bool requiresSourceSelection)
    {
        return new SetupNavigationReadiness(
            IsLoading: isLoading,
            HasGameSystemSelection: hasGameSystemSelection,
            HasSourceSelection: hasSourceSelection,
            IsFirstStep: isFirstStep,
            RequiresSourceSelection: requiresSourceSelection);
    }

    public static SetupLaunchReadiness CreateLaunchReadiness(
        SetupNavigationReadiness navigation,
        bool isConfirmStep,
        bool hasLaunchPath)
    {
        return new SetupLaunchReadiness(
            Navigation: navigation,
            IsConfirmStep: isConfirmStep,
            HasLaunchPath: hasLaunchPath);
    }
}
