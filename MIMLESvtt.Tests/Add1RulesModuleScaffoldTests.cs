using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Rules;
using MIMLESvtt.src.Domain.Rules.Add1;

namespace MIMLESvtt.Tests;

[TestClass]
public class Add1RulesModuleScaffoldTests
{
    [TestMethod]
    public void ValidateAttackRequest_MissingTarget_Fails()
    {
        var module = new Add1RulesModuleScaffold(new DiceRandomizationService(new DeterministicDiceRandomProvider([10])));
        var context = CreateContext();
        var request = new RulesActionRequest
        {
            ActionType = Add1RulesModuleScaffold.ActionTypeAttack,
            ActorParticipantId = "player-1",
            Payload = new Add1AttackRequest
            {
                AttackerParticipantId = "player-1",
                AttackerPieceId = "piece-attacker-1",
                TargetPieceId = string.Empty,
                AttackType = "melee",
                ToHitThreshold = 10,
                DamageAmountOnHit = 5
            }
        };

        var validation = module.ValidateAction(context, request);

        Assert.IsFalse(validation.IsValid);
        Assert.AreEqual("Add1Attack target piece id is required.", validation.Message);
    }

    [TestMethod]
    public void ResolveAttack_WithHitThreshold_ReturnsHitPayload()
    {
        var module = new Add1RulesModuleScaffold(new DiceRandomizationService(new DeterministicDiceRandomProvider([16])));
        var context = CreateContext();
        var request = new RulesActionRequest
        {
            ActionType = Add1RulesModuleScaffold.ActionTypeAttack,
            ActorParticipantId = "player-1",
            Payload = new Add1AttackRequest
            {
                AttackerParticipantId = "player-1",
                AttackerPieceId = "piece-attacker-1",
                TargetPieceId = "piece-1",
                AttackType = "melee",
                AttackModifier = 2,
                ToHitThreshold = 12,
                DamageAmountOnHit = 7,
                StatusOnHit = "stunned"
            }
        };

        var resolution = module.ResolveAction(context, request);

        Assert.IsTrue(resolution.IsSuccess);
        Assert.IsNotNull(resolution.EngineActionRequest);
        Assert.AreEqual("NoOpAction", resolution.EngineActionRequest!.ActionType);
        var payload = resolution.EngineActionRequest.Payload as Add1AttackResult;
        Assert.IsNotNull(payload);
        Assert.IsTrue(payload!.IsHit);
        Assert.IsTrue(payload.IsSuccess);
        Assert.AreEqual(18, payload.AttackRoll);
        Assert.AreEqual(2, payload.AppliedModifier);
        Assert.AreEqual("add1:attack", payload.RollDetails.ContextTag);
        Assert.AreEqual(7, payload.DamageEffect.DamageAmount);
        Assert.AreEqual("stunned", payload.StatusEffect.ConditionMarkerId);
        StringAssert.Contains(payload.Summary, "hit");
    }

    [TestMethod]
    public void ResolveSavingThrow_ReturnsStructuredResult()
    {
        var module = new Add1RulesModuleScaffold(new DiceRandomizationService(new DeterministicDiceRandomProvider([14])));
        var context = CreateContext();
        var request = new RulesActionRequest
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

        var resolution = module.ResolveAction(context, request);

        Assert.IsTrue(resolution.IsSuccess);
        var payload = resolution.EngineActionRequest!.Payload as Add1SavingThrowResult;
        Assert.IsNotNull(payload);
        Assert.IsTrue(payload!.IsSuccess);
        Assert.IsTrue(payload.SavePassed);
        Assert.AreEqual(14, payload.SaveRoll);
        Assert.AreEqual(12, payload.SaveTarget);
        Assert.AreEqual("add1:save", payload.RollDetails.ContextTag);
        StringAssert.Contains(payload.Summary, "passed");
    }

    [TestMethod]
    public void ValidateAttackRequest_MissingAttackerPiece_Fails()
    {
        var module = new Add1RulesModuleScaffold(new DiceRandomizationService(new DeterministicDiceRandomProvider([10])));
        var context = CreateContext();
        var request = new RulesActionRequest
        {
            ActionType = Add1RulesModuleScaffold.ActionTypeAttack,
            ActorParticipantId = "player-1",
            Payload = new Add1AttackRequest
            {
                AttackerParticipantId = "player-1",
                AttackerPieceId = string.Empty,
                TargetPieceId = "piece-1",
                AttackType = "melee",
                ToHitThreshold = 10,
                DamageAmountOnHit = 5
            }
        };

        var validation = module.ValidateAction(context, request);

        Assert.IsFalse(validation.IsValid);
        Assert.AreEqual("Add1Attack attacker piece id is required.", validation.Message);
    }

