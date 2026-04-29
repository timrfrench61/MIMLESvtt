namespace VttMvuView.GameSetup;

public static class GameSetupUpdate
{
    public static GameSetupState Reduce(GameSetupState state, IGameSetupAction action)
    {
        return ReduceWithEffects(state, action).State;
    }

    public static GameSetupUpdateResult ReduceWithEffects(GameSetupState state, IGameSetupAction action)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(action);

        var nextState = action switch
        {
            SelectGameSystemAction select => SelectGameSystem(state, select),
            ChooseSessionModeAction chooseMode => ChooseSessionMode(state, chooseMode),
            SelectSessionSourceAction selectSource => state with
            {
                SelectedSessionSourceId = selectSource.SessionSourceId,
                ErrorMessage = null,
                LaunchPath = null
            },
            ContinueAction => Continue(state),
            GoBackAction => GoBack(state),
            ClearErrorAction => state with { ErrorMessage = null },
            _ => state
        };

        if (action is not ExecuteLaunchAction)
        {
            return new GameSetupUpdateResult(nextState, []);
        }

        var launchState = string.IsNullOrWhiteSpace(nextState.LaunchPath)
            ? nextState with { LaunchPath = BuildLaunchPath(nextState) }
            : nextState;

        if (!GameSetupNavigationRules.CanExecuteLaunch(launchState))
        {
            return new GameSetupUpdateResult(
                nextState with { ErrorMessage = "Complete setup confirmation before launching." },
                []);
        }

        var launchPath = launchState.LaunchPath!;
        var effect = new ExecuteLaunchEffect(
            launchPath,
            launchState.SessionMode,
            launchState.SelectedGameSystemId,
            launchState.SelectedSessionSourceId,
            "New Campaign");

        return new GameSetupUpdateResult(launchState, [effect]);
    }

    private static GameSetupState SelectGameSystem(GameSetupState state, SelectGameSystemAction action)
    {
        if (string.IsNullOrWhiteSpace(action.GameSystemId))
        {
            return state with { ErrorMessage = "Select a valid game system." };
        }

        return state with
        {
            SelectedGameSystemId = action.GameSystemId,
            CurrentStep = GameSetupStep.ChooseSessionMode,
            ErrorMessage = null,
            LaunchPath = null
        };
    }

    private static GameSetupState ChooseSessionMode(GameSetupState state, ChooseSessionModeAction action)
    {
        return state with
        {
            SessionMode = action.SessionMode,
            SelectedSessionSourceId = null,
            CurrentStep = GameSetupStep.ChooseSessionSource,
            ErrorMessage = null,
            LaunchPath = null
        };
    }

    private static GameSetupState Continue(GameSetupState state)
    {
        return state.CurrentStep switch
        {
            GameSetupStep.ChooseGameSystem => string.IsNullOrWhiteSpace(state.SelectedGameSystemId)
                ? state with { ErrorMessage = "Select a game system before continuing." }
                : state with { CurrentStep = GameSetupStep.ChooseSessionMode, ErrorMessage = null },
            GameSetupStep.ChooseSessionMode => state with { CurrentStep = GameSetupStep.ChooseSessionSource, ErrorMessage = null },
            GameSetupStep.ChooseSessionSource => string.IsNullOrWhiteSpace(state.SelectedSessionSourceId)
                ? state with { ErrorMessage = "Select a session source before continuing." }
                : state with
                {
                    CurrentStep = GameSetupStep.ConfirmLaunch,
                    ErrorMessage = null,
                    LaunchPath = BuildLaunchPath(state)
                },
            _ => state
        };
    }

    private static GameSetupState GoBack(GameSetupState state)
    {
        var previousStep = state.CurrentStep switch
        {
            GameSetupStep.ChooseSessionMode => GameSetupStep.ChooseGameSystem,
            GameSetupStep.ChooseSessionSource => GameSetupStep.ChooseSessionMode,
            GameSetupStep.ConfirmLaunch => GameSetupStep.ChooseSessionSource,
            _ => state.CurrentStep
        };

        return state with
        {
            CurrentStep = previousStep,
            ErrorMessage = null,
            LaunchPath = previousStep == GameSetupStep.ConfirmLaunch ? state.LaunchPath : null
        };
    }

    private static string BuildLaunchPath(GameSetupState state)
    {
        var mode = state.SessionMode == GameSetupSessionMode.LoadSavedGame ? "play" : "edit";
        var source = string.IsNullOrWhiteSpace(state.SelectedSessionSourceId)
            ? string.Empty
            : $"&source={Uri.EscapeDataString(state.SelectedSessionSourceId)}";

        return $"/tabletop?mode={mode}{source}";
    }
}
