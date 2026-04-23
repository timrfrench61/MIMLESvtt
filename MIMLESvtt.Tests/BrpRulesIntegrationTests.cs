using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src;
using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Rules;
using MIMLESvtt.src.Domain.Rules.Brp;

namespace MIMLESvtt.Tests;

[TestClass]
public class BrpRulesIntegrationTests
{
    [TestMethod]
    public void RulesOrchestrator_WithBrpSkillOutcome_MapsToEngineApplyLogPipeline()
    {
        var session = new VttSession
        {
            Id = "session-brp-integration",
            Title = "BRP Integration Session"
        };

        var commandService = new RecordingCommandService(session);
        var dice = new DiceRandomizationService(new DeterministicDiceRandomProvider([42]));
        var module = new BrpRulesModuleScaffold(dice, new BrpOpposedCheckService());
        var orchestrator = new RulesActionOrchestrator(module);

        var request = new ActionRequest
        {
            ActionType = BrpRulesModuleScaffold.ActionTypeSkillResolution,
            ActorParticipantId = "investigator-1",
            Payload = new BrpSkillResolutionRequest
            {
                ActorParticipantId = "investigator-1",
                SkillId = "Stealth",
                TargetValue = 55,
                Modifier = 5,
                ContextTag = "brp:integration:skill"
            }
        };

        var record = orchestrator.Process(commandService, "brp", "v1", "scenario-brp", request);

        Assert.AreEqual(1, commandService.Requests.Count);
        Assert.AreEqual("NoOpAction", commandService.Requests[0].ActionType);
        Assert.AreEqual(1, session.ActionLog.Count);
        Assert.AreEqual(record.Id, session.ActionLog[0].Id);

        var payload = commandService.Requests[0].Payload as BrpPercentileCheckResult;
        Assert.IsNotNull(payload);
        Assert.IsTrue(payload!.Passed);
        Assert.AreEqual(42, payload.RolledValue);
    }

    [TestMethod]
    public void RulesOrchestrator_WithBrpOpposedOutcome_MapsToEngineApplyLogPipeline()
    {
        var session = new VttSession
        {
            Id = "session-brp-opposed-integration",
            Title = "BRP Opposed Integration Session"
        };

        var commandService = new RecordingCommandService(session);
        var dice = new DiceRandomizationService(new DeterministicDiceRandomProvider([40, 62]));
        var module = new BrpRulesModuleScaffold(dice, new BrpOpposedCheckService());
        var orchestrator = new RulesActionOrchestrator(module);

        var request = new ActionRequest
        {
            ActionType = BrpRulesModuleScaffold.ActionTypeOpposedCheck,
            ActorParticipantId = "investigator-1",
            Payload = new BrpOpposedCheckRequest
            {
                AttackerParticipantId = "investigator-1",
                AttackerTargetValue = 55,
                AttackerModifier = 0,
                DefenderParticipantId = "investigator-2",
                DefenderTargetValue = 60,
                DefenderModifier = 0
            }
        };

        var record = orchestrator.Process(commandService, "brp", "v1", "scenario-brp", request);

        Assert.AreEqual(1, commandService.Requests.Count);
        Assert.AreEqual("NoOpAction", commandService.Requests[0].ActionType);
        Assert.AreEqual(1, session.ActionLog.Count);
        Assert.AreEqual(record.Id, session.ActionLog[0].Id);

        var payload = commandService.Requests[0].Payload as BrpOpposedCheckResult;
        Assert.IsNotNull(payload);
        Assert.AreEqual("investigator-1", payload!.WinnerParticipantId);
        Assert.IsFalse(payload.WasTie);
    }

    private sealed class RecordingCommandService : IVttSessionCommandService
    {
        public RecordingCommandService(VttSession session)
        {
            CurrentVttSession = session;
        }

        public VttSession? CurrentVttSession { get; }

        public List<ActionRequest> Requests { get; } = [];

        public ActionRecord ProcessAction(ActionRequest request)
        {
            Requests.Add(request);

            var record = new ActionRecord
            {
                Id = Guid.NewGuid().ToString("N"),
                ActionType = request.ActionType,
                ActorParticipantId = request.ActorParticipantId,
                TimestampUtc = DateTime.UtcNow,
                Payload = request.Payload
            };

            CurrentVttSession!.ActionLog.Add(record);
            return record;
        }
    }
}
