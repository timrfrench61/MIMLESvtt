using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Persistence.VttSessionNSPC;

namespace MIMLESvtt.Tests;

[TestClass]
public class TurnStateModelTests
{
    [TestMethod]
    public void TurnState_Model_SupportsSeatPhaseAndCounters()
    {
        var turnState = new TurnState
        {
            CurrentSeatId = "seat-gm",
            CurrentPhase = "Action",
            CurrentPhaseIndex = 1,
            RoundNumber = 3,
            TurnNumber = 7,
            PhaseSequence = ["Setup", "Action", "Resolution"]
        };

        Assert.AreEqual("seat-gm", turnState.CurrentSeatId);
        Assert.AreEqual("Action", turnState.CurrentPhase);
        Assert.AreEqual(1, turnState.CurrentPhaseIndex);
        Assert.AreEqual(3, turnState.RoundNumber);
        Assert.AreEqual(7, turnState.TurnNumber);
        CollectionAssert.AreEqual(new List<string> { "Setup", "Action", "Resolution" }, turnState.PhaseSequence);
    }

    [TestMethod]
    public void VttSessionSnapshotSerializer_RoundTrip_PreservesTurnStateSequence()
    {
        var serializer = new VttSessionSnapshotSerializer();
        var session = new VttSession
        {
            Id = "session-turnstate-1",
            Title = "Turn State Session",
            TurnState = new TurnState
            {
                CurrentSeatId = "seat-1",
                CurrentPhase = "Resolution",
                CurrentPhaseIndex = 2,
                RoundNumber = 4,
                TurnNumber = 11,
                PhaseSequence = ["Setup", "Action", "Resolution"],
                Metadata =
                {
                    ["InitiativeMode"] = "Individual",
                    ["StrictPhases"] = true
                }
            }
        };

        var json = serializer.Save(session);
        var loaded = serializer.Load(json);

        Assert.IsNotNull(loaded.TurnState);
        Assert.AreEqual("seat-1", loaded.TurnState.CurrentSeatId);
        Assert.AreEqual("Resolution", loaded.TurnState.CurrentPhase);
        Assert.AreEqual(2, loaded.TurnState.CurrentPhaseIndex);
        Assert.AreEqual(4, loaded.TurnState.RoundNumber);
        Assert.AreEqual(11, loaded.TurnState.TurnNumber);
        CollectionAssert.AreEqual(new List<string> { "Setup", "Action", "Resolution" }, loaded.TurnState.PhaseSequence);

        var initiativeMode = (JsonElement)loaded.TurnState.Metadata["InitiativeMode"];
        var strictPhases = (JsonElement)loaded.TurnState.Metadata["StrictPhases"];

        Assert.AreEqual("Individual", initiativeMode.GetString());
        Assert.IsTrue(strictPhases.GetBoolean());
    }
}
