using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.Services;

namespace MIMLESvtt.Tests;

[TestClass]
public class SetupModeOptionCatalogServiceTests
{
    [TestMethod]
    public void GameSetupSessionModes_ReturnsNewAndLoadSavedGameOptions()
    {
        var result = SetupModeOptionCatalogService.GameSetupSessionModes();

        Assert.AreEqual(2, result.Count);
        CollectionAssert.AreEqual(
            new[] { "NewGame", "LoadSavedGame" },
            result.Select(item => item.Id).ToArray());
        CollectionAssert.AreEqual(
            new[] { "New Game", "Load Saved Game" },
            result.Select(item => item.Name).ToArray());
    }

    [TestMethod]
    public void WorkspaceScenarioModes_ReturnsNewAndSavedGameOptions()
    {
        var result = SetupModeOptionCatalogService.WorkspaceScenarioModes();

        Assert.AreEqual(2, result.Count);
        CollectionAssert.AreEqual(
            new[] { "NewScenario", "SavedGame" },
            result.Select(item => item.Id).ToArray());
        CollectionAssert.AreEqual(
            new[] { "New Scenario", "Saved Game" },
            result.Select(item => item.Name).ToArray());
    }

    [TestMethod]
    public void GameSetupSessionModes_ReturnsSameCachedReferenceAcrossCalls()
    {
        var first = SetupModeOptionCatalogService.GameSetupSessionModes();
        var second = SetupModeOptionCatalogService.GameSetupSessionModes();

        Assert.AreSame(first, second);
    }

    [TestMethod]
    public void WorkspaceScenarioModes_ReturnsSameCachedReferenceAcrossCalls()
    {
        var first = SetupModeOptionCatalogService.WorkspaceScenarioModes();
        var second = SetupModeOptionCatalogService.WorkspaceScenarioModes();

        Assert.AreSame(first, second);
    }
}
