namespace VttMvuView.WorkspaceLaunch;

public static class WorkspaceLaunchUpdate
{
    public static WorkspaceLaunchViewState Reduce(WorkspaceLaunchViewState state, IWorkspaceLaunchAction action)
    {
        return ReduceWithEffects(state, action).State;
    }

    public static WorkspaceLaunchUpdateResult ReduceWithEffects(WorkspaceLaunchViewState state, IWorkspaceLaunchAction action)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(action);

        var nextState = action switch
        {
            SelectGameSystemAction select => ReduceSelectGameSystem(state, select),
            ChooseScenarioModeAction chooseMode => ReduceChooseScenarioMode(state, chooseMode),
            SelectScenarioAction selectScenario => state with
            {
                ScenarioPicker = state.ScenarioPicker with { SelectedScenarioId = selectScenario.ScenarioId },
                ErrorMessage = null,
                LaunchPath = null
            },
            ContinueAction => ReduceContinue(state),
            GoBackAction => ReduceGoBack(state),
            ClearErrorAction => state with { ErrorMessage = null, LaunchPath = null },
            _ => state
        };

        if (action is not ExecuteLaunchAction)
        {
            return new WorkspaceLaunchUpdateResult(nextState, []);
        }

        var launchState = string.IsNullOrWhiteSpace(nextState.LaunchPath)
            ? nextState with { LaunchPath = BuildLaunchPath(nextState) }
            : nextState;

        if (!WorkspaceLaunchNavigationRules.CanExecuteLaunch(launchState))
        {
            return new WorkspaceLaunchUpdateResult(
                nextState with { ErrorMessage = "Complete launch confirmation before opening workspace." },
                []);
        }

        var launchPath = launchState.LaunchPath!;
        return new WorkspaceLaunchUpdateResult(
            nextState with { LaunchPath = launchPath },
            [new NavigateToWorkspaceEffect(launchPath)]);
    }

    private static WorkspaceLaunchViewState ReduceSelectGameSystem(WorkspaceLaunchViewState state, SelectGameSystemAction action)
    {
        if (string.IsNullOrWhiteSpace(action.GameSystemId))
        {
            return state with { ErrorMessage = "Select a valid game system." };
        }

        return state with
        {
            GameSystemPicker = state.GameSystemPicker with { SelectedGameSystemId = action.GameSystemId },
            CurrentStep = WorkspaceLaunchStep.ChooseScenarioMode,
            ErrorMessage = null,
            LaunchPath = null
        };
    }

    private static WorkspaceLaunchViewState ReduceChooseScenarioMode(WorkspaceLaunchViewState state, ChooseScenarioModeAction action)
    {
        var nextStep = action.Mode == ScenarioPickerMode.SavedGame
            ? WorkspaceLaunchStep.ChooseSavedGame
            : WorkspaceLaunchStep.ChooseNewScenario;

        return state with
        {
            ScenarioPicker = state.ScenarioPicker with { Mode = action.Mode, SelectedScenarioId = null },
            CurrentStep = nextStep,
            ErrorMessage = null,
            LaunchPath = null
        };
    }

    private static WorkspaceLaunchViewState ReduceContinue(WorkspaceLaunchViewState state)
    {
        return state.CurrentStep switch
        {
            WorkspaceLaunchStep.ChooseScenarioMode => state with
            {
                CurrentStep = state.ScenarioPicker.Mode == ScenarioPickerMode.SavedGame
                    ? WorkspaceLaunchStep.ChooseSavedGame
                    : WorkspaceLaunchStep.ChooseNewScenario,
                ErrorMessage = null,
                LaunchPath = null
            },
            WorkspaceLaunchStep.ChooseSavedGame or WorkspaceLaunchStep.ChooseNewScenario =>
                string.IsNullOrWhiteSpace(state.ScenarioPicker.SelectedScenarioId)
                    ? state with { ErrorMessage = "Select a scenario before continuing." }
                    : state with { CurrentStep = WorkspaceLaunchStep.ConfirmLaunch, ErrorMessage = null, LaunchPath = null },
            _ => state
        };
    }

    private static WorkspaceLaunchViewState ReduceGoBack(WorkspaceLaunchViewState state)
    {
        var previousStep = state.CurrentStep switch
        {
            WorkspaceLaunchStep.ChooseScenarioMode => WorkspaceLaunchStep.ChooseGameSystem,
            WorkspaceLaunchStep.ChooseSavedGame => WorkspaceLaunchStep.ChooseScenarioMode,
            WorkspaceLaunchStep.ChooseNewScenario => WorkspaceLaunchStep.ChooseScenarioMode,
            WorkspaceLaunchStep.ConfirmLaunch => state.ScenarioPicker.Mode == ScenarioPickerMode.SavedGame
                ? WorkspaceLaunchStep.ChooseSavedGame
                : WorkspaceLaunchStep.ChooseNewScenario,
            _ => state.CurrentStep
        };

        return state with
        {
            CurrentStep = previousStep,
            ErrorMessage = null,
            LaunchPath = null
        };
    }

    private static string BuildLaunchPath(WorkspaceLaunchViewState state)
    {
        var mode = state.ScenarioPicker.Mode == ScenarioPickerMode.SavedGame ? "play" : "edit";
        var source = string.IsNullOrWhiteSpace(state.ScenarioPicker.SelectedScenarioId)
            ? string.Empty
            : $"&source={Uri.EscapeDataString(state.ScenarioPicker.SelectedScenarioId)}";

        return $"/tabletop?mode={mode}{source}";
    }
}
