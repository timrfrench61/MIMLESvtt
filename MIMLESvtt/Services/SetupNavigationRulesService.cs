using VttMvuModel.Workspace;

namespace MIMLESvtt.Services;

public static class SetupNavigationRulesService
{
    public static bool CanGoBack(SetupNavigationReadiness readiness)
    {
        ArgumentNullException.ThrowIfNull(readiness);
        return !readiness.IsFirstStep;
    }

    public static bool CanContinue(SetupNavigationReadiness readiness)
    {
        ArgumentNullException.ThrowIfNull(readiness);

        if (readiness.IsLoading)
        {
            return false;
        }

        if (!readiness.HasGameSystemSelection)
        {
            return false;
        }

        if (readiness.RequiresSourceSelection && !readiness.HasSourceSelection)
        {
            return false;
        }

        return true;
    }
}
