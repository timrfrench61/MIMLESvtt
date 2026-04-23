using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Rules;
using MIMLESvtt.src.Domain.Rules.Brp;

namespace MIMLESvtt.Tests;

[TestClass]
public class BrpRulesModuleScaffoldTests
{
    [TestMethod]
    public void ValidatePercentileCheck_MissingSkillId_Fails()
    {
        var module = CreateModule([50]);
        var context = CreateContext();
        var request = new RulesActionRequest
        {
            ActionType = BrpRulesModuleScaffold.ActionTypePercentileCheck,
            ActorParticipantId = "investigator-1",
            Payload = new BrpPercentileCheckRequest
            {
                ActorParticipantId = "investigator-1",
                SkillOrAttributeId = string.Empty,
                TargetValue = 45,
                Modifier = 0
            }
        };

        var validation = module.ValidateAction(context, request);

        Assert.IsFalse(validation.IsValid);
        Assert.AreEqual("BrpPercentileCheck skill/attribute id is required.", validation.Message);
    }

    [TestMethod]
    public void ResolveSkillResolution_ReturnsStructuredPercentileResult()
    {
        var module = CreateModule([37]);
        var context = CreateContext();
        var request = new RulesActionRequest
        {
            ActionType = BrpRulesModuleScaffold.ActionTypeSkillResolution,
            ActorParticipantId = "investigator-1",
            Payload = new BrpSkillResolutionRequest
            {
                ActorParticipantId = "investigator-1",
                SkillId = "Stealth",
                TargetValue = 45,
                Modifier = 5,
                ContextTag = "brp:test:skill"
            }
        };

        var resolution = module.ResolveAction(context, request);

        Assert.IsTrue(resolution.IsSuccess);
        var payload = resolution.EngineActionRequest!.Payload as BrpPercentileCheckResult;
        Assert.IsNotNull(payload);
        Assert.IsTrue(payload!.IsSuccess);
        Assert.IsTrue(payload.Passed);
        Assert.AreEqual(37, payload.RolledValue);
        Assert.AreEqual(50, payload.EffectiveTargetValue);
        Assert.AreEqual("brp:test:skill", payload.RollDetails.ContextTag);
        StringAssert.Contains(payload.Summary, "passed");
    }

    [TestMethod]
    public void ResolvePercentileCheck_WithHighRoll_FailsInterpretationCorrectly()
    {
        var module = CreateModule([90]);
        var context = CreateContext();
        var request = new RulesActionRequest
        {
            ActionType = BrpRulesModuleScaffold.ActionTypePercentileCheck,
            ActorParticipantId = "investigator-1",
            Payload = new BrpPercentileCheckRequest
            {
                ActorParticipantId = "investigator-1",
                SkillOrAttributeId = "Listen",
                TargetValue = 40,
                Modifier = 0
            }
        };

        var resolution = module.ResolveAction(context, request);
        var payload = resolution.EngineActionRequest!.Payload as BrpPercentileCheckResult;

        Assert.IsNotNull(payload);
        Assert.IsFalse(payload!.Passed);
        Assert.AreEqual(90, payload.RolledValue);
        Assert.AreEqual(40, payload.EffectiveTargetValue);
        StringAssert.Contains(payload.Summary, "failed");
    }

    [TestMethod]
    public void ResolvePercentileCheck_WithModifier_UpdatesEffectiveTarget()
    {
        var module = CreateModule([61]);
        var context = CreateContext();
        var request = new RulesActionRequest
        {
            ActionType = BrpRulesModuleScaffold.ActionTypePercentileCheck,
            ActorParticipantId = "investigator-1",
            Payload = new BrpPercentileCheckRequest
            {
                ActorParticipantId = "investigator-1",
                SkillOrAttributeId = "Persuade",
                TargetValue = 55,
                Modifier = 10
            }
        };

        var resolution = module.ResolveAction(context, request);
        var payload = resolution.EngineActionRequest!.Payload as BrpPercentileCheckResult;

        Assert.IsNotNull(payload);
        Assert.AreEqual(10, payload!.AppliedModifier);
        Assert.AreEqual(65, payload.EffectiveTargetValue);
        Assert.IsTrue(payload.Passed);
    }

