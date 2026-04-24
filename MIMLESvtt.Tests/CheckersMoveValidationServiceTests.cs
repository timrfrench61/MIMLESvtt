using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src.Domain.Models.Pieces;
using MIMLESvtt.src.Domain.Models.Placement;
using MIMLESvtt.src.Domain.Rules;

namespace MIMLESvtt.Tests;

[TestClass]
public class CheckersMoveValidationServiceTests
{
    [TestMethod]
    public void IsLegalBasicMove_RedManForwardDiagonal_IsAllowed()
    {
        var service = new CheckersMoveValidationService();
        var piece = CreatePiece("Red", kinged: false, x: 2, y: 2);

        var isLegal = service.IsLegalBasicMove(piece, 3, 3, out var error);

        Assert.IsTrue(isLegal);
        Assert.AreEqual(string.Empty, error);
    }

    [TestMethod]
    public void IsLegalBasicMove_RedManBackwardDiagonal_IsRejected()
    {
        var service = new CheckersMoveValidationService();
        var piece = CreatePiece("Red", kinged: false, x: 2, y: 2);

        var isLegal = service.IsLegalBasicMove(piece, 3, 1, out var error);

        Assert.IsFalse(isLegal);
        StringAssert.Contains(error, "Red checkers men move forward only");
    }

    [TestMethod]
    public void IsLegalBasicMove_BlackManForwardDiagonal_IsAllowed()
    {
        var service = new CheckersMoveValidationService();
        var piece = CreatePiece("Black", kinged: false, x: 5, y: 5);

        var isLegal = service.IsLegalBasicMove(piece, 4, 4, out var error);

        Assert.IsTrue(isLegal);
        Assert.AreEqual(string.Empty, error);
    }

    [TestMethod]
    public void IsLegalBasicMove_KingBackwardDiagonal_IsAllowed()
    {
        var service = new CheckersMoveValidationService();
        var piece = CreatePiece("Red", kinged: true, x: 4, y: 4);

        var isLegal = service.IsLegalBasicMove(piece, 3, 3, out var error);

        Assert.IsTrue(isLegal);
        Assert.AreEqual(string.Empty, error);
    }

    private static PieceInstance CreatePiece(string side, bool kinged, float x, float y)
    {
        return new PieceInstance
        {
            Id = "piece-1",
            DefinitionId = "checkers-piece",
            Location = new Location
            {
                SurfaceId = "checkers-board",
                Coordinate = new Coordinate { X = x, Y = y }
            },
            State =
            {
                ["Side"] = side,
                ["Kinged"] = kinged
            }
        };
    }
}
