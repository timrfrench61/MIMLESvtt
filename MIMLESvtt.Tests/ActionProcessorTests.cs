using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src;

namespace MIMLESvtt.Tests;

[TestClass]
public class ActionProcessorTests
{
    [TestMethod]
    public void Process_AppendsExactlyOneActionRecordToActionLog()
    {
        var session = new TableSession();
        var request = new ActionRequest
        {
            ActionType = "NoOpAction",
            ActorParticipantId = "participant-1",
            Payload = new { PieceId = "piece-1" }
        };

        var processor = new ActionProcessor();

        processor.Process(session, request);

        Assert.AreEqual(1, session.ActionLog.Count);
    }

    [TestMethod]
    public void Process_CopiesActionRequestFieldsIntoActionRecord()
    {
        var payload = new { PieceId = "piece-2", SurfaceId = "surface-1" };

        var session = new TableSession();
        var request = new ActionRequest
        {
            ActionType = "NoOpAction",
            ActorParticipantId = "participant-2",
            Payload = payload
        };

        var processor = new ActionProcessor();

        var actionRecord = processor.Process(session, request);

        Assert.AreEqual(request.ActionType, actionRecord.ActionType);
        Assert.AreEqual(request.ActorParticipantId, actionRecord.ActorParticipantId);
        Assert.AreSame(payload, actionRecord.Payload);
        Assert.AreEqual(actionRecord, session.ActionLog[0]);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("   ")]
    public void Process_WithEmptyOrWhitespaceActionType_ThrowsArgumentException(string actionType)
    {
        var session = new TableSession();
        var request = new ActionRequest
        {
            ActionType = actionType,
            ActorParticipantId = "participant-1",
            Payload = new { Any = "value" }
        };

        var processor = new ActionProcessor();

        Assert.ThrowsException<ArgumentException>(() => processor.Process(session, request));
        Assert.AreEqual(0, session.ActionLog.Count);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("   ")]
    public void Process_WithEmptyOrWhitespaceActorParticipantId_ThrowsArgumentException(string actorParticipantId)
    {
        var session = new TableSession();
        var request = new ActionRequest
        {
            ActionType = "MovePiece",
            ActorParticipantId = actorParticipantId,
            Payload = new { Any = "value" }
        };

        var processor = new ActionProcessor();

        Assert.ThrowsException<ArgumentException>(() => processor.Process(session, request));
        Assert.AreEqual(0, session.ActionLog.Count);
    }

    [TestMethod]
    public void Process_WhenValidationFails_DoesNotAppendToActionLog()
    {
        var session = new TableSession();
        var request = new ActionRequest
        {
            ActionType = " ",
            ActorParticipantId = "participant-1",
            Payload = new { Any = "value" }
        };

        var processor = new ActionProcessor();

        Assert.ThrowsException<ArgumentException>(() => processor.Process(session, request));
        Assert.AreEqual(0, session.ActionLog.Count);
    }

    [TestMethod]
    public void Process_WithValidRequest_PreservesSessionStateOutsideActionLog()
    {
        var session = new TableSession
        {
            Id = "session-1",
            Title = "Test Session"
        };

        session.Pieces.Add(new PieceInstance { Id = "piece-1", DefinitionId = "def-1" });
        session.Surfaces.Add(new SurfaceInstance { Id = "surface-1", DefinitionId = "surface-def-1" });

        var initialPieceCount = session.Pieces.Count;
        var initialSurfaceCount = session.Surfaces.Count;

        var request = new ActionRequest
        {
            ActionType = "NoOpAction",
            ActorParticipantId = "participant-1",
            Payload = new { PieceId = "piece-1" }
        };

        var processor = new ActionProcessor();

        processor.Process(session, request);

        Assert.AreEqual(1, session.ActionLog.Count);
        Assert.AreEqual(initialPieceCount, session.Pieces.Count);
        Assert.AreEqual(initialSurfaceCount, session.Surfaces.Count);
    }

