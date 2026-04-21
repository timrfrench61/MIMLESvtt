using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src;

namespace MIMLESvtt.Tests;

[TestClass]
public class ActionValidationServiceTests
{
    [TestMethod]
    public void ActionValidation_InvalidPiece_Fails()
    {
        var validator = new ActionValidationService();
        var session = new TableSession();
        session.Surfaces.Add(new SurfaceInstance { Id = "surface-1", DefinitionId = "def-surface-1" });

        var request = new ActionRequest
        {
            ActionType = "MovePiece",
            ActorParticipantId = "gm",
            Payload = new MovePiecePayload
            {
                PieceId = "missing-piece",
                NewLocation = new Location
                {
                    SurfaceId = "surface-1",
                    Coordinate = new Coordinate { X = 1, Y = 2 }
                }
            }
        };

        var result = validator.Validate(request, session);

        Assert.IsFalse(result.IsValid);
        StringAssert.Contains(result.Message, "target piece");
    }

    [TestMethod]
    public void ActionValidation_InvalidSurface_Fails()
    {
        var validator = new ActionValidationService();
        var session = new TableSession();
        session.Surfaces.Add(new SurfaceInstance { Id = "surface-1", DefinitionId = "def-surface-1" });
        session.Pieces.Add(new PieceInstance
        {
            Id = "piece-1",
            DefinitionId = "def-piece-1",
            Location = new Location
            {
                SurfaceId = "surface-1",
                Coordinate = new Coordinate { X = 0, Y = 0 }
            }
        });

        var request = new ActionRequest
        {
            ActionType = "MovePiece",
            ActorParticipantId = "gm",
            Payload = new MovePiecePayload
            {
                PieceId = "piece-1",
                NewLocation = new Location
                {
                    SurfaceId = "missing-surface",
                    Coordinate = new Coordinate { X = 2, Y = 3 }
                }
            }
        };

        var result = validator.Validate(request, session);

        Assert.IsFalse(result.IsValid);
        StringAssert.Contains(result.Message, "target surface");
    }

    [TestMethod]
    public void ActionValidation_ValidMove_Succeeds()
    {
        var validator = new ActionValidationService();
        var session = new TableSession();
        session.Surfaces.Add(new SurfaceInstance { Id = "surface-1", DefinitionId = "def-surface-1" });
        session.Pieces.Add(new PieceInstance
        {
            Id = "piece-1",
            DefinitionId = "def-piece-1",
            OwnerParticipantId = "gm",
            Location = new Location
            {
                SurfaceId = "surface-1",
                Coordinate = new Coordinate { X = 0, Y = 0 }
            }
        });

        var request = new ActionRequest
        {
            ActionType = "MovePiece",
            ActorParticipantId = "gm",
            Payload = new MovePiecePayload
            {
                PieceId = "piece-1",
                NewLocation = new Location
                {
                    SurfaceId = "surface-1",
                    Coordinate = new Coordinate { X = 4, Y = 5 }
                }
            }
        };

        var result = validator.Validate(request, session);

        Assert.IsTrue(result.IsValid);
        Assert.AreEqual(string.Empty, result.Message);
    }
}
