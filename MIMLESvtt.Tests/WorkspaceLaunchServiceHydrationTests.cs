using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.Services;
using VttMvuView.WorkspaceLaunch;
using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Persistence.Snapshot;
using MIMLESvtt.src.Domain.Persistence.VttSessionNSPC;

namespace MIMLESvtt.Tests;

[TestClass]
public class WorkspaceLaunchServiceHydrationTests
{
    [TestMethod]
    public void ModuleImportService_ListAvailableGameSystems_ReturnsSeededAndDiscoveredIds()
    {
        var dataRootPath = BuildTempDataRootPath();

        try
        {
            var workspace = new VttSessionWorkspaceService(dataRootPath);
            workspace.SetCanCreateSession(true);
            workspace.CreateNewGamebox("TEST-MVU-BOX");

            var moduleService = new ModuleImportService(workspace);
            var gameSystems = moduleService.ListAvailableGameSystems();

            Assert.IsTrue(gameSystems.Any(id => string.Equals(id, "TEST-MVU-BOX", StringComparison.OrdinalIgnoreCase)));
            Assert.IsTrue(gameSystems.Any(id => string.Equals(id, "EMPTY-GAMEBOX", StringComparison.OrdinalIgnoreCase)));
        }
        finally
        {
            DeleteDirectoryIfExists(dataRootPath);
        }
    }

    [TestMethod]
    public void WorkspaceLaunchUpdate_ChooseSavedGame_UsesSavedGameStep()
    {
        var state = BuildInitialState();

        var result = WorkspaceLaunchUpdate.Reduce(state, new ChooseScenarioModeAction(ScenarioPickerMode.SavedGame));

        Assert.AreEqual(WorkspaceLaunchStep.ChooseSavedGame, result.CurrentStep);
    }

    private static WorkspaceLaunchViewState BuildInitialState()
    {
        return new WorkspaceLaunchViewState(
            new GameSystemPickerViewState(
            [
                new GameSystemCardViewModel("EMPTY-GAMEBOX", "Empty", "desc", null, true, true)
            ],
            "EMPTY-GAMEBOX",
            false),
            new ScenarioPickerViewState(
            [
                new ScenarioCardViewModel("NEW:EMPTY-GAMEBOX", "New Empty", "desc", "Empty", null)
            ],
            "NEW:EMPTY-GAMEBOX",
            ScenarioPickerMode.NewScenario,
            false),
            WorkspaceLaunchStep.ChooseGameSystem,
            null,
            null);
    }

    private static string BuildTempDataRootPath()
    {
        return Path.Combine(Path.GetTempPath(), $"mimlesvtt-workspace-launch-tests-{Guid.NewGuid():N}");
    }

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}
