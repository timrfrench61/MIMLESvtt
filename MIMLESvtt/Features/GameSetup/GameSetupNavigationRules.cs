using MIMLESvtt.Services;
using VttMvuModel.Workspace;

namespace VttMvuView.GameSetup;

public static class GameSetupNavigationRules
{
    public static bool CanGoBack(GameSetupState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        var readiness = BuildReadiness(state);
        return SetupNavigationRulesService.CanGoBack(readiness);
    }

    public static bool CanContinue(GameSetupState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        var readiness = BuildReadiness(state);
        return SetupNavigationRulesService.CanContinue(readiness);
    }

    public static bool CanExecuteLaunch(GameSetupState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        var launchReadiness = SetupReadinessBuilder.CreateLaunchReadiness(
            navigation: BuildReadiness(state),
            isConfirmStep: state.CurrentStep == GameSetupStep.ConfirmLaunch,
            hasLaunchPath: !string.IsNullOrWhiteSpace(state.LaunchPath));

        return SetupLaunchRulesService.CanExecuteLaunch(launchReadiness);
    }

    private static SetupNavigationReadiness BuildReadiness(GameSetupState state)
    {
        var isFirstStep = state.CurrentStep == GameSetupStep.ChooseGameSystem;
        var requiresSourceSelection = state.CurrentStep == GameSetupStep.ChooseSessionSource;

        var hasGameSystemSelection = state.CurrentStep != GameSetupStep.ChooseGameSystem
            || !string.IsNullOrWhiteSpace(state.SelectedGameSystemId);

        var hasSourceSelection = !requiresSourceSelection || !string.IsNullOrWhiteSpace(state.SelectedSessionSourceId);

        return SetupReadinessBuilder.CreateNavigationReadiness(
            isLoading: state.IsLoading,
            hasGameSystemSelection: hasGameSystemSelection,
            hasSourceSelection: hasSourceSelection,
            isFirstStep: isFirstStep,
            requiresSourceSelection: requiresSourceSelection);
    }
}