    [TestMethod]
    public void Process_WithMovePiece_UpdatesTargetPieceLocation()
    {
        var originalLocation = new Location
        {
            SurfaceId = "surface-1",
            Coordinate = new Coordinate { X = 1, Y = 1 }
        };

        var newLocation = new Location
        {
            SurfaceId = "surface-1",
            Coordinate = new Coordinate { X = 4, Y = 6 }
        };

        var targetPiece = new PieceInstance
        {
            Id = "piece-1",
            DefinitionId = "def-1",
            Location = originalLocation
        };

        var session = new TableSession();
        session.Surfaces.Add(new SurfaceInstance { Id = "surface-1", DefinitionId = "surface-def-1" });
        session.Pieces.Add(targetPiece);

        var request = new ActionRequest
        {
            ActionType = "MovePiece",
            ActorParticipantId = "participant-1",
            Payload = new MovePiecePayload
            {
                PieceId = "piece-1",
                NewLocation = newLocation
            }
        };

        var processor = new ActionProcessor();

        processor.Process(session, request);

        Assert.AreSame(newLocation, targetPiece.Location);
    }

    [TestMethod]
    public void Process_WithMovePiece_AppendsActionRecord()
    {
        var session = new TableSession();
        session.Surfaces.Add(new SurfaceInstance { Id = "surface-1", DefinitionId = "surface-def-1" });
        session.Pieces.Add(new PieceInstance
        {
            Id = "piece-1",
            DefinitionId = "def-1",
            Location = new Location()
        });

        var request = new ActionRequest
        {
            ActionType = "MovePiece",
            ActorParticipantId = "participant-1",
            Payload = new MovePiecePayload
            {
                PieceId = "piece-1",
                NewLocation = new Location
                {
                    SurfaceId = "surface-1",
                    Coordinate = new Coordinate { X = 2, Y = 3 }
                }
            }
        };

        var processor = new ActionProcessor();

        var actionRecord = processor.Process(session, request);

        Assert.AreEqual(1, session.ActionLog.Count);
        Assert.AreEqual(actionRecord, session.ActionLog[0]);
    }

    [TestMethod]
    public void Process_WithMovePiece_OnlyMutatesTargetPiece()
    {
        var targetNewLocation = new Location
        {
            SurfaceId = "surface-1",
            Coordinate = new Coordinate { X = 7, Y = 8 }
        };

        var targetOriginalLocation = new Location
        {
            SurfaceId = "surface-1",
            Coordinate = new Coordinate { X = 1, Y = 1 }
        };

        var untouchedLocation = new Location
        {
            SurfaceId = "surface-1",
            Coordinate = new Coordinate { X = 9, Y = 9 }
        };

        var targetPiece = new PieceInstance { Id = "piece-1", DefinitionId = "def-1", Location = targetOriginalLocation };
        var otherPiece = new PieceInstance { Id = "piece-2", DefinitionId = "def-2", Location = untouchedLocation };

        var session = new TableSession();
        session.Surfaces.Add(new SurfaceInstance { Id = "surface-1", DefinitionId = "surface-def-1" });
        session.Pieces.Add(targetPiece);
        session.Pieces.Add(otherPiece);

        var request = new ActionRequest
        {
            ActionType = "MovePiece",
            ActorParticipantId = "participant-1",
            Payload = new MovePiecePayload
            {
                PieceId = "piece-1",
                NewLocation = targetNewLocation
            }
        };

        var processor = new ActionProcessor();

        processor.Process(session, request);

        Assert.AreSame(targetNewLocation, targetPiece.Location);
        Assert.AreSame(untouchedLocation, otherPiece.Location);
    }

    [TestMethod]
    public void Process_WithMovePiece_WhenPieceMissing_FailsAndDoesNotLog()
    {
        var session = new TableSession();
        session.Surfaces.Add(new SurfaceInstance { Id = "surface-1", DefinitionId = "surface-def-1" });

        var request = new ActionRequest
        {
            ActionType = "MovePiece",
            ActorParticipantId = "participant-1",
            Payload = new MovePiecePayload
            {
                PieceId = "missing-piece",
                NewLocation = new Location
                {
                    SurfaceId = "surface-1",
                    Coordinate = new Coordinate { X = 5, Y = 5 }
                }
            }
        };

        var processor = new ActionProcessor();

        Assert.ThrowsException<InvalidOperationException>(() => processor.Process(session, request));
        Assert.AreEqual(0, session.ActionLog.Count);
    }

