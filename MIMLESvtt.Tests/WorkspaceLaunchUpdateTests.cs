using Microsoft.VisualStudio.TestTools.UnitTesting;
using VttMvuView.WorkspaceLaunch;

namespace MIMLESvtt.Tests;

[TestClass]
public class WorkspaceLaunchUpdateTests
{
    [TestMethod]
    public void SelectGameSystem_MovesToScenarioMode()
    {
        var state = BuildInitialState();

        var result = WorkspaceLaunchUpdate.Reduce(state, new SelectGameSystemAction("CHECKERS"));

        Assert.AreEqual("CHECKERS", result.GameSystemPicker.SelectedGameSystemId);
        Assert.AreEqual(WorkspaceLaunchStep.ChooseScenarioMode, result.CurrentStep);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public void ChooseScenarioMode_SavedGame_MovesToChooseSavedGame()
    {
        var state = BuildInitialState() with { CurrentStep = WorkspaceLaunchStep.ChooseScenarioMode };

        var result = WorkspaceLaunchUpdate.Reduce(state, new ChooseScenarioModeAction(ScenarioPickerMode.SavedGame));

        Assert.AreEqual(ScenarioPickerMode.SavedGame, result.ScenarioPicker.Mode);
        Assert.AreEqual(WorkspaceLaunchStep.ChooseSavedGame, result.CurrentStep);
    }

    [TestMethod]
    public void Continue_WithoutScenarioSelection_SetsError()
    {
        var state = BuildInitialState() with
        {
            CurrentStep = WorkspaceLaunchStep.ChooseNewScenario,
            ScenarioPicker = BuildInitialState().ScenarioPicker with { SelectedScenarioId = null }
        };

        var result = WorkspaceLaunchUpdate.Reduce(state, new ContinueAction());

        Assert.AreEqual(WorkspaceLaunchStep.ChooseNewScenario, result.CurrentStep);
        Assert.AreEqual("Select a scenario before continuing.", result.ErrorMessage);
    }

    [TestMethod]
    public void Continue_WithScenarioSelection_MovesToConfirmLaunch()
    {
        var state = BuildInitialState() with
        {
            CurrentStep = WorkspaceLaunchStep.ChooseNewScenario,
            ScenarioPicker = BuildInitialState().ScenarioPicker with { SelectedScenarioId = "SCENARIO-ALPHA" }
        };

        var result = WorkspaceLaunchUpdate.Reduce(state, new ContinueAction());

        Assert.AreEqual(WorkspaceLaunchStep.ConfirmLaunch, result.CurrentStep);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public void ExecuteLaunchAction_OnConfirmLaunch_ProducesNavigateEffect()
    {
        var state = BuildInitialState() with
        {
            CurrentStep = WorkspaceLaunchStep.ConfirmLaunch,
            ScenarioPicker = BuildInitialState().ScenarioPicker with
            {
                Mode = ScenarioPickerMode.SavedGame,
                SelectedScenarioId = "D:\\sessions\\saved.vttsession.json"
            }
        };

        var result = WorkspaceLaunchUpdate.ReduceWithEffects(state, new ExecuteLaunchAction());

        Assert.AreEqual(1, result.Effects.Count);
        Assert.IsInstanceOfType<NavigateToWorkspaceEffect>(result.Effects[0]);
        var effect = (NavigateToWorkspaceEffect)result.Effects[0];
        Assert.AreEqual("/tabletop?mode=play&source=D%3A%5Csessions%5Csaved.vttsession.json", effect.LaunchPath);
        Assert.AreEqual(effect.LaunchPath, result.State.LaunchPath);
    }

    [TestMethod]
    public void ExecuteLaunchAction_WithoutConfirmLaunch_SetsErrorAndNoEffects()
    {
        var state = BuildInitialState() with { CurrentStep = WorkspaceLaunchStep.ChooseSavedGame };

        var result = WorkspaceLaunchUpdate.ReduceWithEffects(state, new ExecuteLaunchAction());

        Assert.AreEqual(0, result.Effects.Count);
        Assert.AreEqual("Complete launch confirmation before opening workspace.", result.State.ErrorMessage);
    }

    [TestMethod]
    public void NavigationRules_CanContinue_RespectsStepRequirements()
    {
        var missingSystemState = BuildInitialState() with
        {
            CurrentStep = WorkspaceLaunchStep.ChooseGameSystem,
            GameSystemPicker = BuildInitialState().GameSystemPicker with { SelectedGameSystemId = null }
        };
        Assert.IsFalse(WorkspaceLaunchNavigationRules.CanContinue(missingSystemState));

        var missingScenarioState = BuildInitialState() with
        {
            CurrentStep = WorkspaceLaunchStep.ChooseSavedGame,
            ScenarioPicker = BuildInitialState().ScenarioPicker with { SelectedScenarioId = null }
        };
        Assert.IsFalse(WorkspaceLaunchNavigationRules.CanContinue(missingScenarioState));

        var confirmState = BuildInitialState() with { CurrentStep = WorkspaceLaunchStep.ConfirmLaunch };
        Assert.IsTrue(WorkspaceLaunchNavigationRules.CanContinue(confirmState));
    }

    [TestMethod]
    public void NavigationRules_CanGoBack_IsFalseOnFirstStep()
    {
        var firstStepState = BuildInitialState() with { CurrentStep = WorkspaceLaunchStep.ChooseGameSystem };
        var laterStepState = BuildInitialState() with { CurrentStep = WorkspaceLaunchStep.ChooseScenarioMode };

        Assert.IsFalse(WorkspaceLaunchNavigationRules.CanGoBack(firstStepState));
        Assert.IsTrue(WorkspaceLaunchNavigationRules.CanGoBack(laterStepState));
    }

    [TestMethod]
    public void NavigationRules_CanExecuteLaunch_RequiresConfirmStepAndLaunchPath()
    {
        var missingPathState = BuildInitialState() with
        {
            CurrentStep = WorkspaceLaunchStep.ConfirmLaunch,
            LaunchPath = null
        };
        Assert.IsFalse(WorkspaceLaunchNavigationRules.CanExecuteLaunch(missingPathState));

        var readyState = BuildInitialState() with
        {
            CurrentStep = WorkspaceLaunchStep.ConfirmLaunch,
            GameSystemPicker = BuildInitialState().GameSystemPicker with { SelectedGameSystemId = "CHECKERS" },
            ScenarioPicker = BuildInitialState().ScenarioPicker with { SelectedScenarioId = "SCENARIO-ALPHA" },
            LaunchPath = "/tabletop?mode=edit&source=SCENARIO-ALPHA"
        };
        Assert.IsTrue(WorkspaceLaunchNavigationRules.CanExecuteLaunch(readyState));
    }

    private static WorkspaceLaunchViewState BuildInitialState()
    {
        return new WorkspaceLaunchViewState(
            new GameSystemPickerViewState(
            [
                new GameSystemCardViewModel("CHECKERS", "Checkers", "desc", null, true, true)
            ],
            null,
            false),
            new ScenarioPickerViewState(
            [
                new ScenarioCardViewModel("SCENARIO-ALPHA", "Scenario Alpha", "desc", "Checkers", null)
            ],
            null,
            ScenarioPickerMode.NewScenario,
            false),
            WorkspaceLaunchStep.ChooseGameSystem,
            null,
            null);
    }
}
