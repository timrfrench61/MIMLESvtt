using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src;

namespace MIMLESvtt.Tests;

[TestClass]
public class TableSessionSnapshotSerializerTests
{
    [TestMethod]
    public void SaveLoad_RoundTrip_PreservesTableSessionCoreFields()
    {
        var session = CreateSessionFixture();
        var serializer = new TableSessionSnapshotSerializer();

        var json = serializer.Save(session);
        var loaded = serializer.Load(json);

        Assert.AreEqual(session.Id, loaded.Id);
        Assert.AreEqual(session.Title, loaded.Title);
        Assert.AreEqual(session.Participants.Count, loaded.Participants.Count);
        Assert.AreEqual(session.Participants[0].Id, loaded.Participants[0].Id);
        Assert.AreEqual(session.Participants[0].Name, loaded.Participants[0].Name);
        Assert.AreEqual(session.Participants[0].Role, loaded.Participants[0].Role);
        Assert.AreEqual(session.Surfaces.Count, loaded.Surfaces.Count);
        Assert.AreEqual(session.Options.EnableFog, loaded.Options.EnableFog);
        Assert.AreEqual(session.Visibility.IsHidden, loaded.Visibility.IsHidden);
    }

    [TestMethod]
    public void SaveLoad_RoundTrip_PreservesPiecesAndLocations()
    {
        var session = CreateSessionFixture();
        var serializer = new TableSessionSnapshotSerializer();

        var json = serializer.Save(session);
        var loaded = serializer.Load(json);

        Assert.AreEqual(1, loaded.Pieces.Count);

        var piece = loaded.Pieces[0];
        Assert.AreEqual("piece-1", piece.Id);
        Assert.AreEqual("def-piece-1", piece.DefinitionId);
        Assert.AreEqual("surface-1", piece.Location.SurfaceId);
        Assert.AreEqual(4, piece.Location.Coordinate.X);
        Assert.AreEqual(6, piece.Location.Coordinate.Y);
    }

    [TestMethod]
    public void SaveLoad_RoundTrip_PreservesActionLog()
    {
        var session = CreateSessionFixture();
        var serializer = new TableSessionSnapshotSerializer();

        var json = serializer.Save(session);
        var loaded = serializer.Load(json);

        Assert.AreEqual(1, loaded.ActionLog.Count);
        Assert.AreEqual("action-1", loaded.ActionLog[0].Id);
        Assert.AreEqual("MovePiece", loaded.ActionLog[0].ActionType);
        Assert.AreEqual("participant-1", loaded.ActionLog[0].ActorParticipantId);
    }

    [TestMethod]
    public void SaveLoad_RoundTrip_PreservesTurnState()
    {
        var session = CreateSessionFixture();
        var serializer = new TableSessionSnapshotSerializer();

        var json = serializer.Save(session);
        var loaded = serializer.Load(json);

        CollectionAssert.AreEqual(new[] { "participant-1", "participant-2" }, loaded.TurnOrder);
        Assert.AreEqual(1, loaded.CurrentTurnIndex);
        Assert.AreEqual("Action", loaded.CurrentPhase);
    }

    [TestMethod]
    public void SaveLoad_RoundTrip_PreservesModuleState()
    {
        var session = CreateSessionFixture();
        var serializer = new TableSessionSnapshotSerializer();

        var json = serializer.Save(session);
        var loaded = serializer.Load(json);

        Assert.AreEqual(2, loaded.ModuleState.Count);
        Assert.IsTrue(loaded.ModuleState.TryGetValue("Turn", out var turnValue));
        var turnElement = (JsonElement)turnValue!;
        Assert.AreEqual(JsonValueKind.Number, turnElement.ValueKind);
        Assert.AreEqual(3, turnElement.GetInt32());

        Assert.IsTrue(loaded.ModuleState.TryGetValue("Phase", out var phaseValue));
        var phaseElement = (JsonElement)phaseValue!;
        Assert.AreEqual(JsonValueKind.String, phaseElement.ValueKind);
        Assert.AreEqual("Combat", phaseElement.GetString());
    }