    [TestMethod]
    public void Process_WithMovePiece_WhenPayloadInvalid_FailsAndDoesNotLog()
    {
        var session = new TableSession();
        session.Pieces.Add(new PieceInstance { Id = "piece-1", DefinitionId = "def-1", Location = new Location() });

        var request = new ActionRequest
        {
            ActionType = "MovePiece",
            ActorParticipantId = "participant-1",
            Payload = new { PieceId = "piece-1" }
        };

        var processor = new ActionProcessor();

        Assert.ThrowsException<ArgumentException>(() => processor.Process(session, request));
        Assert.AreEqual(0, session.ActionLog.Count);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("   ")]
    public void Process_WithMovePiece_WhenSurfaceIdMissing_FailsAndDoesNotLog(string surfaceId)
    {
        var session = new TableSession();
        session.Surfaces.Add(new SurfaceInstance { Id = "surface-1", DefinitionId = "surface-def-1" });
        session.Pieces.Add(new PieceInstance
        {
            Id = "piece-1",
            DefinitionId = "def-1",
            Location = new Location { SurfaceId = "surface-1", Coordinate = new Coordinate { X = 1, Y = 1 } }
        });

        var request = new ActionRequest
        {
            ActionType = "MovePiece",
            ActorParticipantId = "participant-1",
            Payload = new MovePiecePayload
            {
                PieceId = "piece-1",
                NewLocation = new Location
                {
                    SurfaceId = surfaceId,
                    Coordinate = new Coordinate { X = 2, Y = 2 }
                }
            }
        };

        var processor = new ActionProcessor();

        Assert.ThrowsException<ArgumentException>(() => processor.Process(session, request));
        Assert.AreEqual(0, session.ActionLog.Count);
    }

    [TestMethod]
    public void Process_WithMovePiece_WhenSurfaceMissing_FailsAndDoesNotLog()
    {
        var originalLocation = new Location
        {
            SurfaceId = "surface-1",
            Coordinate = new Coordinate { X = 1, Y = 1 }
        };

        var targetPiece = new PieceInstance
        {
            Id = "piece-1",
            DefinitionId = "def-1",
            Location = originalLocation
        };

        var session = new TableSession();
        session.Surfaces.Add(new SurfaceInstance { Id = "surface-1", DefinitionId = "surface-def-1" });
        session.Pieces.Add(targetPiece);

        var request = new ActionRequest
        {
            ActionType = "MovePiece",
            ActorParticipantId = "participant-1",
            Payload = new MovePiecePayload
            {
                PieceId = "piece-1",
                NewLocation = new Location
                {
                    SurfaceId = "surface-missing",
                    Coordinate = new Coordinate { X = 5, Y = 5 }
                }
            }
        };

        var processor = new ActionProcessor();

        Assert.ThrowsException<InvalidOperationException>(() => processor.Process(session, request));
        Assert.AreSame(originalLocation, targetPiece.Location);
        Assert.AreEqual(0, session.ActionLog.Count);
    }

    [TestMethod]
    public void Process_WithMovePiece_WhenSurfaceExists_StillUpdatesLocationAndLogs()
    {
        var originalLocation = new Location
        {
            SurfaceId = "surface-1",
            Coordinate = new Coordinate { X = 1, Y = 1 }
        };

        var newLocation = new Location
        {
            SurfaceId = "surface-2",
            Coordinate = new Coordinate { X = 8, Y = 3 }
        };

        var targetPiece = new PieceInstance
        {
            Id = "piece-1",
            DefinitionId = "def-1",
            Location = originalLocation
        };

        var session = new TableSession();
        session.Surfaces.Add(new SurfaceInstance { Id = "surface-1", DefinitionId = "surface-def-1" });
        session.Surfaces.Add(new SurfaceInstance { Id = "surface-2", DefinitionId = "surface-def-2" });
        session.Pieces.Add(targetPiece);

        var request = new ActionRequest
        {
            ActionType = "MovePiece",
            ActorParticipantId = "participant-1",
            Payload = new MovePiecePayload
            {
                PieceId = "piece-1",
                NewLocation = newLocation
            }
        };

        var processor = new ActionProcessor();

        var actionRecord = processor.Process(session, request);

        Assert.AreSame(newLocation, targetPiece.Location);
        Assert.AreEqual(1, session.ActionLog.Count);
        Assert.AreEqual(actionRecord, session.ActionLog[0]);
    }

