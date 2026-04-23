using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Persistence.VttSessionNSPC;

namespace MIMLESvtt.Tests;

[TestClass]
public class PlayerSeatModelTests
{
    [TestMethod]
    public void PlayerSeat_Model_SupportsRoleAndSideAssignments()
    {
        var gmSeat = new PlayerSeat
        {
            Id = "seat-gm",
            ParticipantId = "participant-gm",
            Role = ParticipantRole.GM,
            Side = "Blue",
            Faction = "Alliance"
        };

        var playerSeat = new PlayerSeat
        {
            Id = "seat-player",
            ParticipantId = "participant-player",
            Role = ParticipantRole.Player,
            Side = "Red",
            Faction = "Horde"
        };

        var observerSeat = new PlayerSeat
        {
            Id = "seat-observer",
            ParticipantId = "participant-observer",
            Role = ParticipantRole.Observer,
            Side = string.Empty,
            Faction = string.Empty
        };

        Assert.AreEqual(ParticipantRole.GM, gmSeat.Role);
        Assert.AreEqual("Blue", gmSeat.Side);
        Assert.AreEqual("Alliance", gmSeat.Faction);

        Assert.AreEqual(ParticipantRole.Player, playerSeat.Role);
        Assert.AreEqual("Red", playerSeat.Side);
        Assert.AreEqual("Horde", playerSeat.Faction);

        Assert.AreEqual(ParticipantRole.Observer, observerSeat.Role);
    }

    [TestMethod]
    public void VttSessionSnapshotSerializer_RoundTrip_PreservesPlayerSeats()
    {
        var serializer = new VttSessionSnapshotSerializer();
        var session = new VttSession
        {
            Id = "session-seat-1",
            Title = "Seat Session",
            Participants =
            [
                new Participant { Id = "p-gm", Name = "Game Master", Role = ParticipantRole.GM },
                new Participant { Id = "p-1", Name = "Player One", Role = ParticipantRole.Player },
                new Participant { Id = "p-ob", Name = "Observer", Role = ParticipantRole.Observer }
            ],
            PlayerSeats =
            [
                new PlayerSeat { Id = "seat-gm", ParticipantId = "p-gm", Role = ParticipantRole.GM, Side = "Blue", Faction = "Alliance" },
                new PlayerSeat { Id = "seat-1", ParticipantId = "p-1", Role = ParticipantRole.Player, Side = "Red", Faction = "Horde" },
                new PlayerSeat { Id = "seat-ob", ParticipantId = "p-ob", Role = ParticipantRole.Observer }
            ]
        };

        var json = serializer.Save(session);
        var loaded = serializer.Load(json);

        Assert.AreEqual(3, loaded.PlayerSeats.Count);
        Assert.AreEqual("seat-gm", loaded.PlayerSeats[0].Id);
        Assert.AreEqual("p-gm", loaded.PlayerSeats[0].ParticipantId);
        Assert.AreEqual(ParticipantRole.GM, loaded.PlayerSeats[0].Role);
        Assert.AreEqual("Blue", loaded.PlayerSeats[0].Side);
        Assert.AreEqual("Alliance", loaded.PlayerSeats[0].Faction);

        Assert.AreEqual("seat-ob", loaded.PlayerSeats[2].Id);
        Assert.AreEqual(ParticipantRole.Observer, loaded.PlayerSeats[2].Role);
    }
}