    [TestMethod]
    public void SaveLoad_RoundTrip_PreservesActionEnrichedTableSessionState()
    {
        var session = CreateActionEnrichedSessionFixture();
        var serializer = new TableSessionSnapshotSerializer();

        var json = serializer.Save(session);
        var loaded = serializer.Load(json);

        Assert.AreEqual(1, loaded.Pieces.Count);
        var piece = loaded.Pieces[0];
        Assert.AreEqual("surface-2", piece.Location.SurfaceId);
        Assert.AreEqual(8, piece.Location.Coordinate.X);
        Assert.AreEqual(5, piece.Location.Coordinate.Y);

        Assert.AreEqual(180f, piece.Rotation.Degrees);
        CollectionAssert.AreEqual(new List<string> { "marker-hidden", "marker-stunned" }, piece.MarkerIds);

        Assert.IsTrue(piece.State.TryGetValue("HitPoints", out var hitPointsState));
        var hitPointsElement = (JsonElement)hitPointsState!;
        Assert.AreEqual(JsonValueKind.Number, hitPointsElement.ValueKind);
        Assert.AreEqual(7, hitPointsElement.GetInt32());

        Assert.IsTrue(piece.State.TryGetValue("Condition", out var conditionState));
        var conditionElement = (JsonElement)conditionState!;
        Assert.AreEqual(JsonValueKind.String, conditionElement.ValueKind);
        Assert.AreEqual("Pinned", conditionElement.GetString());

        Assert.AreEqual(5, loaded.ActionLog.Count);
        CollectionAssert.AreEqual(
            new List<string> { "MovePiece", "RotatePiece", "AddMarker", "RemoveMarker", "ChangePieceState" },
            loaded.ActionLog.Select(a => a.ActionType).ToList());

        foreach (var action in loaded.ActionLog)
        {
            Assert.IsInstanceOfType<JsonElement>(action.Payload);
        }

        Assert.AreEqual(3, loaded.ModuleState.Count);
        var moduleTurnElement = (JsonElement)loaded.ModuleState["Turn"];
        Assert.AreEqual(12, moduleTurnElement.GetInt32());

        var modulePhaseElement = (JsonElement)loaded.ModuleState["Phase"];
        Assert.AreEqual("Resolution", modulePhaseElement.GetString());

        var moduleStrictModeElement = (JsonElement)loaded.ModuleState["StrictMode"];
        Assert.AreEqual(JsonValueKind.True, moduleStrictModeElement.ValueKind);
    }

    [TestMethod]
    public void Load_WhenVersionMissingOrInvalid_FailsClearly()
    {
        var serializer = new TableSessionSnapshotSerializer();

        var missingVersionJson = "{\"TableSession\":{\"Id\":\"session-1\",\"Title\":\"Session\"}}";
        var invalidVersionJson = "{\"Version\":999,\"TableSession\":{\"Id\":\"session-1\",\"Title\":\"Session\"}}";

        var missingVersionException = Assert.ThrowsException<InvalidOperationException>(() => serializer.Load(missingVersionJson));
        StringAssert.Contains(missingVersionException.Message, "Version");

        var invalidVersionException = Assert.ThrowsException<InvalidOperationException>(() => serializer.Load(invalidVersionJson));
        StringAssert.Contains(invalidVersionException.Message, "Version");
    }

    [TestMethod]
    public void DeserializeSnapshot_WhenTableSessionMissing_FailsClearly()
    {
        var serializer = new TableSessionSnapshotSerializer();
        var json = "{\"Version\":1}";

        var exception = Assert.ThrowsException<InvalidOperationException>(() => serializer.DeserializeSnapshot(json));
        StringAssert.Contains(exception.Message, "TableSession");
    }

    [TestMethod]
    public void DeserializeSnapshot_WhenVersionZero_FailsClearly()
    {
        var serializer = new TableSessionSnapshotSerializer();
        var json = "{\"Version\":0,\"TableSession\":{\"Id\":\"session-1\",\"Title\":\"Session\"}}";

        var exception = Assert.ThrowsException<InvalidOperationException>(() => serializer.DeserializeSnapshot(json));
        StringAssert.Contains(exception.Message, "Version");
    }

    [TestMethod]
    public void DeserializeSnapshot_WhenVersionUnsupported_FailsClearly()
    {
        var serializer = new TableSessionSnapshotSerializer();
        var json = "{\"Version\":999,\"TableSession\":{\"Id\":\"session-1\",\"Title\":\"Session\"}}";

        var exception = Assert.ThrowsException<InvalidOperationException>(() => serializer.DeserializeSnapshot(json));
        StringAssert.Contains(exception.Message, "Version");
    }