    [TestMethod]
    public void Process_WithRotatePiece_UpdatesTargetPieceRotation()
    {
        var originalRotation = new Rotation { Degrees = 10f };
        var newRotation = new Rotation { Degrees = 90f };

        var targetPiece = new PieceInstance
        {
            Id = "piece-1",
            DefinitionId = "def-1",
            Rotation = originalRotation
        };

        var session = new TableSession();
        session.Pieces.Add(targetPiece);

        var request = new ActionRequest
        {
            ActionType = "RotatePiece",
            ActorParticipantId = "participant-1",
            Payload = new RotatePiecePayload
            {
                PieceId = "piece-1",
                NewRotation = newRotation
            }
        };

        var processor = new ActionProcessor();

        processor.Process(session, request);

        Assert.AreSame(newRotation, targetPiece.Rotation);
    }

    [TestMethod]
    public void Process_WithRotatePiece_AppendsActionRecord()
    {
        var session = new TableSession();
        session.Pieces.Add(new PieceInstance
        {
            Id = "piece-1",
            DefinitionId = "def-1",
            Rotation = new Rotation { Degrees = 0f }
        });

        var request = new ActionRequest
        {
            ActionType = "RotatePiece",
            ActorParticipantId = "participant-1",
            Payload = new RotatePiecePayload
            {
                PieceId = "piece-1",
                NewRotation = new Rotation { Degrees = 180f }
            }
        };

        var processor = new ActionProcessor();

        var actionRecord = processor.Process(session, request);

        Assert.AreEqual(1, session.ActionLog.Count);
        Assert.AreEqual(actionRecord, session.ActionLog[0]);
    }

    [TestMethod]
    public void Process_WithRotatePiece_OnlyMutatesTargetPiece()
    {
        var targetPiece = new PieceInstance
        {
            Id = "piece-1",
            DefinitionId = "def-1",
            Rotation = new Rotation { Degrees = 5f }
        };

        var otherPieceRotation = new Rotation { Degrees = 45f };
        var otherPiece = new PieceInstance
        {
            Id = "piece-2",
            DefinitionId = "def-2",
            Rotation = otherPieceRotation
        };

        var newRotation = new Rotation { Degrees = 270f };

        var session = new TableSession();
        session.Pieces.Add(targetPiece);
        session.Pieces.Add(otherPiece);

        var request = new ActionRequest
        {
            ActionType = "RotatePiece",
            ActorParticipantId = "participant-1",
            Payload = new RotatePiecePayload
            {
                PieceId = "piece-1",
                NewRotation = newRotation
            }
        };

        var processor = new ActionProcessor();

        processor.Process(session, request);

        Assert.AreSame(newRotation, targetPiece.Rotation);
        Assert.AreSame(otherPieceRotation, otherPiece.Rotation);
    }

    [TestMethod]
    public void Process_WithRotatePiece_WhenPieceMissing_FailsAndDoesNotLog()
    {
        var session = new TableSession();

        var request = new ActionRequest
        {
            ActionType = "RotatePiece",
            ActorParticipantId = "participant-1",
            Payload = new RotatePiecePayload
            {
                PieceId = "missing-piece",
                NewRotation = new Rotation { Degrees = 90f }
            }
        };

        var processor = new ActionProcessor();

        Assert.ThrowsException<InvalidOperationException>(() => processor.Process(session, request));
        Assert.AreEqual(0, session.ActionLog.Count);
    }

    [TestMethod]
    public void Process_WithRotatePiece_WhenPayloadInvalid_FailsAndDoesNotLog()
    {
        var session = new TableSession();
        session.Pieces.Add(new PieceInstance
        {
            Id = "piece-1",
            DefinitionId = "def-1",
            Rotation = new Rotation { Degrees = 0f }
        });

        var request = new ActionRequest
        {
            ActionType = "RotatePiece",
            ActorParticipantId = "participant-1",
            Payload = new { PieceId = "piece-1" }
        };

        var processor = new ActionProcessor();

        Assert.ThrowsException<ArgumentException>(() => processor.Process(session, request));
        Assert.AreEqual(0, session.ActionLog.Count);
    }