    [TestMethod]
    public void ValidateAttackRequest_MissingAttackType_Fails()
    {
        var module = new Add1RulesModuleScaffold(new DiceRandomizationService(new DeterministicDiceRandomProvider([10])));
        var context = CreateContext();
        var request = new RulesActionRequest
        {
            ActionType = Add1RulesModuleScaffold.ActionTypeAttack,
            ActorParticipantId = "player-1",
            Payload = new Add1AttackRequest
            {
                AttackerParticipantId = "player-1",
                AttackerPieceId = "piece-attacker-1",
                TargetPieceId = "piece-1",
                AttackType = string.Empty,
                ToHitThreshold = 10,
                DamageAmountOnHit = 5
            }
        };

        var validation = module.ValidateAction(context, request);

        Assert.IsFalse(validation.IsValid);
        Assert.AreEqual("Add1Attack attack type is required.", validation.Message);
    }

    [TestMethod]
    public void DeferredMechanicAction_ResolvesAsDocumentedNoOpTag()
    {
        var module = new Add1RulesModuleScaffold(new DiceRandomizationService(new DeterministicDiceRandomProvider([1])));
        var context = CreateContext();
        var request = new RulesActionRequest
        {
            ActionType = "Add1SpellCast",
            ActorParticipantId = "player-1",
            Payload = new { Spell = "Magic Missile" }
        };

        var validation = module.ValidateAction(context, request);
        var resolution = module.ResolveAction(context, request);

        Assert.IsTrue(validation.IsValid);
        Assert.IsTrue(resolution.IsSuccess);
        Assert.AreEqual("NoOpAction", resolution.EngineActionRequest!.ActionType);
        StringAssert.Contains(string.Join(" | ", resolution.Diagnostics), Add1DeferredMechanicTag.SpellSubsystem.ToString());
    }

    [TestMethod]
    public void ResolveInitiative_ProducesOrderedParticipantList()
    {
        var module = new Add1RulesModuleScaffold(new DiceRandomizationService(new DeterministicDiceRandomProvider([4, 6, 2])));
        var context = CreateContext();
        var request = new RulesActionRequest
        {
            ActionType = Add1RulesModuleScaffold.ActionTypeInitiative,
            ActorParticipantId = "gm",
            Payload = new Add1InitiativeRequest
            {
                ParticipantIds = ["p1", "p2", "p3"],
                InitiativeMode = Add1InitiativeMode.Individual,
                TieBreakPolicy = Add1InitiativeTieBreakPolicy.StableOrder,
                RerollPolicy = Add1InitiativeRerollPolicy.EachRound,
                RoundNumber = 1,
                PhaseContext = "Combat"
            }
        };

        var resolution = module.ResolveAction(context, request);

        Assert.IsTrue(resolution.IsSuccess);
        var payload = resolution.EngineActionRequest!.Payload as Add1InitiativeResult;
        Assert.IsNotNull(payload);
        CollectionAssert.AreEqual(new[] { "p2", "p1", "p3" }, payload!.OrderedParticipantIds);
        Assert.AreEqual(Add1InitiativeMode.Individual, payload.InitiativeMode);
        Assert.IsTrue(payload.WasRerolledThisRound);
        Assert.AreEqual(1, payload.RoundNumber);
        Assert.AreEqual("Combat", payload.PhaseContext);
    }

    [TestMethod]
    public void ResolveInitiative_SideMode_ProducesSideOrderedParticipantList()
    {
        var module = new Add1RulesModuleScaffold(new DiceRandomizationService(new DeterministicDiceRandomProvider([4, 6])));
        var context = CreateContext();
        var request = new RulesActionRequest
        {
            ActionType = Add1RulesModuleScaffold.ActionTypeInitiative,
            ActorParticipantId = "gm",
            Payload = new Add1InitiativeRequest
            {
                ParticipantIds = ["p1", "p2", "p3", "p4"],
                InitiativeMode = Add1InitiativeMode.Side,
                SideByParticipantId = new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["p1"] = "A",
                    ["p2"] = "A",
                    ["p3"] = "B",
                    ["p4"] = "B"
                },
                TieBreakPolicy = Add1InitiativeTieBreakPolicy.StableOrder,
                RerollPolicy = Add1InitiativeRerollPolicy.EachRound,
                RoundNumber = 1,
                PhaseContext = "Combat"
            }
        };

        var resolution = module.ResolveAction(context, request);