    private static TableSession CreateSessionFixture()
    {
        return new TableSession
        {
            Id = "session-1",
            Title = "Snapshot Test Session",
            Participants =
            [
                new Participant
                {
                    Id = "participant-1",
                    Name = "GM",
                    Role = ParticipantRole.GM
                },
                new Participant
                {
                    Id = "participant-2",
                    Name = "Player",
                    Role = ParticipantRole.Player
                }
            ],
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
                            X = 4,
                            Y = 6
                        }
                    }
                }
            ],
            Options = new TableOptions
            {
                EnableFog = true,
                EnableTurnTracker = true
            },
            Visibility = new VisibilityState
            {
                IsHidden = false,
                VisibleToParticipantIds = ["participant-1"]
            },
            ActionLog =
            [
                new ActionRecord
                {
                    Id = "action-1",
                    ActionType = "MovePiece",
                    ActorParticipantId = "participant-1",
                    TimestampUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    Payload = new MovePiecePayload
                    {
                        PieceId = "piece-1",
                        NewLocation = new Location
                        {
                            SurfaceId = "surface-1",
                            Coordinate = new Coordinate
                            {
                                X = 4,
                                Y = 6
                            }
                        }
                    }
                }
            ],
            ModuleState =
            {
                ["Turn"] = 3,
                ["Phase"] = "Combat"
            },
            TurnOrder = ["participant-1", "participant-2"],
            CurrentTurnIndex = 1,
            CurrentPhase = "Action"
        };
    }

    private static TableSession CreateActionEnrichedSessionFixture()
    {
        return new TableSession
        {
            Id = "session-enriched-1",
            Title = "Action-Enriched Snapshot Session",
            Participants =
            [
                new Participant
                {
                    Id = "participant-1",
                    Name = "GM",
                    Role = ParticipantRole.GM
                }
            ],
            Surfaces =
            [
                new SurfaceInstance
                {
                    Id = "surface-1",
                    DefinitionId = "def-surface-1"
                },
                new SurfaceInstance
                {
                    Id = "surface-2",
                    DefinitionId = "def-surface-2"
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
                        SurfaceId = "surface-2",
                        Coordinate = new Coordinate
                        {
                            X = 8,
                            Y = 5
                        }
                    },
                    Rotation = new Rotation
                    {
                        Degrees = 180f
                    },
                    MarkerIds = ["marker-hidden", "marker-stunned"],
                    State =
                    {
                        ["HitPoints"] = 7,
                        ["Condition"] = "Pinned"
                    }
                }
            ],
            Options = new TableOptions
            {
                EnableFog = true,
                EnableTurnTracker = true
            },
            Visibility = new VisibilityState
            {
                IsHidden = false,
                VisibleToParticipantIds = ["participant-1"]
            },
            ActionLog =
            [
                new ActionRecord
                {
                    Id = "action-1",
                    ActionType = "MovePiece",
                    ActorParticipantId = "participant-1",
                    TimestampUtc = new DateTime(2026, 2, 1, 10, 0, 0, DateTimeKind.Utc),
                    Payload = new MovePiecePayload
                    {
                        PieceId = "piece-1",
                        NewLocation = new Location
                        {
                            SurfaceId = "surface-2",
                            Coordinate = new Coordinate
                            {
                                X = 8,
                                Y = 5
                            }
                        }
                    }
                },
                new ActionRecord
                {
                    Id = "action-2",
                    ActionType = "RotatePiece",
                    ActorParticipantId = "participant-1",
                    TimestampUtc = new DateTime(2026, 2, 1, 10, 1, 0, DateTimeKind.Utc),
                    Payload = new RotatePiecePayload
                    {
                        PieceId = "piece-1",
                        NewRotation = new Rotation
                        {
                            Degrees = 180f
                        }
                    }
                },
                new ActionRecord
                {
                    Id = "action-3",
                    ActionType = "AddMarker",
                    ActorParticipantId = "participant-1",
                    TimestampUtc = new DateTime(2026, 2, 1, 10, 2, 0, DateTimeKind.Utc),
                    Payload = new AddMarkerPayload
                    {
                        PieceId = "piece-1",
                        MarkerId = "marker-hidden"
                    }
                },
                new ActionRecord
                {
                    Id = "action-4",
                    ActionType = "RemoveMarker",
                    ActorParticipantId = "participant-1",
                    TimestampUtc = new DateTime(2026, 2, 1, 10, 3, 0, DateTimeKind.Utc),
                    Payload = new RemoveMarkerPayload
                    {
                        PieceId = "piece-1",
                        MarkerId = "marker-exhausted"
                    }
                },
                new ActionRecord
                {
                    Id = "action-5",
                    ActionType = "ChangePieceState",
                    ActorParticipantId = "participant-1",
                    TimestampUtc = new DateTime(2026, 2, 1, 10, 4, 0, DateTimeKind.Utc),
                    Payload = new ChangePieceStatePayload
                    {
                        PieceId = "piece-1",
                        Key = "HitPoints",
                        Value = 7
                    }
                }
            ],
            ModuleState =
            {
                ["Turn"] = 12,
                ["Phase"] = "Resolution",
                ["StrictMode"] = true
            }
        };
    }
}