    [TestMethod]
    public void Process_WithAddMarker_AppendsMarkerToTargetPiece()
    {
        var targetPiece = new PieceInstance
        {
            Id = "piece-1",
            DefinitionId = "def-1"
        };

        var session = new TableSession();
        session.Pieces.Add(targetPiece);

        var request = new ActionRequest
        {
            ActionType = "AddMarker",
            ActorParticipantId = "participant-1",
            Payload = new AddMarkerPayload
            {
                PieceId = "piece-1",
                MarkerId = "marker-stunned"
            }
        };

        var processor = new ActionProcessor();

        processor.Process(session, request);

        Assert.AreEqual(1, targetPiece.MarkerIds.Count);
        Assert.AreEqual("marker-stunned", targetPiece.MarkerIds[0]);
    }

    [TestMethod]
    public void Process_WithAddMarker_AppendsActionRecord()
    {
        var session = new TableSession();
        session.Pieces.Add(new PieceInstance { Id = "piece-1", DefinitionId = "def-1" });

        var request = new ActionRequest
        {
            ActionType = "AddMarker",
            ActorParticipantId = "participant-1",
            Payload = new AddMarkerPayload
            {
                PieceId = "piece-1",
                MarkerId = "marker-hidden"
            }
        };

        var processor = new ActionProcessor();

        var actionRecord = processor.Process(session, request);

        Assert.AreEqual(1, session.ActionLog.Count);
        Assert.AreEqual(actionRecord, session.ActionLog[0]);
    }

    [TestMethod]
    public void Process_WithAddMarker_OnlyMutatesTargetPiece()
    {
        var targetPiece = new PieceInstance
        {
            Id = "piece-1",
            DefinitionId = "def-1"
        };

        var otherPiece = new PieceInstance
        {
            Id = "piece-2",
            DefinitionId = "def-2",
            MarkerIds = ["marker-existing"]
        };

        var session = new TableSession();
        session.Pieces.Add(targetPiece);
        session.Pieces.Add(otherPiece);

        var request = new ActionRequest
        {
            ActionType = "AddMarker",
            ActorParticipantId = "participant-1",
            Payload = new AddMarkerPayload
            {
                PieceId = "piece-1",
                MarkerId = "marker-poisoned"
            }
        };

        var processor = new ActionProcessor();

        processor.Process(session, request);

        CollectionAssert.AreEqual(new List<string> { "marker-poisoned" }, targetPiece.MarkerIds);
        CollectionAssert.AreEqual(new List<string> { "marker-existing" }, otherPiece.MarkerIds);
    }

    [TestMethod]
    public void Process_WithAddMarker_WhenPieceMissing_FailsAndDoesNotLog()
    {
        var session = new TableSession();

        var request = new ActionRequest
        {
            ActionType = "AddMarker",
            ActorParticipantId = "participant-1",
            Payload = new AddMarkerPayload
            {
                PieceId = "missing-piece",
                MarkerId = "marker-any"
            }
        };

        var processor = new ActionProcessor();

        Assert.ThrowsException<InvalidOperationException>(() => processor.Process(session, request));
        Assert.AreEqual(0, session.ActionLog.Count);
    }

    [TestMethod]
    public void Process_WithAddMarker_WhenPayloadInvalid_FailsAndDoesNotLog()
    {
        var session = new TableSession();
        session.Pieces.Add(new PieceInstance { Id = "piece-1", DefinitionId = "def-1" });

        var request = new ActionRequest
        {
            ActionType = "AddMarker",
            ActorParticipantId = "participant-1",
            Payload = new { PieceId = "piece-1" }
        };

        var processor = new ActionProcessor();

        Assert.ThrowsException<ArgumentException>(() => processor.Process(session, request));
        Assert.AreEqual(0, session.ActionLog.Count);
    }

