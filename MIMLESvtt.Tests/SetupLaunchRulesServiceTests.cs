using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.Services;
using VttMvuModel.Workspace;

namespace MIMLESvtt.Tests;

[TestClass]
public class SetupLaunchRulesServiceTests
{
    [TestMethod]
    public void CanExecuteLaunch_ReturnsFalse_WhenNotConfirmStep()
    {
        var readiness = BuildReadiness(isConfirmStep: false, hasLaunchPath: true, hasSourceSelection: true);
        Assert.IsFalse(SetupLaunchRulesService.CanExecuteLaunch(readiness));
    }

    [TestMethod]
    public void CanExecuteLaunch_ReturnsFalse_WhenLaunchPathMissing()
    {
        var readiness = BuildReadiness(isConfirmStep: true, hasLaunchPath: false, hasSourceSelection: true);
        Assert.IsFalse(SetupLaunchRulesService.CanExecuteLaunch(readiness));
    }

    [TestMethod]
    public void CanExecuteLaunch_ReturnsFalse_WhenNavigationNotReady()
    {
        var readiness = BuildReadiness(isConfirmStep: true, hasLaunchPath: true, hasSourceSelection: false);
        Assert.IsFalse(SetupLaunchRulesService.CanExecuteLaunch(readiness));
    }

    [TestMethod]
    public void CanExecuteLaunch_ReturnsTrue_WhenConfirmStepHasPathAndNavigationReady()
    {
        var readiness = BuildReadiness(isConfirmStep: true, hasLaunchPath: true, hasSourceSelection: true);
        Assert.IsTrue(SetupLaunchRulesService.CanExecuteLaunch(readiness));
    }

    private static SetupLaunchReadiness BuildReadiness(bool isConfirmStep, bool hasLaunchPath, bool hasSourceSelection)
    {
        var navigation = new SetupNavigationReadiness(
            IsLoading: false,
            HasGameSystemSelection: true,
            HasSourceSelection: hasSourceSelection,
            IsFirstStep: false,
            RequiresSourceSelection: true);

        return new SetupLaunchReadiness(
            Navigation: navigation,
            IsConfirmStep: isConfirmStep,
            HasLaunchPath: hasLaunchPath);
    }
}
