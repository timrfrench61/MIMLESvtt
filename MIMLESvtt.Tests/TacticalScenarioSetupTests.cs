using Microsoft.AspNetCore.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.Components.Pages;
using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Models.Placement;
using MIMLESvtt.src.Domain.Models.Pieces;
using MIMLESvtt.src.Domain.Models.Surfaces;
using MIMLESvtt.src.Domain.Persistence.Snapshot;
using MIMLESvtt.src.Domain.Persistence.VttSessionNSPC;

namespace MIMLESvtt.Tests;

[TestClass]
public class TacticalScenarioSetupTests
{
    [TestMethod]
    public void SaveSetupAsScenario_WithValidTitleAndPath_SavesScenarioAndShowsSuccessMessage()
    {
        var workspace = new VttSessionWorkspaceService();
        var page = CreatePage(workspace);
        var scenarioPath = CreateTempFilePath("tactical-setup-save", SnapshotFileExtensions.VttScenario);

        try
        {
            workspace.CreateNewSession();
            workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);
            workspace.AddPiece("piece-1", "def-piece-1", "surface-1", 2, 3, string.Empty, 0);

            page.TestSetScenarioTitle("Tactical Setup Scenario");
            page.TestSetScenarioPath(scenarioPath);
            page.TestSaveSetupAsScenario();

            Assert.IsTrue(File.Exists(scenarioPath));
            Assert.AreEqual("Saved tactical setup as scenario snapshot.", page.TestSetupStatusMessage);
        }
        finally
        {
            DeleteFileIfExists(scenarioPath);
        }
    }

    [TestMethod]
    public void SaveSetupAsScenario_WithMissingPath_ShowsValidationMessage()
    {
        var workspace = new VttSessionWorkspaceService();
        var page = CreatePage(workspace);

        workspace.CreateNewSession();
        workspace.AddSurface("surface-1", "def-surface-1", SurfaceType.Map, CoordinateSystem.Square);

        page.TestSetScenarioTitle("Tactical Setup Scenario");
        page.TestSetScenarioPath(string.Empty);
        page.TestSaveSetupAsScenario();

        Assert.AreEqual("Scenario path is required.", page.TestSetupStatusMessage);
    }

    private static TacticalScenarioSetup CreatePage(VttSessionWorkspaceService workspace)
    {
        return new TacticalScenarioSetup
        {
            WorkspaceService = workspace,
            Navigation = new TestNavigationManager()
        };
    }

    private static string CreateTempFilePath(string prefix, string extension)
    {
        return Path.Combine(Path.GetTempPath(), $"{prefix}-{Guid.NewGuid():N}{extension}");
    }

    private static void DeleteFileIfExists(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private sealed class TestNavigationManager : NavigationManager
    {
        public TestNavigationManager()
        {
            Initialize("http://localhost/", "http://localhost/");
        }

        protected override void NavigateToCore(string uri, NavigationOptions options)
        {
            Uri = ToAbsoluteUri(uri).ToString();
        }
    }
}
