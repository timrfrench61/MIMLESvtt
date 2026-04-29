using Microsoft.VisualStudio.TestTools.UnitTesting;
using VttMvuView.Tabletop;

namespace MIMLESvtt.Tests;

[TestClass]
public class TabletopUpdateTests
{
    [TestMethod]
    public void SelectToken_SetsSelectedTokenId()
    {
        var state = BuildInitialState();

        var result = TabletopUpdate.Reduce(state, new SelectTokenAction("T-1"));

        Assert.AreEqual("T-1", result.SelectedTokenId);
        Assert.AreEqual("Selected token T-1.", result.StatusMessage);
    }

    [TestMethod]
    public void MoveSelectedToken_UpdatesTokenCoordinates()
    {
        var state = BuildInitialState() with { SelectedTokenId = "T-1" };

        var result = TabletopUpdate.Reduce(state, new MoveSelectedTokenAction(1, -1));
        var token = result.Tokens.First(t => t.TokenId == "T-1");

        Assert.AreEqual(3, token.X);
        Assert.AreEqual(1, token.Y);
    }

    [TestMethod]
    public void RotateSelectedToken_UpdatesRotation()
    {
        var state = BuildInitialState() with { SelectedTokenId = "T-2" };

        var result = TabletopUpdate.Reduce(state, new RotateSelectedTokenAction(90));
        var token = result.Tokens.First(t => t.TokenId == "T-2");

        Assert.AreEqual(270, token.RotationDegrees);
    }

    [TestMethod]
    public void EndTurn_IncrementsTurnNumber()
    {
        var state = BuildInitialState();

        var result = TabletopUpdate.Reduce(state, new EndTurnAction());

        Assert.AreEqual(2, result.TurnNumber);
        Assert.AreEqual("Main", result.TurnPhase);
    }

    private static TabletopState BuildInitialState()
    {
        return new TabletopState(
            BoardWidth: 8,
            BoardHeight: 8,
            Tokens:
            [
                new TabletopTokenState("T-1", "Blue Unit", 2, 2, 0),
                new TabletopTokenState("T-2", "Red Unit", 5, 5, 180)
            ],
            SelectedTokenId: null,
            TurnNumber: 1,
            TurnPhase: "Main",
            ActiveToolId: "Select",
            LastDiceRoll: null,
            StatusMessage: null);
    }
}