        Assert.IsTrue(resolution.IsSuccess);
        var payload = resolution.EngineActionRequest!.Payload as Add1InitiativeResult;
        Assert.IsNotNull(payload);
        CollectionAssert.AreEqual(new[] { "p3", "p4", "p1", "p2" }, payload!.OrderedParticipantIds);
        Assert.AreEqual(Add1InitiativeMode.Side, payload.InitiativeMode);
        Assert.AreEqual(2, payload.InitiativeBySide.Count);
    }

    [TestMethod]
    public void ResolveInitiative_KeepOrderPolicy_ReusesPreviousRoundOrderWithoutReroll()
    {
        var module = new Add1RulesModuleScaffold(new DiceRandomizationService(new DeterministicDiceRandomProvider([6, 5, 4])));
        var context = CreateContext();

        var firstRoundRequest = new RulesActionRequest
        {
            ActionType = Add1RulesModuleScaffold.ActionTypeInitiative,
            ActorParticipantId = "gm",
            Payload = new Add1InitiativeRequest
            {
                ParticipantIds = ["p1", "p2", "p3"],
                InitiativeMode = Add1InitiativeMode.Individual,
                TieBreakPolicy = Add1InitiativeTieBreakPolicy.StableOrder,
                RerollPolicy = Add1InitiativeRerollPolicy.KeepOrderUntilCombatEnds,
                RoundNumber = 1,
                PhaseContext = "Combat"
            }
        };

        var firstRoundResolution = module.ResolveAction(context, firstRoundRequest);
        var firstRoundResult = (Add1InitiativeResult)firstRoundResolution.EngineActionRequest!.Payload!;

        var secondRoundRequest = new RulesActionRequest
        {
            ActionType = Add1RulesModuleScaffold.ActionTypeInitiative,
            ActorParticipantId = "gm",
            Payload = new Add1InitiativeRequest
            {
                ParticipantIds = ["p1", "p2", "p3"],
                InitiativeMode = Add1InitiativeMode.Individual,
                TieBreakPolicy = Add1InitiativeTieBreakPolicy.StableOrder,
                RerollPolicy = Add1InitiativeRerollPolicy.KeepOrderUntilCombatEnds,
                RoundNumber = 2,
                PhaseContext = "Combat",
                PreviousRoundResult = firstRoundResult
            }
        };

        var secondRoundResolution = module.ResolveAction(context, secondRoundRequest);
        var secondRoundResult = secondRoundResolution.EngineActionRequest!.Payload as Add1InitiativeResult;

        Assert.IsNotNull(secondRoundResult);
        CollectionAssert.AreEqual(firstRoundResult.OrderedParticipantIds, secondRoundResult!.OrderedParticipantIds);
        Assert.IsFalse(secondRoundResult.WasRerolledThisRound);
        Assert.AreEqual(2, secondRoundResult.RoundNumber);
    }

    [TestMethod]
    public void InitiativeTurnProjectionAdapter_MapsInitiativeToEngineTurnStructures()
    {
        var session = new VttSession
        {
            Id = "session-turn-projection",
            Title = "Turn Projection",
            TurnNumber = 3,
            TurnState = new TurnState()
        };

        var result = new Add1InitiativeResult
        {
            OrderedParticipantIds = ["p2", "p1", "p3"],
            InitiativeByParticipantId = new Dictionary<string, int>(StringComparer.Ordinal)
            {
                ["p1"] = 4,
                ["p2"] = 6,
                ["p3"] = 2
            },
            InitiativeBySide = new Dictionary<string, int>(StringComparer.Ordinal),
            InitiativeMode = Add1InitiativeMode.Individual,
            TieBreakPolicy = Add1InitiativeTieBreakPolicy.StableOrder,
            RerollPolicy = Add1InitiativeRerollPolicy.EachRound,
            RoundNumber = 5,
            PhaseContext = "Combat"
        };

        var adapter = new Add1InitiativeTurnProjectionAdapter();
        adapter.ApplyToTurnState(session, result);

        CollectionAssert.AreEqual(new[] { "p2", "p1", "p3" }, session.TurnOrder);
        Assert.AreEqual(0, session.CurrentTurnIndex);
        Assert.AreEqual("Combat", session.CurrentPhase);
        Assert.AreEqual("p2", session.TurnState.CurrentSeatId);
        Assert.AreEqual("Individual", session.TurnState.Metadata["InitiativeMode"]);
        Assert.AreEqual("StableOrder", session.TurnState.Metadata["TieBreakPolicy"]);
    }

    private static RulesContext CreateContext()
    {
        return new RulesContext
        {
            CurrentSession = new VttSession
            {
                Id = "session-add1",
                Title = "AD&D1 Rules Test",
                TurnNumber = 1,
                CurrentTurnIndex = 0,
                CurrentPhase = "Combat"
            },
            CurrentTurnNumber = 1,
            CurrentTurnIndex = 0,
            CurrentPhase = "Combat",
            ActorParticipantId = "player-1",
            SelectedRulesModuleId = "add1",
            SelectedRulesModuleVersion = "v1",
            ScenarioMetadata = "test-scenario",
            EnableStrictValidation = true
        };
    }
}
