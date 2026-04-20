using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src;

namespace MIMLESvtt.Tests;

[TestClass]
public class ScenarioActivationEndToEndTests
{
    [TestMethod]
    public void EndToEnd_ScenarioJson_ToPendingPlan_ToCandidate_ToDryRunActivation_DoesNotMutateContext()
    {
        var applyWorkflow = new SnapshotImportApplyWorkflowService();
        var scenarioPlanApplyService = new ScenarioPlanApplyService();
        var activationService = new ScenarioCandidateActivationService();
        var scenarioSerializer = new ScenarioSnapshotSerializer();

        var json = scenarioSerializer.SerializeScenario(CreateImportedScenario());

        var context = new SnapshotImportApplyContext
        {
            CurrentTableSession = new TableSession { Id = "current-session", Title = "Current" }
        };

        var importApplyResponse = applyWorkflow.ImportAndApply(json, context);

        Assert.IsNotNull(importApplyResponse.PendingScenarioPlan);

        var candidateResult = scenarioPlanApplyService.Apply(new ScenarioPlanApplyRequest
        {
            PendingScenarioPlan = importApplyResponse.PendingScenarioPlan,
            ActiveRuntimeTableSession = context.CurrentTableSession
        });

        Assert.IsTrue(candidateResult.IsSuccess);
        Assert.IsNotNull(candidateResult.TableSessionCandidate);

        var activationResult = activationService.Activate(new ScenarioCandidateActivationRequest
        {
            TableSessionCandidate = candidateResult.TableSessionCandidate,
            TargetContext = context,
            Mode = ScenarioCandidateActivationMode.DryRun
        });

        Assert.IsTrue(activationResult.IsSuccess);
        Assert.IsFalse(activationResult.IsRuntimeStateMutated);
        Assert.AreEqual("current-session", context.CurrentTableSession!.Id);
    }

    [TestMethod]
    public void EndToEnd_ScenarioJson_ToPendingPlan_ToCandidate_ToActivate_ReplacesRuntimeContextWhenPolicyAllows()
    {
        var applyWorkflow = new SnapshotImportApplyWorkflowService();
        var scenarioPlanApplyService = new ScenarioPlanApplyService();
        var activationService = new ScenarioCandidateActivationService();
        var scenarioSerializer = new ScenarioSnapshotSerializer();

        var json = scenarioSerializer.SerializeScenario(CreateImportedScenario());

        var context = new SnapshotImportApplyContext
        {
            CurrentTableSession = new TableSession { Id = "current-session", Title = "Current" }
        };

        var importApplyResponse = applyWorkflow.ImportAndApply(json, context);

        Assert.IsNotNull(importApplyResponse.PendingScenarioPlan);

        var candidateResult = scenarioPlanApplyService.Apply(new ScenarioPlanApplyRequest
        {
            PendingScenarioPlan = importApplyResponse.PendingScenarioPlan,
            ActiveRuntimeTableSession = context.CurrentTableSession
        });

        Assert.IsTrue(candidateResult.IsSuccess);
        Assert.IsNotNull(candidateResult.TableSessionCandidate);

        var policy = new SnapshotImportApplyPolicy
        {
            AllowActivateScenarioCandidate = true
        };

        var activationResult = activationService.Activate(new ScenarioCandidateActivationRequest
        {
            TableSessionCandidate = candidateResult.TableSessionCandidate,
            TargetContext = context,
            Mode = ScenarioCandidateActivationMode.Activate
        }, policy);

        Assert.IsTrue(activationResult.IsSuccess);
        Assert.IsTrue(activationResult.IsRuntimeStateMutated);
        Assert.AreEqual(candidateResult.TableSessionCandidate!.Id, context.CurrentTableSession!.Id);
        Assert.AreEqual("Imported Scenario", context.CurrentTableSession.Title);
    }

    private static ScenarioExport CreateImportedScenario()
    {
        return new ScenarioExport
        {
            Title = "Imported Scenario",
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
                        Coordinate = new Coordinate { X = 1, Y = 1 }
                    }
                }
            ],
            Options = new TableOptions
            {
                EnableFog = true,
                EnableTurnTracker = false
            }
        };
    }
}
