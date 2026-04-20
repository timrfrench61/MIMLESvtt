using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src;

namespace MIMLESvtt.Tests;

[TestClass]
public class TableSessionPersistenceServiceTests
{
    [TestMethod]
    public void PersistenceService_Save_DelegatesAndReturnsJson()
    {
        var service = new TableSessionPersistenceService();
        var session = CreateSessionFixture();

        var json = service.Save(session);

        StringAssert.Contains(json, "\"Version\":1");
        StringAssert.Contains(json, "\"TableSession\"");
        StringAssert.Contains(json, "\"Id\":\"session-1\"");
    }

    [TestMethod]
    public void PersistenceService_Load_DelegatesAndReturnsTableSession()
    {
        var service = new TableSessionPersistenceService();
        var session = CreateSessionFixture();

        var json = service.Save(session);
        var loaded = service.Load(json);

        Assert.AreEqual("session-1", loaded.Id);
        Assert.AreEqual("Persistence Service Session", loaded.Title);
        Assert.AreEqual(1, loaded.Pieces.Count);
        Assert.AreEqual(1, loaded.ActionLog.Count);
    }

    [TestMethod]
    public void Save_WhenSessionIsNull_FailsClearly()
    {
        var service = new TableSessionPersistenceService();

        Assert.ThrowsException<ArgumentNullException>(() => service.Save(null!));
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("   ")]
    public void Load_WhenJsonIsNullEmptyOrWhitespace_FailsClearly(string json)
    {
        var service = new TableSessionPersistenceService();

        Assert.ThrowsException<ArgumentException>(() => service.Load(json));
    }

    [TestMethod]
    public void Load_WhenJsonMalformed_FailsClearly()
    {
        var service = new TableSessionPersistenceService();

        Assert.ThrowsException<JsonException>(() => service.Load("{ \"Version\": 1, \"TableSession\": "));
    }

    private static TableSession CreateSessionFixture()
    {
        return new TableSession
        {
            Id = "session-1",
            Title = "Persistence Service Session",
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
                            X = 3,
                            Y = 5
                        }
                    }
                }
            ],
            Options = new TableOptions
            {
                EnableFog = true,
                EnableTurnTracker = false
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
                    ActionType = "NoOpAction",
                    ActorParticipantId = "participant-1",
                    TimestampUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    Payload = null
                }
            ],
            ModuleState =
            {
                ["Turn"] = 1
            }
        };
    }
}
