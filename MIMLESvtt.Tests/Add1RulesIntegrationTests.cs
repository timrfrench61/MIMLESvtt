using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src;
using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Rules;
using MIMLESvtt.src.Domain.Rules.Add1;

namespace MIMLESvtt.Tests;

[TestClass]
public class Add1RulesIntegrationTests
{
    [TestMethod]
    public void RulesOrchestrator_WithAdd1AttackOutcome_AppliesAndLogsThroughEngineCommandFlow()
    {
        var session = new VttSession
        {
            Id = "session-add1-integration",
            Title = "AD&D1 Integration Session"
        };

        var commandService = new RecordingCommandService(session);
        var dice = new DiceRandomizationService(new DeterministicDiceRandomProvider([15]));
        var module = new Add1RulesModuleScaffold(dice);
        var orchestrator = new RulesActionOrchestrator(module);

        var request = new ActionRequest
        {
            ActionType = Add1RulesModuleScaffold.ActionTypeAttack,
            ActorParticipantId = "player-1",
            Payload = new Add1AttackRequest
            {
                AttackerParticipantId = "player-1",
                AttackerPieceId = "piece-attacker-1",
                TargetPieceId = "piece-target-1",
                AttackType = "melee",
                ToHitThreshold = 12,
                DamageAmountOnHit = 6,
                StatusOnHit = "stunned"
            }
        };

        var record = orchestrator.Process(commandService, "add1", "v1", "scenario-add1", request);

        Assert.AreEqual(1, commandService.Requests.Count);
        Assert.AreEqual("NoOpAction", commandService.Requests[0].ActionType);
        Assert.AreEqual(1, session.ActionLog.Count);
        Assert.AreEqual(record.Id, session.ActionLog[0].Id);

        var add1Payload = commandService.Requests[0].Payload as Add1AttackResult;
        Assert.IsNotNull(add1Payload);
        Assert.IsTrue(add1Payload!.IsHit);
        Assert.AreEqual(6, add1Payload.DamageEffect.DamageAmount);
    }

    [TestMethod]
    public void RulesOrchestrator_WithAdd1SaveOutcome_AppliesAndLogsThroughEngineCommandFlow()
    {
        var session = new VttSession
        {
            Id = "session-add1-save-integration",
            Title = "AD&D1 Save Integration Session"
        };

        var commandService = new RecordingCommandService(session);
        var dice = new DiceRandomizationService(new DeterministicDiceRandomProvider([11]));
        var module = new Add1RulesModuleScaffold(dice);
        var orchestrator = new RulesActionOrchestrator(module);

        var request = new ActionRequest
        {
            ActionType = Add1RulesModuleScaffold.ActionTypeSavingThrow,
            ActorParticipantId = "player-2",
            Payload = new Add1SavingThrowRequest
            {
                ActorParticipantId = "player-2",
                SaveType = "Poison",
                SaveTarget = 12
            }
        };

        var record = orchestrator.Process(commandService, "add1", "v1", "scenario-add1", request);

        Assert.AreEqual(1, commandService.Requests.Count);
        Assert.AreEqual("NoOpAction", commandService.Requests[0].ActionType);
        Assert.AreEqual(1, session.ActionLog.Count);
        Assert.AreEqual(record.Id, session.ActionLog[0].Id);

        var add1Payload = commandService.Requests[0].Payload as Add1SavingThrowResult;
        Assert.IsNotNull(add1Payload);
        Assert.IsFalse(add1Payload!.SavePassed);
        StringAssert.Contains(add1Payload.Summary, "failed");
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