    [TestMethod]
    public void ResolveOpposedCheck_Tie_UsesDeterministicTieBreakPolicy()
    {
        var module = CreateModule([50, 50]);
        var context = CreateContext();
        var request = new RulesActionRequest
        {
            ActionType = BrpRulesModuleScaffold.ActionTypeOpposedCheck,
            ActorParticipantId = "investigator-1",
            Payload = new BrpOpposedCheckRequest
            {
                AttackerParticipantId = "investigator-2",
                AttackerTargetValue = 50,
                AttackerModifier = 0,
                DefenderParticipantId = "investigator-1",
                DefenderTargetValue = 50,
                DefenderModifier = 0
            }
        };

        var resolution = module.ResolveAction(context, request);

        Assert.IsTrue(resolution.IsSuccess);
        var payload = resolution.EngineActionRequest!.Payload as BrpOpposedCheckResult;
        Assert.IsNotNull(payload);
        Assert.IsTrue(payload!.WasTie);
        Assert.AreEqual("DeterministicParticipantIdOrder", payload.TieBreakPolicy);
        Assert.AreEqual("investigator-1", payload.WinnerParticipantId);
    }

    [TestMethod]
    public void DeferredMechanicAction_ResolvesAsNoOpDeferredTag()
    {
        var module = CreateModule([1]);
        var context = CreateContext();
        var request = new RulesActionRequest
        {
            ActionType = "BrpHitLocation",
            ActorParticipantId = "investigator-1",
            Payload = new { Segment = "Head" }
        };

        var validation = module.ValidateAction(context, request);
        var resolution = module.ResolveAction(context, request);

        Assert.IsTrue(validation.IsValid);
        Assert.IsTrue(resolution.IsSuccess);
        Assert.AreEqual("NoOpAction", resolution.EngineActionRequest!.ActionType);
        StringAssert.Contains(string.Join(" | ", resolution.Diagnostics), BrpDeferredMechanicTag.HitLocationWoundSeverity.ToString());
    }

    [TestMethod]
    public void ValidateOpposedCheck_InvalidDefenderTarget_Fails()
    {
        var module = CreateModule([1]);
        var context = CreateContext();
        var request = new RulesActionRequest
        {
            ActionType = BrpRulesModuleScaffold.ActionTypeOpposedCheck,
            ActorParticipantId = "investigator-1",
            Payload = new BrpOpposedCheckRequest
            {
                AttackerParticipantId = "investigator-1",
                AttackerTargetValue = 50,
                DefenderParticipantId = "investigator-2",
                DefenderTargetValue = 0
            }
        };

        var validation = module.ValidateAction(context, request);

        Assert.IsFalse(validation.IsValid);
        Assert.AreEqual("BrpOpposedCheck defender target value must be in range 1-100.", validation.Message);
    }

    [TestMethod]
    public void BrpModule_UsesSharedDiceServiceComposition()
    {
        var provider = new DeterministicDiceRandomProvider([20]);
        IDiceRandomizationService sharedDice = new DiceRandomizationService(provider);
        var module = new BrpRulesModuleScaffold(sharedDice, new BrpOpposedCheckService());

        var context = CreateContext();
        var request = new RulesActionRequest
        {
            ActionType = BrpRulesModuleScaffold.ActionTypePercentileCheck,
            ActorParticipantId = "investigator-1",
            Payload = new BrpPercentileCheckRequest
            {
                ActorParticipantId = "investigator-1",
                SkillOrAttributeId = "Listen",
                TargetValue = 40,
                Modifier = 0
            }
        };

        var resolution = module.ResolveAction(context, request);
        var payload = resolution.EngineActionRequest!.Payload as BrpPercentileCheckResult;

        Assert.IsNotNull(payload);
        Assert.AreEqual(20, payload!.RolledValue);
        Assert.AreEqual("Deterministic", payload.RollDetails.RandomProvider);
    }

    private static BrpRulesModuleScaffold CreateModule(IEnumerable<int> deterministicRolls)
    {
        var dice = new DiceRandomizationService(new DeterministicDiceRandomProvider(deterministicRolls));
        return new BrpRulesModuleScaffold(dice, new BrpOpposedCheckService());
    }

    private static RulesContext CreateContext()
    {
        return new RulesContext
        {
            CurrentSession = new VttSession
            {
                Id = "session-brp",
                Title = "BRP Rules Test",
                TurnNumber = 1,
                CurrentTurnIndex = 0,
                CurrentPhase = "Investigation"
            },
            CurrentTurnNumber = 1,
            CurrentTurnIndex = 0,
            CurrentPhase = "Investigation",
            ActorParticipantId = "investigator-1",
            SelectedRulesModuleId = "brp",
            SelectedRulesModuleVersion = "v1",
            ScenarioMetadata = "test-scenario",
            EnableStrictValidation = true
        };
    }
}
