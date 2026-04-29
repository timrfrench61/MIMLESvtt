using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.Services;
using VttMvuModel.Workspace;

namespace MIMLESvtt.Tests;

[TestClass]
public class SetupNavigationRulesServiceTests
{
    [TestMethod]
    public void CanGoBack_ReturnsFalse_WhenFirstStep()
    {
        var readiness = new SetupNavigationReadiness(
            IsLoading: false,
            HasGameSystemSelection: false,
            HasSourceSelection: false,
            IsFirstStep: true,
            RequiresSourceSelection: false);

        Assert.IsFalse(SetupNavigationRulesService.CanGoBack(readiness));
    }

    [TestMethod]
    public void CanContinue_ReturnsFalse_WhenLoading()
    {
        var readiness = new SetupNavigationReadiness(
            IsLoading: true,
            HasGameSystemSelection: true,
            HasSourceSelection: true,
            IsFirstStep: false,
            RequiresSourceSelection: true);

        Assert.IsFalse(SetupNavigationRulesService.CanContinue(readiness));
    }

    [TestMethod]
    public void CanContinue_ReturnsFalse_WhenGameSystemMissing()
    {
        var readiness = new SetupNavigationReadiness(
            IsLoading: false,
            HasGameSystemSelection: false,
            HasSourceSelection: true,
            IsFirstStep: false,
            RequiresSourceSelection: false);

        Assert.IsFalse(SetupNavigationRulesService.CanContinue(readiness));
    }

    [TestMethod]
    public void CanContinue_ReturnsFalse_WhenSourceRequiredAndMissing()
    {
        var readiness = new SetupNavigationReadiness(
            IsLoading: false,
            HasGameSystemSelection: true,
            HasSourceSelection: false,
            IsFirstStep: false,
            RequiresSourceSelection: true);

        Assert.IsFalse(SetupNavigationRulesService.CanContinue(readiness));
    }

    [TestMethod]
    public void CanContinue_ReturnsTrue_WhenReady()
    {
        var readiness = new SetupNavigationReadiness(
            IsLoading: false,
            HasGameSystemSelection: true,
            HasSourceSelection: true,
            IsFirstStep: false,
            RequiresSourceSelection: true);

        Assert.IsTrue(SetupNavigationRulesService.CanContinue(readiness));
    }
}
