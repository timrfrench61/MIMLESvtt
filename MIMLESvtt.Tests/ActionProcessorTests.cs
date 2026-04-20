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
            ActionType = "MovePiece",
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
            ActionType = "AddMarker",
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
}
