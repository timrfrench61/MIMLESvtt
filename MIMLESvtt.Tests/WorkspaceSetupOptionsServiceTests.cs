using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.Services;
using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Persistence.Snapshot;
using MIMLESvtt.src.Domain.Persistence.VttSessionNSPC;

namespace MIMLESvtt.Tests;

[TestClass]
public class WorkspaceSetupOptionsServiceTests
{
    [TestMethod]
    public void ListGameSystems_MapsAvailableGameSystemsToSummaries()
    {
        var dataRootPath = BuildTempDataRootPath();

        try
        {
            var workspace = new VttSessionWorkspaceService(dataRootPath);
            workspace.SetCanCreateSession(true);
            workspace.CreateNewGamebox("SERVICE-SETUP-BOX");

            var moduleService = new ModuleImportService(workspace);
            var loadService = new LoadGameService(workspace);
            var setupService = new WorkspaceSetupOptionsService(moduleService, loadService);

            var gameSystems = setupService.ListGameSystems();

            Assert.IsTrue(gameSystems.Any(g => string.Equals(g.GameSystemId, "SERVICE-SETUP-BOX", StringComparison.OrdinalIgnoreCase)));
            Assert.IsTrue(gameSystems.All(g => string.Equals(g.Version, "local", StringComparison.OrdinalIgnoreCase)));
        }
        finally
        {
            DeleteDirectoryIfExists(dataRootPath);
        }
    }

    [TestMethod]
    public void ListNewScenarios_MapsGameSystemsToNewScenarioSummaries()
    {
        var dataRootPath = BuildTempDataRootPath();

        try
        {
            var workspace = new VttSessionWorkspaceService(dataRootPath);
            var moduleService = new ModuleImportService(workspace);
            var loadService = new LoadGameService(workspace);
            var setupService = new WorkspaceSetupOptionsService(moduleService, loadService);

            var gameSystems = new List<VttMvuModel.Workspace.GameSystemSummary>
            {
                new("CHECKERS-GAMEBOX", "Checkers", "local", "Checkers module")
            };

            var scenarios = setupService.ListNewScenarios(gameSystems);

            Assert.AreEqual(1, scenarios.Count);
            Assert.AreEqual("NEW:CHECKERS-GAMEBOX", scenarios[0].ScenarioId);
            Assert.AreEqual("CHECKERS-GAMEBOX", scenarios[0].GameSystemId);
        }
        finally
        {
            DeleteDirectoryIfExists(dataRootPath);
        }
    }

    [TestMethod]
    public void ListSavedScenarios_MapsKnownSessionsToScenarioSummaries()
    {
        var dataRootPath = BuildTempDataRootPath();

        try
        {
            var workspace = new VttSessionWorkspaceService(dataRootPath);
            var sessionPath = Path.Combine(dataRootPath, "campaign-sessions", $"summary{SnapshotFileExtensions.VttSession}");
            WriteSessionFile(sessionPath, "session-summary", "Summary Session");
            workspace.AddKnownSnapshotPath(sessionPath);

            var moduleService = new ModuleImportService(workspace);
            var loadService = new LoadGameService(workspace);
            var setupService = new WorkspaceSetupOptionsService(moduleService, loadService);

            var scenarios = setupService.ListSavedScenarios();

            Assert.AreEqual(1, scenarios.Count);
            Assert.AreEqual(sessionPath, scenarios[0].ScenarioId);
            Assert.AreEqual("summary.vttsession.json", scenarios[0].Name);
        }
        finally
        {
            DeleteDirectoryIfExists(dataRootPath);
        }
    }

    private static void WriteSessionFile(string path, string sessionId, string title)
    {
        var directoryPath = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        var serializer = new VttSessionSnapshotSerializer();
        var session = new VttSession
        {
            Id = sessionId,
            Title = title
        };

        File.WriteAllText(path, serializer.Save(session));
    }

    private static string BuildTempDataRootPath()
    {
        return Path.Combine(Path.GetTempPath(), $"mimlesvtt-setup-options-tests-{Guid.NewGuid():N}");
    }

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}
