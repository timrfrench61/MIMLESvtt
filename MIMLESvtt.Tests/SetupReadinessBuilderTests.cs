using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.Services;

namespace MIMLESvtt.Tests;

[TestClass]
public class SetupReadinessBuilderTests
{
    [TestMethod]
    public void CreateNavigationReadiness_MapsAllFields()
    {
        var readiness = SetupReadinessBuilder.CreateNavigationReadiness(
            isLoading: true,
            hasGameSystemSelection: false,
            hasSourceSelection: true,
            isFirstStep: false,
            requiresSourceSelection: true);

        Assert.IsTrue(readiness.IsLoading);
        Assert.IsFalse(readiness.HasGameSystemSelection);
        Assert.IsTrue(readiness.HasSourceSelection);
        Assert.IsFalse(readiness.IsFirstStep);
        Assert.IsTrue(readiness.RequiresSourceSelection);
    }

    [TestMethod]
    public void CreateLaunchReadiness_MapsNavigationAndFlags()
    {
        var navigation = SetupReadinessBuilder.CreateNavigationReadiness(
            isLoading: false,
            hasGameSystemSelection: true,
            hasSourceSelection: true,
            isFirstStep: false,
            requiresSourceSelection: false);

        var launchReadiness = SetupReadinessBuilder.CreateLaunchReadiness(
            navigation,
            isConfirmStep: true,
            hasLaunchPath: true);

        Assert.AreSame(navigation, launchReadiness.Navigation);
        Assert.IsTrue(launchReadiness.IsConfirmStep);
        Assert.IsTrue(launchReadiness.HasLaunchPath);
    }
}
