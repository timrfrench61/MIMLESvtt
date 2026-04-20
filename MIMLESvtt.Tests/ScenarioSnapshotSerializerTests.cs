using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src;

namespace MIMLESvtt.Tests;

[TestClass]
public class ScenarioSnapshotSerializerTests
{
    [TestMethod]
    public void ScenarioSerializeDeserialize_RoundTrip_PreservesCoreScenarioFields()
    {
        var serializer = new ScenarioSnapshotSerializer();
        var scenario = CreateScenarioFixture();

        var json = serializer.SerializeScenario(scenario);
        var loaded = serializer.DeserializeScenario(json);

        Assert.AreEqual(scenario.Title, loaded.Title);
        Assert.AreEqual(1, loaded.Surfaces.Count);
        Assert.AreEqual("surface-1", loaded.Surfaces[0].Id);
        Assert.AreEqual(1, loaded.Pieces.Count);
        Assert.AreEqual("piece-1", loaded.Pieces[0].Id);
        Assert.AreEqual("surface-1", loaded.Pieces[0].Location.SurfaceId);
        Assert.AreEqual(scenario.Options.EnableFog, loaded.Options.EnableFog);
        Assert.AreEqual(scenario.Options.EnableTurnTracker, loaded.Options.EnableTurnTracker);
    }

    [TestMethod]
    public void DeserializeScenario_WhenScenarioMissing_FailsClearly()
    {
        var serializer = new ScenarioSnapshotSerializer();
        var json = "{\"Version\":1}";

        var exception = Assert.ThrowsException<InvalidOperationException>(() => serializer.DeserializeScenario(json));
        StringAssert.Contains(exception.Message, "Scenario");
    }

    [TestMethod]
    public void DeserializeScenario_WhenVersionMissingOrInvalid_FailsClearly()
    {
        var serializer = new ScenarioSnapshotSerializer();

        var missingVersionJson = "{\"Scenario\":{\"Title\":\"Scenario\"}}";
        var invalidVersionJson = "{\"Version\":999,\"Scenario\":{\"Title\":\"Scenario\"}}";

        var missingVersionException = Assert.ThrowsException<InvalidOperationException>(() => serializer.DeserializeScenario(missingVersionJson));
        StringAssert.Contains(missingVersionException.Message, "Version");

        var invalidVersionException = Assert.ThrowsException<InvalidOperationException>(() => serializer.DeserializeScenario(invalidVersionJson));
        StringAssert.Contains(invalidVersionException.Message, "Version");
    }

    [TestMethod]
    public void ScenarioFormat_IsSeparateFromTableSessionSnapshotFormat()
    {
        var scenarioSerializer = new ScenarioSnapshotSerializer();
        var sessionSerializer = new TableSessionSnapshotSerializer();

        var scenarioJson = scenarioSerializer.SerializeScenario(CreateScenarioFixture());
        var sessionJson = sessionSerializer.Save(CreateTableSessionFixture());

        StringAssert.Contains(scenarioJson, "\"Scenario\"");
        Assert.IsFalse(scenarioJson.Contains("\"TableSession\"", StringComparison.Ordinal));

        StringAssert.Contains(sessionJson, "\"TableSession\"");
        Assert.IsFalse(sessionJson.Contains("\"Scenario\"", StringComparison.Ordinal));
    }

    private static ScenarioExport CreateScenarioFixture()
    {
        return new ScenarioExport
        {
            Title = "Scenario Test",
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
                        Coordinate = new Coordinate
                        {
                            X = 2,
                            Y = 3
                        }
                    }
                }
            ],
            Options = new TableOptions
            {
                EnableFog = true,
                EnableTurnTracker = false
            }
        };
    }

    private static TableSession CreateTableSessionFixture()
    {
        return new TableSession
        {
            Id = "session-1",
            Title = "Session Test",
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
                    DefinitionId = "def-piece-1"
                }
            ],
            Options = new TableOptions
            {
                EnableFog = true,
                EnableTurnTracker = true
            }
        };
    }
}
