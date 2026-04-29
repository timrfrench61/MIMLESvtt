using Microsoft.VisualStudio.TestTools.UnitTesting;
using VttMvuView.GameSetup;

namespace MIMLESvtt.Tests;

[TestClass]
public class GameSetupUpdateTests
{
    [TestMethod]
    public void SelectGameSystem_MovesToChooseSessionMode()
    {
        var state = BuildInitialState();

        var result = GameSetupUpdate.Reduce(state, new SelectGameSystemAction("CHECKERS"));

        Assert.AreEqual("CHECKERS", result.SelectedGameSystemId);
        Assert.AreEqual(GameSetupStep.ChooseSessionMode, result.CurrentStep);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public void ChooseSessionMode_MovesToChooseSessionSource()
    {
        var state = BuildInitialState() with { CurrentStep = GameSetupStep.ChooseSessionMode };

        var result = GameSetupUpdate.Reduce(state, new ChooseSessionModeAction(GameSetupSessionMode.LoadSavedGame));

        Assert.AreEqual(GameSetupSessionMode.LoadSavedGame, result.SessionMode);
        Assert.AreEqual(GameSetupStep.ChooseSessionSource, result.CurrentStep);
    }

    [TestMethod]
    public void Continue_WithoutSessionSource_SetsValidationError()
    {
        var state = BuildInitialState() with
        {
            CurrentStep = GameSetupStep.ChooseSessionSource,
            SelectedSessionSourceId = null
        };

        var result = GameSetupUpdate.Reduce(state, new ContinueAction());

        Assert.AreEqual(GameSetupStep.ChooseSessionSource, result.CurrentStep);
        Assert.AreEqual("Select a session source before continuing.", result.ErrorMessage);
    }

    [TestMethod]
    public void Continue_WithSessionSource_MovesToConfirmLaunch_AndBuildsLaunchPath()
    {
        var state = BuildInitialState() with
        {
            CurrentStep = GameSetupStep.ChooseSessionSource,
            SessionMode = GameSetupSessionMode.LoadSavedGame,
            SelectedSessionSourceId = "SAVED-ALPHA"
        };

        var result = GameSetupUpdate.Reduce(state, new ContinueAction());

        Assert.AreEqual(GameSetupStep.ConfirmLaunch, result.CurrentStep);
        Assert.AreEqual("/tabletop?mode=play&source=SAVED-ALPHA", result.LaunchPath);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public void ExecuteLaunchAction_OnConfirmLaunch_ProducesExecuteLaunchEffect()
    {
        var state = BuildInitialState() with
        {
            CurrentStep = GameSetupStep.ConfirmLaunch,
            SessionMode = GameSetupSessionMode.NewGame,
            SelectedGameSystemId = "CHECKERS",
            LaunchPath = "/tabletop?mode=edit&source=CHECKERS"
        };

        var result = GameSetupUpdate.ReduceWithEffects(state, new ExecuteLaunchAction());

        Assert.AreEqual(1, result.Effects.Count);
        Assert.IsInstanceOfType<ExecuteLaunchEffect>(result.Effects[0]);

        var effect = (ExecuteLaunchEffect)result.Effects[0];
        Assert.AreEqual("CHECKERS", effect.SelectedGameSystemId);
        Assert.AreEqual(GameSetupSessionMode.NewGame, effect.SessionMode);
        Assert.AreEqual("/tabletop?mode=edit&source=CHECKERS", effect.LaunchPath);
    }

    [TestMethod]
    public void ExecuteLaunchAction_WithoutConfirmLaunch_SetsErrorAndNoEffects()
    {
        var state = BuildInitialState() with
        {
            CurrentStep = GameSetupStep.ChooseSessionSource,
            LaunchPath = null
        };

        var result = GameSetupUpdate.ReduceWithEffects(state, new ExecuteLaunchAction());

        Assert.AreEqual(0, result.Effects.Count);
        Assert.AreEqual("Complete setup confirmation before launching.", result.State.ErrorMessage);
    }

    [TestMethod]
    public void NavigationRules_CanContinue_RespectsLoadingAndStepRequirements()
    {
        var loadingState = BuildInitialState() with { IsLoading = true };
        Assert.IsFalse(GameSetupNavigationRules.CanContinue(loadingState));

        var missingSystemState = BuildInitialState() with
        {
            IsLoading = false,
            CurrentStep = GameSetupStep.ChooseGameSystem,
            SelectedGameSystemId = null
        };
        Assert.IsFalse(GameSetupNavigationRules.CanContinue(missingSystemState));

        var missingSourceState = BuildInitialState() with
        {
            CurrentStep = GameSetupStep.ChooseSessionSource,
            SelectedSessionSourceId = null
        };
        Assert.IsFalse(GameSetupNavigationRules.CanContinue(missingSourceState));

        var confirmState = BuildInitialState() with { CurrentStep = GameSetupStep.ConfirmLaunch };
        Assert.IsTrue(GameSetupNavigationRules.CanContinue(confirmState));
    }

    [TestMethod]
    public void NavigationRules_CanGoBack_IsFalseOnFirstStep()
    {
        var firstStepState = BuildInitialState() with { CurrentStep = GameSetupStep.ChooseGameSystem };
        var laterStepState = BuildInitialState() with { CurrentStep = GameSetupStep.ChooseSessionMode };

        Assert.IsFalse(GameSetupNavigationRules.CanGoBack(firstStepState));
        Assert.IsTrue(GameSetupNavigationRules.CanGoBack(laterStepState));
    }

    [TestMethod]
    public void NavigationRules_CanExecuteLaunch_RequiresConfirmStepAndLaunchPath()
    {
        var confirmWithoutPath = BuildInitialState() with
        {
            CurrentStep = GameSetupStep.ConfirmLaunch,
            LaunchPath = null
        };
        Assert.IsFalse(GameSetupNavigationRules.CanExecuteLaunch(confirmWithoutPath));

        var confirmWithPath = BuildInitialState() with
        {
            CurrentStep = GameSetupStep.ConfirmLaunch,
            SelectedGameSystemId = "CHECKERS",
            LaunchPath = "/tabletop?mode=edit&source=CHECKERS"
        };
        Assert.IsTrue(GameSetupNavigationRules.CanExecuteLaunch(confirmWithPath));
    }

    private static GameSetupState BuildInitialState()
    {
        return new GameSetupState(
        [
            new GameSetupGameSystemOption("CHECKERS", "Checkers", "desc")
        ],
        null,
        GameSetupSessionMode.NewGame,
        [
            new GameSetupSessionSourceOption("NEW-SCENARIO", "New Scenario", "desc")
        ],
        false,
        null,
        GameSetupStep.ChooseGameSystem,
        null,
        null);
    }
}
