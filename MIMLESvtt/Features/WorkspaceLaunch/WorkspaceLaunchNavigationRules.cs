using MIMLESvtt.Services;
using VttMvuModel.Workspace;

namespace VttMvuView.WorkspaceLaunch;

public static class WorkspaceLaunchNavigationRules
{
    public static bool CanGoBack(WorkspaceLaunchViewState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        var readiness = BuildReadiness(state);
        return SetupNavigationRulesService.CanGoBack(readiness);
    }

    public static bool CanContinue(WorkspaceLaunchViewState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        var readiness = BuildReadiness(state);
        return SetupNavigationRulesService.CanContinue(readiness);
    }

    public static bool CanExecuteLaunch(WorkspaceLaunchViewState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        var launchReadiness = SetupReadinessBuilder.CreateLaunchReadiness(
            navigation: BuildReadiness(state),
            isConfirmStep: state.CurrentStep == WorkspaceLaunchStep.ConfirmLaunch,
            hasLaunchPath: !string.IsNullOrWhiteSpace(state.LaunchPath));

        return SetupLaunchRulesService.CanExecuteLaunch(launchReadiness);
    }

    private static SetupNavigationReadiness BuildReadiness(WorkspaceLaunchViewState state)
    {
        var isFirstStep = state.CurrentStep == WorkspaceLaunchStep.ChooseGameSystem;
        var requiresSourceSelection = state.CurrentStep is WorkspaceLaunchStep.ChooseSavedGame or WorkspaceLaunchStep.ChooseNewScenario;

        var hasGameSystemSelection = state.CurrentStep != WorkspaceLaunchStep.ChooseGameSystem
            || !string.IsNullOrWhiteSpace(state.GameSystemPicker.SelectedGameSystemId);

        var hasSourceSelection = !requiresSourceSelection || !string.IsNullOrWhiteSpace(state.ScenarioPicker.SelectedScenarioId);

        return SetupReadinessBuilder.CreateNavigationReadiness(
            isLoading: false,
            hasGameSystemSelection: hasGameSystemSelection,
            hasSourceSelection: hasSourceSelection,
            isFirstStep: isFirstStep,
            requiresSourceSelection: requiresSourceSelection);
    }
}
