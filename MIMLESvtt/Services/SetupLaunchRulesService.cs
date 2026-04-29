using VttMvuModel.Workspace;

namespace MIMLESvtt.Services;

public static class SetupLaunchRulesService
{
    public static bool CanExecuteLaunch(SetupLaunchReadiness readiness)
    {
        ArgumentNullException.ThrowIfNull(readiness);

        if (!readiness.IsConfirmStep || !readiness.HasLaunchPath)
        {
            return false;
        }

        return SetupNavigationRulesService.CanContinue(readiness.Navigation);
    }
}
