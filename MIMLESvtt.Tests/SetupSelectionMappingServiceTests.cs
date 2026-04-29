using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.Services;
using VttMvuModel.Workspace;
using VttMvuView.WorkspaceLaunch;

namespace MIMLESvtt.Tests;

[TestClass]
public class SetupSelectionMappingServiceTests
{
    [TestMethod]
    public void SetupGameSystemSummaryProjection_FromSelectedGameSystem_UsesFallbackWhenMissing()
    {
        var result = SetupGameSystemSummaryProjectionService.FromSelectedGameSystem(null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("EMPTY-GAMEBOX", result[0].GameSystemId);
        Assert.AreEqual("EMPTY-GAMEBOX", result[0].Name);
    }

    [TestMethod]
    public void SetupGameSystemSummaryProjection_FromWorkspaceLaunchCards_MapsFields()
    {
        var cards = new List<GameSystemCardViewModel>
        {
            new("CHECKERS", "Checkers", "Checkers module", null, true, true)
        };

        var result = SetupGameSystemSummaryProjectionService.FromWorkspaceLaunchCards(cards);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("CHECKERS", result[0].GameSystemId);
        Assert.AreEqual("Checkers", result[0].Name);
        Assert.AreEqual("Checkers module", result[0].Description);
    }

    [TestMethod]
    public void ToGameSetupGameSystemOptions_MapsFields()
    {
        var systems = new List<GameSystemSummary>
        {
            new("CHECKERS", "Checkers", "local", "Checkers module")
        };

        var result = SetupSelectionMappingService.ToGameSetupGameSystemOptions(systems);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("CHECKERS", result[0].GameSystemId);
        Assert.AreEqual("Checkers", result[0].Name);
        Assert.AreEqual("Checkers module", result[0].Description);
    }

    [TestMethod]
    public void ToGameSetupSessionSourceOptions_MapsFields()
    {
        var scenarios = new List<ScenarioSummary>
        {
            new("S1", "CHECKERS", "Scenario One", "desc", null)
        };

        var result = SetupSelectionMappingService.ToGameSetupSessionSourceOptions(scenarios);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("S1", result[0].SourceId);
        Assert.AreEqual("Scenario One", result[0].Name);
        Assert.AreEqual("desc", result[0].Description);
    }

    [TestMethod]
    public void ToWorkspaceLaunchScenarioCards_UsesFallbackGameSystemName_WhenMissing()
    {
        var scenarios = new List<ScenarioSummary>
        {
            new("S1", "", "Scenario One", "desc", null)
        };

        var result = SetupSelectionMappingService.ToWorkspaceLaunchScenarioCards(scenarios, fallbackGameSystemName: "Saved Session");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Saved Session", result[0].GameSystemName);
    }
}
