using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src;

namespace MIMLESvtt.Tests;

[TestClass]
public class FilePersistenceServiceTests
{
    [TestMethod]
    public void SnapshotFileWorkflow_SaveAndLoadTableSession_WithCorrectExtension_Succeeds()
    {
        var service = new SnapshotFileWorkflowService();
        var session = CreateTableSessionFixture();
        var path = CreateTempFilePath("workflow-session", SnapshotFileExtensions.TableSession);

        try
        {
            service.SaveTableSession(session, path);
            var loaded = service.LoadTableSession(path);

            Assert.AreEqual(session.Id, loaded.Id);
            Assert.AreEqual(session.Title, loaded.Title);
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void SnapshotFileWorkflow_SaveAndLoadScenario_WithCorrectExtension_Succeeds()
    {
        var service = new SnapshotFileWorkflowService();
        var scenario = CreateScenarioFixture();
        var path = CreateTempFilePath("workflow-scenario", SnapshotFileExtensions.Scenario);

        try
        {
            service.SaveScenario(scenario, path);
            var loaded = service.LoadScenario(path);

            Assert.AreEqual(scenario.Title, loaded.Title);
            Assert.AreEqual(1, loaded.Pieces.Count);
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void SnapshotFileWorkflow_SaveAndLoadContentPack_WithCorrectExtension_Succeeds()
    {
        var service = new SnapshotFileWorkflowService();
        var contentPack = CreateContentPackFixture();
        var path = CreateTempFilePath("workflow-contentpack", SnapshotFileExtensions.ContentPack);

        try
        {
            service.SaveContentPack(contentPack, path);
            var loaded = service.LoadContentPack(path);

            Assert.AreEqual(contentPack.Version, loaded.Version);
            Assert.AreEqual("Core Pack", loaded.Manifest!.Name);
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void SnapshotFileWorkflow_SaveAndLoadActionLog_WithCorrectExtension_Succeeds()
    {
        var service = new SnapshotFileWorkflowService();
        var actionLog = CreateActionLogFixture();
        var path = CreateTempFilePath("workflow-actionlog", SnapshotFileExtensions.ActionLog);

        try
        {
            service.SaveActionLog(actionLog, path);
            var loaded = service.LoadActionLog(path);

            Assert.AreEqual(actionLog.Version, loaded.Version);
            Assert.AreEqual("session-1", loaded.SessionId);
            Assert.AreEqual(2, loaded.Actions!.Count);
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void SnapshotFileWorkflow_SaveTableSession_WithWrongExtension_FailsClearly()
    {
        var service = new SnapshotFileWorkflowService();
        var session = CreateTableSessionFixture();
        var path = CreateTempFilePath("workflow-wrong-session", ".json");

        Assert.ThrowsException<ArgumentException>(() => service.SaveTableSession(session, path));
    }

    [TestMethod]
    public void SnapshotFileWorkflow_SaveScenario_WithWrongExtension_FailsClearly()
    {
        var service = new SnapshotFileWorkflowService();
        var scenario = CreateScenarioFixture();
        var path = CreateTempFilePath("workflow-wrong-scenario", ".json");

        Assert.ThrowsException<ArgumentException>(() => service.SaveScenario(scenario, path));
    }

    [TestMethod]
    public void SnapshotFileWorkflow_SaveContentPack_WithWrongExtension_FailsClearly()
    {
        var service = new SnapshotFileWorkflowService();
        var contentPack = CreateContentPackFixture();
        var path = CreateTempFilePath("workflow-wrong-contentpack", ".json");

        Assert.ThrowsException<ArgumentException>(() => service.SaveContentPack(contentPack, path));
    }

    [TestMethod]
    public void SnapshotFileWorkflow_SaveActionLog_WithWrongExtension_FailsClearly()
    {
        var service = new SnapshotFileWorkflowService();
        var actionLog = CreateActionLogFixture();
        var path = CreateTempFilePath("workflow-wrong-actionlog", ".json");

        Assert.ThrowsException<ArgumentException>(() => service.SaveActionLog(actionLog, path));
    }

    [TestMethod]
    public void SnapshotFileWorkflow_LoadTableSession_WithWrongExtension_FailsClearly()
    {
        var service = new SnapshotFileWorkflowService();
        var path = CreateTempFilePath("workflow-load-wrong-session", ".json");

        Assert.ThrowsException<ArgumentException>(() => service.LoadTableSession(path));
    }

    [TestMethod]
    public void SnapshotFileWorkflow_LoadScenario_WithWrongExtension_FailsClearly()
    {
        var service = new SnapshotFileWorkflowService();
        var path = CreateTempFilePath("workflow-load-wrong-scenario", ".json");

        Assert.ThrowsException<ArgumentException>(() => service.LoadScenario(path));
    }

    [TestMethod]
    public void SnapshotFileWorkflow_LoadContentPack_WithWrongExtension_FailsClearly()
    {
        var service = new SnapshotFileWorkflowService();
        var path = CreateTempFilePath("workflow-load-wrong-contentpack", ".json");

        Assert.ThrowsException<ArgumentException>(() => service.LoadContentPack(path));
    }

    [TestMethod]
    public void SnapshotFileWorkflow_LoadActionLog_WithWrongExtension_FailsClearly()
    {
        var service = new SnapshotFileWorkflowService();
        var path = CreateTempFilePath("workflow-load-wrong-actionlog", ".json");

        Assert.ThrowsException<ArgumentException>(() => service.LoadActionLog(path));
    }

    [TestMethod]
    public void TableSessionFilePersistence_SaveThenLoad_RoundTripsSessionSnapshot()
    {
        var service = new TableSessionFilePersistenceService();
        var session = CreateTableSessionFixture();
        var path = CreateTempFilePath("session", ".session.json");

        try
        {
            service.SaveToFile(session, path);
            var loaded = service.LoadFromFile(path);

            Assert.AreEqual(session.Id, loaded.Id);
            Assert.AreEqual(session.Title, loaded.Title);
            Assert.AreEqual(1, loaded.Surfaces.Count);
            Assert.AreEqual(1, loaded.Pieces.Count);
            Assert.AreEqual("piece-1", loaded.Pieces[0].Id);
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void ScenarioFilePersistence_SaveThenLoad_RoundTripsScenarioSnapshot()
    {
        var service = new ScenarioFilePersistenceService();
        var scenario = CreateScenarioFixture();
        var path = CreateTempFilePath("scenario", ".scenario.json");

        try
        {
            service.SaveToFile(scenario, path);
            var loaded = service.LoadFromFile(path);

            Assert.AreEqual(scenario.Title, loaded.Title);
            Assert.AreEqual(1, loaded.Surfaces.Count);
            Assert.AreEqual(1, loaded.Pieces.Count);
            Assert.AreEqual("piece-1", loaded.Pieces[0].Id);
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void TableSessionFilePersistence_LoadMissingFile_FailsClearly()
    {
        var service = new TableSessionFilePersistenceService();
        var path = CreateTempFilePath("missing-session", ".json");

        var exception = Assert.ThrowsException<FileNotFoundException>(() => service.LoadFromFile(path));
        StringAssert.Contains(exception.Message, "not found");
    }

    [TestMethod]
    public void ScenarioFilePersistence_LoadMissingFile_FailsClearly()
    {
        var service = new ScenarioFilePersistenceService();
        var path = CreateTempFilePath("missing-scenario", ".json");

        var exception = Assert.ThrowsException<FileNotFoundException>(() => service.LoadFromFile(path));
        StringAssert.Contains(exception.Message, "not found");
    }

    [TestMethod]
    public void TableSessionFilePersistence_LoadMalformedJson_FailsClearly()
    {
        var service = new TableSessionFilePersistenceService();
        var path = CreateTempFilePath("malformed-session", ".json");

        try
        {
            File.WriteAllText(path, "{\"Version\":1,\"TableSession\":", Encoding.UTF8);

            var exception = Assert.ThrowsException<InvalidOperationException>(() => service.LoadFromFile(path));
            StringAssert.Contains(exception.Message, "malformed JSON");
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void ScenarioFilePersistence_LoadMalformedJson_FailsClearly()
    {
        var service = new ScenarioFilePersistenceService();
        var path = CreateTempFilePath("malformed-scenario", ".json");

        try
        {
            File.WriteAllText(path, "{\"Version\":1,\"Scenario\":", Encoding.UTF8);

            var exception = Assert.ThrowsException<InvalidOperationException>(() => service.LoadFromFile(path));
            StringAssert.Contains(exception.Message, "malformed JSON");
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("   ")]
    public void TableSessionFilePersistence_SaveWithInvalidPath_FailsClearly(string path)
    {
        var service = new TableSessionFilePersistenceService();
        var session = CreateTableSessionFixture();

        Assert.ThrowsException<ArgumentException>(() => service.SaveToFile(session, path));
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("   ")]
    public void ScenarioFilePersistence_SaveWithInvalidPath_FailsClearly(string path)
    {
        var service = new ScenarioFilePersistenceService();
        var scenario = CreateScenarioFixture();

        Assert.ThrowsException<ArgumentException>(() => service.SaveToFile(scenario, path));
    }

    [TestMethod]
    public void ContentPackFilePersistence_SaveThenLoad_RoundTripsContentPackSnapshot()
    {
        var service = new ContentPackFilePersistenceService();
        var contentPack = CreateContentPackFixture();
        var path = CreateTempFilePath("contentpack", SnapshotFileExtensions.ContentPack);

        try
        {
            service.SaveToFile(contentPack, path);
            var loaded = service.LoadFromFile(path);

            Assert.AreEqual(contentPack.Version, loaded.Version);
            Assert.IsNotNull(loaded.Manifest);
            Assert.AreEqual("Core Pack", loaded.Manifest!.Name);
            Assert.AreEqual(1, loaded.Definitions!.Count);
            Assert.AreEqual("def-1", loaded.Definitions[0].Id);
            Assert.AreEqual(1, loaded.Assets!.Count);
            Assert.AreEqual("asset-1", loaded.Assets[0].Id);
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void ActionLogFilePersistence_SaveThenLoad_RoundTripsActionLogSnapshot()
    {
        var service = new ActionLogFilePersistenceService();
        var actionLog = CreateActionLogFixture();
        var path = CreateTempFilePath("actionlog", SnapshotFileExtensions.ActionLog);

        try
        {
            service.SaveToFile(actionLog, path);
            var loaded = service.LoadFromFile(path);

            Assert.AreEqual(actionLog.Version, loaded.Version);
            Assert.AreEqual("session-1", loaded.SessionId);
            Assert.AreEqual(2, loaded.Actions!.Count);
            Assert.AreEqual("action-1", loaded.Actions[0].Id);
            Assert.AreEqual("MovePiece", loaded.Actions[0].ActionType);
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void ContentPackFilePersistence_LoadMissingFile_FailsClearly()
    {
        var service = new ContentPackFilePersistenceService();
        var path = CreateTempFilePath("missing-contentpack", ".json");

        var exception = Assert.ThrowsException<FileNotFoundException>(() => service.LoadFromFile(path));
        StringAssert.Contains(exception.Message, "not found");
    }

    [TestMethod]
    public void ActionLogFilePersistence_LoadMissingFile_FailsClearly()
    {
        var service = new ActionLogFilePersistenceService();
        var path = CreateTempFilePath("missing-actionlog", ".json");

        var exception = Assert.ThrowsException<FileNotFoundException>(() => service.LoadFromFile(path));
        StringAssert.Contains(exception.Message, "not found");
    }

    [TestMethod]
    public void ContentPackFilePersistence_LoadMalformedJson_FailsClearly()
    {
        var service = new ContentPackFilePersistenceService();
        var path = CreateTempFilePath("malformed-contentpack", ".json");

        try
        {
            File.WriteAllText(path, "{\"Version\":1,\"Manifest\":", Encoding.UTF8);

            var exception = Assert.ThrowsException<InvalidOperationException>(() => service.LoadFromFile(path));
            StringAssert.Contains(exception.Message, "malformed JSON");
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void ActionLogFilePersistence_LoadMalformedJson_FailsClearly()
    {
        var service = new ActionLogFilePersistenceService();
        var path = CreateTempFilePath("malformed-actionlog", ".json");

        try
        {
            File.WriteAllText(path, "{\"Version\":1,\"SessionId\":", Encoding.UTF8);

            var exception = Assert.ThrowsException<InvalidOperationException>(() => service.LoadFromFile(path));
            StringAssert.Contains(exception.Message, "malformed JSON");
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("   ")]
    public void ContentPackFilePersistence_SaveWithInvalidPath_FailsClearly(string path)
    {
        var service = new ContentPackFilePersistenceService();
        var contentPack = CreateContentPackFixture();

        Assert.ThrowsException<ArgumentException>(() => service.SaveToFile(contentPack, path));
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("   ")]
    public void ActionLogFilePersistence_SaveWithInvalidPath_FailsClearly(string path)
    {
        var service = new ActionLogFilePersistenceService();
        var actionLog = CreateActionLogFixture();

        Assert.ThrowsException<ArgumentException>(() => service.SaveToFile(actionLog, path));
    }

    [TestMethod]
    public void SnapshotFileExtensions_AreDefinedForAllSupportedSnapshotTypes()
    {
        Assert.AreEqual(".session.json", SnapshotFileExtensions.TableSession);
        Assert.AreEqual(".scenario.json", SnapshotFileExtensions.Scenario);
        Assert.AreEqual(".contentpack.json", SnapshotFileExtensions.ContentPack);
        Assert.AreEqual(".actionlog.json", SnapshotFileExtensions.ActionLog);
    }

    private static TableSession CreateTableSessionFixture()
    {
        return new TableSession
        {
            Id = "session-1",
            Title = "Session File",
            Surfaces =
            [
                new SurfaceInstance
                {
                    Id = "surface-1",
                    DefinitionId = "def-surface-1"
                }
            ],
            Pieces =
            [
                new PieceInstance
                {
                    Id = "piece-1",
                    DefinitionId = "def-piece-1",
                    Location = new Location
                    {
                        SurfaceId = "surface-1",
                        Coordinate = new Coordinate { X = 2, Y = 3 }
                    }
                }
            ],
            Options = new TableOptions
            {
                EnableFog = true,
                EnableTurnTracker = true
            }
        };
    }

    private static ContentPackSnapshot CreateContentPackFixture()
    {
        return new ContentPackSnapshot
        {
            Version = ContentPackSnapshotSerializer.CurrentVersion,
            Manifest = new ContentPackManifest
            {
                Name = "Core Pack",
                Description = "Base content"
            },
            Definitions =
            [
                new ContentPackDefinition
                {
                    Id = "def-1",
                    Type = "Piece"
                }
            ],
            Assets =
            [
                new ContentPackAsset
                {
                    Id = "asset-1",
                    AssetPath = "images/token.png"
                }
            ]
        };
    }

    private static ActionLogSnapshot CreateActionLogFixture()
    {
        return new ActionLogSnapshot
        {
            Version = ActionLogSnapshotSerializer.CurrentVersion,
            SessionId = "session-1",
            Actions =
            [
                new ActionRecord
                {
                    Id = "action-1",
                    ActionType = "MovePiece",
                    ActorParticipantId = "participant-1",
                    TimestampUtc = new DateTime(2026, 3, 1, 12, 0, 0, DateTimeKind.Utc),
                    Payload = null
                },
                new ActionRecord
                {
                    Id = "action-2",
                    ActionType = "AddMarker",
                    ActorParticipantId = "participant-1",
                    TimestampUtc = new DateTime(2026, 3, 1, 12, 1, 0, DateTimeKind.Utc),
                    Payload = new AddMarkerPayload
                    {
                        PieceId = "piece-1",
                        MarkerId = "marker-hidden"
                    }
                }
            ]
        };
    }

    private static ScenarioExport CreateScenarioFixture()
    {
        return new ScenarioExport
        {
            Title = "Scenario File",
            Surfaces =
            [
                new SurfaceInstance
                {
                    Id = "surface-1",
                    DefinitionId = "def-surface-1"
                }
            ],
            Pieces =
            [
                new PieceInstance
                {
                    Id = "piece-1",
                    DefinitionId = "def-piece-1",
                    Location = new Location
                    {
                        SurfaceId = "surface-1",
                        Coordinate = new Coordinate { X = 4, Y = 1 }
                    }
                }
            ],
            Options = new TableOptions
            {
                EnableFog = false,
                EnableTurnTracker = true
            }
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
}
