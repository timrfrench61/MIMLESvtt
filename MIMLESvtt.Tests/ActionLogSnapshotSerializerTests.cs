using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src;

namespace MIMLESvtt.Tests;

[TestClass]
public class ActionLogSnapshotSerializerTests
{
    [TestMethod]
    public void ActionLogSerializeDeserialize_RoundTrip_PreservesSessionIdAndActions()
    {
        var serializer = new ActionLogSnapshotSerializer();
        var actionLog = CreateActionLogFixture();

        var json = serializer.SerializeActionLog(actionLog);
        var loaded = serializer.DeserializeActionLog(json);

        Assert.AreEqual("session-1", loaded.SessionId);
        Assert.AreEqual(2, loaded.Actions!.Count);

        Assert.AreEqual("action-1", loaded.Actions[0].Id);
        Assert.AreEqual("MovePiece", loaded.Actions[0].ActionType);
        Assert.AreEqual("participant-1", loaded.Actions[0].ActorParticipantId);

        Assert.AreEqual("action-2", loaded.Actions[1].Id);
        Assert.AreEqual("AddMarker", loaded.Actions[1].ActionType);
        Assert.AreEqual("participant-1", loaded.Actions[1].ActorParticipantId);
    }

    [TestMethod]
    public void DeserializeActionLog_WhenSessionIdMissing_FailsClearly()
    {
        var serializer = new ActionLogSnapshotSerializer();
        var json = "{\"Version\":1,\"Actions\":[]}";

        var exception = Assert.ThrowsException<InvalidOperationException>(() => serializer.DeserializeActionLog(json));
        StringAssert.Contains(exception.Message, "SessionId");
    }

    [TestMethod]
    public void DeserializeActionLog_WhenActionsMissing_FailsClearly()
    {
        var serializer = new ActionLogSnapshotSerializer();
        var json = "{\"Version\":1,\"SessionId\":\"session-1\"}";

        var exception = Assert.ThrowsException<InvalidOperationException>(() => serializer.DeserializeActionLog(json));
        StringAssert.Contains(exception.Message, "Actions");
    }

    [TestMethod]
    public void DeserializeActionLog_WhenVersionMissingOrInvalid_FailsClearly()
    {
        var serializer = new ActionLogSnapshotSerializer();

        var missingVersionJson = "{\"SessionId\":\"session-1\",\"Actions\":[]}";
        var invalidVersionJson = "{\"Version\":999,\"SessionId\":\"session-1\",\"Actions\":[]}";

        var missingVersionException = Assert.ThrowsException<InvalidOperationException>(() => serializer.DeserializeActionLog(missingVersionJson));
        StringAssert.Contains(missingVersionException.Message, "Version");

        var invalidVersionException = Assert.ThrowsException<InvalidOperationException>(() => serializer.DeserializeActionLog(invalidVersionJson));
        StringAssert.Contains(invalidVersionException.Message, "Version");
    }

    [TestMethod]
    public void ActionLogFormat_IsSeparateFromSessionScenarioAndContentPackFormats()
    {
        var actionLogSerializer = new ActionLogSnapshotSerializer();
        var sessionSerializer = new TableSessionSnapshotSerializer();
        var scenarioSerializer = new ScenarioSnapshotSerializer();
        var contentPackSerializer = new ContentPackSnapshotSerializer();

        var actionLogJson = actionLogSerializer.SerializeActionLog(CreateActionLogFixture());
        var sessionJson = sessionSerializer.Save(CreateTableSessionFixture());
        var scenarioJson = scenarioSerializer.SerializeScenario(CreateScenarioFixture());
        var contentPackJson = contentPackSerializer.SerializeContentPack(CreateContentPackFixture());

        StringAssert.Contains(actionLogJson, "\"SessionId\"");
        StringAssert.Contains(actionLogJson, "\"Actions\"");
        Assert.IsFalse(actionLogJson.Contains("\"TableSession\"", StringComparison.Ordinal));
        Assert.IsFalse(actionLogJson.Contains("\"Scenario\"", StringComparison.Ordinal));
        Assert.IsFalse(actionLogJson.Contains("\"Manifest\"", StringComparison.Ordinal));

        StringAssert.Contains(sessionJson, "\"TableSession\"");
        Assert.IsFalse(sessionJson.Contains("\"SessionId\"", StringComparison.Ordinal));

        StringAssert.Contains(scenarioJson, "\"Scenario\"");
        Assert.IsFalse(scenarioJson.Contains("\"SessionId\"", StringComparison.Ordinal));

        StringAssert.Contains(contentPackJson, "\"Manifest\"");
        Assert.IsFalse(contentPackJson.Contains("\"SessionId\"", StringComparison.Ordinal));
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
                    TimestampUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    Payload = null
                },
                new ActionRecord
                {
                    Id = "action-2",
                    ActionType = "AddMarker",
                    ActorParticipantId = "participant-1",
                    TimestampUtc = new DateTime(2026, 1, 1, 0, 1, 0, DateTimeKind.Utc),
                    Payload = new AddMarkerPayload
                    {
                        PieceId = "piece-1",
                        MarkerId = "marker-hidden"
                    }
                }
            ]
        };
    }

    private static TableSession CreateTableSessionFixture()
    {
        return new TableSession
        {
            Id = "session-1",
            Title = "Session Test"
        };
    }

    private static ScenarioExport CreateScenarioFixture()
    {
        return new ScenarioExport
        {
            Title = "Scenario Test"
        };
    }

    private static ContentPackSnapshot CreateContentPackFixture()
    {
        return new ContentPackSnapshot
        {
            Version = ContentPackSnapshotSerializer.CurrentVersion,
            Manifest = new ContentPackManifest
            {
                Name = "Pack",
                Description = "Desc"
            },
            Definitions = [],
            Assets = []
        };
    }
}
