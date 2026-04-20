using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src;

namespace MIMLESvtt.Tests;

[TestClass]
public class FilePersistenceServiceTests
{
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