    [TestMethod]
    public void Process_WithAddMarker_WhenMarkerAlreadyPresent_DoesNotDuplicateMarker()
    {
        var targetPiece = new PieceInstance
        {
            Id = "piece-1",
            DefinitionId = "def-1",
            MarkerIds = ["marker-stunned"]
        };

        var session = new TableSession();
        session.Pieces.Add(targetPiece);

        var request = new ActionRequest
        {
            ActionType = "AddMarker",
            ActorParticipantId = "participant-1",
            Payload = new AddMarkerPayload
            {
                PieceId = "piece-1",
                MarkerId = "marker-stunned"
            }
        };

        var processor = new ActionProcessor();

        var actionRecord = processor.Process(session, request);

        CollectionAssert.AreEqual(new List<string> { "marker-stunned" }, targetPiece.MarkerIds);
        Assert.AreEqual(1, session.ActionLog.Count);
        Assert.AreEqual(actionRecord, session.ActionLog[0]);
    }

    [TestMethod]
    public void Process_WithChangePieceState_SetsStateValueOnTargetPiece()
    {
        var targetPiece = new PieceInstance
        {
            Id = "piece-1",
            DefinitionId = "def-1"
        };

        var session = new TableSession();
        session.Pieces.Add(targetPiece);

        var request = new ActionRequest
        {
            ActionType = "ChangePieceState",
            ActorParticipantId = "participant-1",
            Payload = new ChangePieceStatePayload
            {
                PieceId = "piece-1",
                Key = "HitPoints",
                Value = 12
            }
        };

        var processor = new ActionProcessor();

        processor.Process(session, request);

        Assert.IsTrue(targetPiece.State.ContainsKey("HitPoints"));
        Assert.AreEqual(12, targetPiece.State["HitPoints"]);
    }

    [TestMethod]
    public void Process_WithChangePieceState_AppendsActionRecord()
    {
        var session = new TableSession();
        session.Pieces.Add(new PieceInstance { Id = "piece-1", DefinitionId = "def-1" });

        var request = new ActionRequest
        {
            ActionType = "ChangePieceState",
            ActorParticipantId = "participant-1",
            Payload = new ChangePieceStatePayload
            {
                PieceId = "piece-1",
                Key = "Condition",
                Value = "Stunned"
            }
        };

        var processor = new ActionProcessor();

        var actionRecord = processor.Process(session, request);

        Assert.AreEqual(1, session.ActionLog.Count);
        Assert.AreEqual(actionRecord, session.ActionLog[0]);
    }

    [TestMethod]
    public void Process_WithChangePieceState_OnlyMutatesTargetPiece()
    {
        var targetPiece = new PieceInstance
        {
            Id = "piece-1",
            DefinitionId = "def-1"
        };

        var otherPiece = new PieceInstance
        {
            Id = "piece-2",
            DefinitionId = "def-2"
        };
        otherPiece.State["ArmorClass"] = 5;

        var session = new TableSession();
        session.Pieces.Add(targetPiece);
        session.Pieces.Add(otherPiece);

        var request = new ActionRequest
        {
            ActionType = "ChangePieceState",
            ActorParticipantId = "participant-1",
            Payload = new ChangePieceStatePayload
            {
                PieceId = "piece-1",
                Key = "ArmorClass",
                Value = 7
            }
        };

        var processor = new ActionProcessor();

        processor.Process(session, request);

        Assert.AreEqual(7, targetPiece.State["ArmorClass"]);
        Assert.AreEqual(5, otherPiece.State["ArmorClass"]);
    }

    [TestMethod]
    public void Process_WithChangePieceState_WhenPieceMissing_FailsAndDoesNotLog()
    {
        var session = new TableSession();

        var request = new ActionRequest
        {
            ActionType = "ChangePieceState",
            ActorParticipantId = "participant-1",
            Payload = new ChangePieceStatePayload
            {
                PieceId = "missing-piece",
                Key = "HitPoints",
                Value = 10
            }
        };

        var processor = new ActionProcessor();

        Assert.ThrowsException<InvalidOperationException>(() => processor.Process(session, request));
        Assert.AreEqual(0, session.ActionLog.Count);
    }

