using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.Services;
using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Persistence.Snapshot;
using MIMLESvtt.src.Domain.Persistence.VttSessionNSPC;

namespace MIMLESvtt.Tests;

[TestClass]
public class SaveLoadServicesTests
{
    [TestMethod]
    public void LoadGameService_ListKnownSessions_ReturnsExistingSessionsSortedByFileName()
    {
        var dataRootPath = BuildTempDataRootPath();

        try
        {
            var workspace = new VttSessionWorkspaceService(dataRootPath);
            var service = new LoadGameService(workspace);

            var alphaPath = Path.Combine(dataRootPath, "campaign-sessions", $"alpha{SnapshotFileExtensions.VttSession}");
            var betaPath = Path.Combine(dataRootPath, "campaign-sessions", $"beta{SnapshotFileExtensions.VttSession}");
            var missingPath = Path.Combine(dataRootPath, "campaign-sessions", $"missing{SnapshotFileExtensions.VttSession}");

            WriteSessionFile(alphaPath, "session-alpha", "Alpha Session");
            WriteSessionFile(betaPath, "session-beta", "Beta Session");

            workspace.AddKnownSnapshotPath(betaPath);
            workspace.AddKnownSnapshotPath(alphaPath);
            workspace.AddKnownSnapshotPath(missingPath);

            var sessions = service.ListKnownSessions();

            Assert.AreEqual(2, sessions.Count);
            Assert.IsTrue(string.Equals("alpha.vttsession.json", sessions[0].FileName, StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(string.Equals("beta.vttsession.json", sessions[1].FileName, StringComparison.OrdinalIgnoreCase));
        }
        finally
        {
            DeleteDirectoryIfExists(dataRootPath);
        }
    }

    [TestMethod]
    public void LoadGameService_OpenSession_LoadsWorkspaceSessionFromFile()
    {
        var dataRootPath = BuildTempDataRootPath();

        try
        {
            var workspace = new VttSessionWorkspaceService(dataRootPath);
            var service = new LoadGameService(workspace);
            var sessionPath = Path.Combine(dataRootPath, "campaign-sessions", $"open-test{SnapshotFileExtensions.VttSession}");

            WriteSessionFile(sessionPath, "session-open", "Open Session Test");

            service.OpenSession(sessionPath);

            Assert.IsNotNull(workspace.CurrentVttSession);
            Assert.AreEqual("session-open", workspace.CurrentVttSession!.Id);
            Assert.AreEqual("Open Session Test", workspace.CurrentVttSession.Title);
        }
        finally
        {
            DeleteDirectoryIfExists(dataRootPath);
        }
    }

    [TestMethod]
    public void SaveGameService_CreateNewSession_UsesProvidedGameboxId()
    {
        var dataRootPath = BuildTempDataRootPath();

        try
        {
            var workspace = new VttSessionWorkspaceService(dataRootPath);
            workspace.SetCanCreateSession(true);
            var service = new SaveGameService(workspace);

            service.CreateNewSession("Service Created Campaign", "EMPTY-GAMEBOX");

            Assert.IsNotNull(workspace.CurrentVttSession);
            Assert.AreEqual("EMPTY-GAMEBOX", workspace.CurrentVttSession!.Campaign.GameboxId);
            Assert.AreEqual("Service Created Campaign", workspace.CurrentVttSession.Campaign.Name);
        }
        finally
        {
            DeleteDirectoryIfExists(dataRootPath);
        }
    }

    [TestMethod]
    public void SaveGameService_SaveCurrentSession_PersistsCurrentFilePath()
    {
        var dataRootPath = BuildTempDataRootPath();

        try
        {
            var workspace = new VttSessionWorkspaceService(dataRootPath);
            workspace.SetCanCreateSession(true);
            var service = new SaveGameService(workspace);

            service.CreateNewSession("Persist Session", "EMPTY-GAMEBOX");
            service.SaveCurrentSession();

            Assert.IsFalse(string.IsNullOrWhiteSpace(workspace.State.CurrentFilePath));
            Assert.IsTrue(File.Exists(workspace.State.CurrentFilePath!));
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
        return Path.Combine(Path.GetTempPath(), $"mimlesvtt-services-tests-{Guid.NewGuid():N}");
    }

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}
