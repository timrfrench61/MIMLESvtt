using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src.Domain.Rules;
using MIMLESvtt.src.Domain.Rules.Add1;
using MIMLESvtt.src.Domain.Rules.Brp;
using MIMLESvtt.Tests.Harness;

namespace MIMLESvtt.Tests;

[TestClass]
public class RulesModuleHarnessRegressionTests
{
    [TestMethod]
    public void Add1Regression_AttackResolution_UsesHarnessFixtureDriverAndAssertions()
    {
        var fixture = new RulesTestFixtureBuilder()
            .WithSurface("surface-1")
            .WithParticipant("player-1", "Player One")
            .WithPiece("piece-attacker-1", "player-1", "surface-1")
            .WithPiece("piece-target-1", "", "surface-1")
            .WithTurnPhase("Combat", roundNumber: 1, turnNumber: 1)
            .WithObjectiveMetadata("objective:capture", "in-progress");

        var context = fixture.BuildContext("player-1", "add1", "v1", "harness-regression");
        var module = new Add1RulesModuleScaffold(RulesDeterministicRandomSupport.CreateDice(16));
        var request = new RulesActionRequest
        {
            ActionType = Add1RulesModuleScaffold.ActionTypeAttack,
            ActorParticipantId = "player-1",
            Payload = new Add1AttackRequest
            {
                AttackerParticipantId = "player-1",
                AttackerPieceId = "piece-attacker-1",
                TargetPieceId = "piece-target-1",
                AttackType = "melee",
                AttackModifier = 2,
                ToHitThreshold = 12,
                DamageAmountOnHit = 6
            }
        };

        var driver = new RulesRequestDriver();
        var validation = driver.Validate(module, context, request);
        RulesAssertionHelpers.AssertValid(validation);

        var resolution = driver.Resolve(module, context, request);
        var payload = RulesAssertionHelpers.AssertOutcomePayload<Add1AttackResult>(resolution);
        RulesAssertionHelpers.AssertAdd1AttackHit(payload, expectedRoll: 18);
    }

    [TestMethod]
    public void BrpRegression_PercentileResolution_UsesHarnessFixtureDriverAndAssertions()
    {
        var fixture = new RulesTestFixtureBuilder()
            .WithSurface("surface-1")
            .WithParticipant("investigator-1", "Investigator One")
            .WithTurnPhase("Investigation", roundNumber: 2, turnNumber: 3)
            .WithObjectiveMetadata("objective:clue", "collected");

        var context = fixture.BuildContext("investigator-1", "brp", "v1", "harness-regression");
        var module = new BrpRulesModuleScaffold(RulesDeterministicRandomSupport.CreateDice(42), new BrpOpposedCheckService());
        var request = new RulesActionRequest
        {
            ActionType = BrpRulesModuleScaffold.ActionTypePercentileCheck,
            ActorParticipantId = "investigator-1",
            Payload = new BrpPercentileCheckRequest
            {
                ActorParticipantId = "investigator-1",
                SkillOrAttributeId = "Search",
                TargetValue = 45,
                Modifier = 5
            }
        };

        var driver = new RulesRequestDriver();
        var validation = driver.Validate(module, context, request);
        RulesAssertionHelpers.AssertValid(validation);

        var resolution = driver.Resolve(module, context, request);
        var payload = RulesAssertionHelpers.AssertOutcomePayload<BrpPercentileCheckResult>(resolution);
        RulesAssertionHelpers.AssertBrpPercentileResult(payload, expectedPassed: true);
    }
}