    [TestMethod]
    public void Process_WithChangePieceState_WhenPayloadInvalid_FailsAndDoesNotLog()
    {
        var session = new TableSession();
        session.Pieces.Add(new PieceInstance { Id = "piece-1", DefinitionId = "def-1" });

        var request = new ActionRequest
        {
            ActionType = "ChangePieceState",
            ActorParticipantId = "participant-1",
            Payload = new { PieceId = "piece-1", Key = "HitPoints", Value = 10 }
        };

        var processor = new ActionProcessor();

        Assert.ThrowsException<ArgumentException>(() => processor.Process(session, request));
        Assert.AreEqual(0, session.ActionLog.Count);
    }

    [TestMethod]
    public void Process_WithChangePieceState_WhenKeyExists_OverwritesValue()
    {
        var targetPiece = new PieceInstance
        {
            Id = "piece-1",
            DefinitionId = "def-1"
        };
        targetPiece.State["HitPoints"] = 8;

        var session = new TableSession();
        session.Pieces.Add(targetPiece);

        var request = new ActionRequest
        {
            ActionType = "ChangePieceState",
            ActorParticipantId = "participant-1",
            Payload = new ChangePieceStatePayload
            {
                PieceId = "piece-1",
                Key = "HitPoints",
                Value = 4
            }
        };

        var processor = new ActionProcessor();

        var actionRecord = processor.Process(session, request);

        Assert.AreEqual(4, targetPiece.State["HitPoints"]);
        Assert.AreEqual(1, session.ActionLog.Count);
        Assert.AreEqual(actionRecord, session.ActionLog[0]);
    }

    [TestMethod]
    public void Process_WithRemoveMarker_RemovesMarkerFromTargetPiece()
    {
        var targetPiece = new PieceInstance
        {
            Id = "piece-1",
            DefinitionId = "def-1",
            MarkerIds = ["marker-stunned", "marker-hidden"]
        };

        var session = new TableSession();
        session.Pieces.Add(targetPiece);

        var request = new ActionRequest
        {
            ActionType = "RemoveMarker",
            ActorParticipantId = "participant-1",
            Payload = new RemoveMarkerPayload
            {
                PieceId = "piece-1",
                MarkerId = "marker-hidden"
            }
        };

        var processor = new ActionProcessor();

        processor.Process(session, request);

        CollectionAssert.AreEqual(new List<string> { "marker-stunned" }, targetPiece.MarkerIds);
    }

    [TestMethod]
    public void Process_WithRemoveMarker_AppendsActionRecord()
    {
        var session = new TableSession();
        session.Pieces.Add(new PieceInstance
        {
            Id = "piece-1",
            DefinitionId = "def-1",
            MarkerIds = ["marker-a"]
        });

        var request = new ActionRequest
        {
            ActionType = "RemoveMarker",
            ActorParticipantId = "participant-1",
            Payload = new RemoveMarkerPayload
            {
                PieceId = "piece-1",
                MarkerId = "marker-a"
            }
        };

        var processor = new ActionProcessor();

        var actionRecord = processor.Process(session, request);

        Assert.AreEqual(1, session.ActionLog.Count);
        Assert.AreEqual(actionRecord, session.ActionLog[0]);
    }

    [TestMethod]
    public void Process_WithRemoveMarker_OnlyMutatesTargetPiece()
    {
        var targetPiece = new PieceInstance
        {
            Id = "piece-1",
            DefinitionId = "def-1",
            MarkerIds = ["marker-a", "marker-b"]
        };

        var otherPiece = new PieceInstance
        {
            Id = "piece-2",
            DefinitionId = "def-2",
            MarkerIds = ["marker-x"]
        };

        var session = new TableSession();
        session.Pieces.Add(targetPiece);
        session.Pieces.Add(otherPiece);

        var request = new ActionRequest
        {
            ActionType = "RemoveMarker",
            ActorParticipantId = "participant-1",
            Payload = new RemoveMarkerPayload
            {
                PieceId = "piece-1",
                MarkerId = "marker-a"
            }
        };

        var processor = new ActionProcessor();

        processor.Process(session, request);

        CollectionAssert.AreEqual(new List<string> { "marker-b" }, targetPiece.MarkerIds);
        CollectionAssert.AreEqual(new List<string> { "marker-x" }, otherPiece.MarkerIds);
    }

    [TestMethod]
    public void Process_WithMovePiece_RecordsMoveEventCategoryAndReferences()
    {
        var session = new TableSession
        {
            TurnNumber = 3
        };

        session.Surfaces.Add(new SurfaceInstance { Id = "surface-1", DefinitionId = "surface-def-1" });
        session.Pieces.Add(new PieceInstance
        {
            Id = "piece-1",
            DefinitionId = "def-1",
            Location = new Location
            {
                SurfaceId = "surface-1",
                Coordinate = new Coordinate { X = 0, Y = 0 }
            }
        });

        var request = new ActionRequest
        {
            ActionType = "MovePiece",
            ActorParticipantId = "participant-1",
            Payload = new MovePiecePayload
            {
                PieceId = "piece-1",
                NewLocation = new Location
                {
                    SurfaceId = "surface-1",
                    Coordinate = new Coordinate { X = 2, Y = 2 }
                }
            }
        };

        var processor = new ActionProcessor();

        var actionRecord = processor.Process(session, request);

        Assert.AreEqual(ActionEventCategory.Move, actionRecord.EventCategory);
        Assert.AreEqual("piece-1", actionRecord.ReferencedPieceId);
        Assert.AreEqual(3, actionRecord.ReferencedTurnNumber);
    }

    [TestMethod]
    public void Process_WithNonMoveAction_RecordsGameplayEventCategory()
    {
        var session = new TableSession();
        var request = new ActionRequest
        {
            ActionType = "NoOpAction",
            ActorParticipantId = "participant-1",
            Payload = new { Any = "value" }
        };

        var processor = new ActionProcessor();

        var actionRecord = processor.Process(session, request);

        Assert.AreEqual(ActionEventCategory.Gameplay, actionRecord.EventCategory);
        Assert.AreEqual(string.Empty, actionRecord.ReferencedPieceId);
        Assert.AreEqual(1, actionRecord.ReferencedTurnNumber);
    }

    [TestMethod]
    public void Process_WithRemoveMarker_WhenPieceMissing_FailsAndDoesNotLog()
    {
        var session = new TableSession();

        var request = new ActionRequest
        {
            ActionType = "RemoveMarker",
            ActorParticipantId = "participant-1",
            Payload = new RemoveMarkerPayload
            {
                PieceId = "missing-piece",
                MarkerId = "marker-a"
            }
        };

        var processor = new ActionProcessor();

        Assert.ThrowsException<InvalidOperationException>(() => processor.Process(session, request));
        Assert.AreEqual(0, session.ActionLog.Count);
    }

    [TestMethod]
    public void Process_WithRemoveMarker_WhenPayloadInvalid_FailsAndDoesNotLog()
    {
        var session = new TableSession();
        session.Pieces.Add(new PieceInstance
        {
            Id = "piece-1",
            DefinitionId = "def-1",
            MarkerIds = ["marker-a"]
        });

        var request = new ActionRequest
        {
            ActionType = "RemoveMarker",
            ActorParticipantId = "participant-1",
            Payload = new { PieceId = "piece-1" }
        };

        var processor = new ActionProcessor();

        Assert.ThrowsException<ArgumentException>(() => processor.Process(session, request));
        Assert.AreEqual(0, session.ActionLog.Count);
    }

    [TestMethod]
    public void Process_WithRemoveMarker_WhenMarkerMissing_DoesNotThrowAndDoesNotMutateMarkerList()
    {
        var targetPiece = new PieceInstance
        {
            Id = "piece-1",
            DefinitionId = "def-1",
            MarkerIds = ["marker-a"]
        };

        var session = new TableSession();
        session.Pieces.Add(targetPiece);

        var request = new ActionRequest
        {
            ActionType = "RemoveMarker",
            ActorParticipantId = "participant-1",
            Payload = new RemoveMarkerPayload
            {
                PieceId = "piece-1",
                MarkerId = "marker-missing"
            }
        };

        var processor = new ActionProcessor();

        var actionRecord = processor.Process(session, request);

        CollectionAssert.AreEqual(new List<string> { "marker-a" }, targetPiece.MarkerIds);
        Assert.AreEqual(1, session.ActionLog.Count);
        Assert.AreEqual(actionRecord, session.ActionLog[0]);
    }
}
