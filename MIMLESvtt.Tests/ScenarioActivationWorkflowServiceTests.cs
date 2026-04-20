using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src;

namespace MIMLESvtt.Tests;

[TestClass]
public class ScenarioActivationWorkflowServiceTests
{
    [TestMethod]
    public void ScenarioActivationWorkflow_WithScenarioJson_DryRun_ReturnsPlanCandidateAndNonMutatingSuccess()
    {
        var service = new ScenarioActivationWorkflowService();
        var serializer = new ScenarioSnapshotSerializer();
        var json = serializer.SerializeScenario(CreateScenarioFixture());

        var currentSession = new TableSession { Id = "current-session", Title = "Current" };
        var context = new SnapshotImportApplyContext { CurrentTableSession = currentSession };

        var response = service.Run(json, context, ScenarioCandidateActivationMode.DryRun, SnapshotImportApplyPolicy.Default);

        Assert.IsTrue(response.IsSuccess);
        Assert.IsTrue(response.IsSupported);
        Assert.IsTrue(response.PendingScenarioPlanCreated);
        Assert.IsTrue(response.CandidateCreated);
        Assert.IsFalse(response.IsRuntimeStateMutated);
        Assert.IsNotNull(response.PendingScenarioPlan);
        Assert.IsNotNull(response.TableSessionCandidate);
        Assert.AreEqual("Imported Scenario", response.PendingScenarioPlan!.ScenarioTitle);
        Assert.AreEqual("Imported Scenario", response.TableSessionCandidate!.Title);
        Assert.AreSame(currentSession, context.CurrentTableSession);
    }

    [TestMethod]
    public void ScenarioActivationWorkflow_WithScenarioJson_Activate_ReplacesRuntimeContextWhenPolicyAllows()
    {
        var service = new ScenarioActivationWorkflowService();
        var serializer = new ScenarioSnapshotSerializer();
        var json = serializer.SerializeScenario(CreateScenarioFixture());

        var context = new SnapshotImportApplyContext
        {
            CurrentTableSession = new TableSession { Id = "current-session", Title = "Current" }
        };

        var policy = new SnapshotImportApplyPolicy
        {
            AllowCreateScenarioFromImport = true,
            AllowActivateScenarioCandidate = true
        };

        var response = service.Run(json, context, ScenarioCandidateActivationMode.Activate, policy);

        Assert.IsTrue(response.IsSuccess);
        Assert.IsTrue(response.IsRuntimeStateMutated);
        Assert.IsTrue(response.PendingScenarioPlanCreated);
        Assert.IsTrue(response.CandidateCreated);
        Assert.IsNotNull(response.ResultingCurrentTableSession);
        Assert.AreSame(response.ResultingCurrentTableSession, context.CurrentTableSession);
        Assert.AreEqual("Imported Scenario", context.CurrentTableSession!.Title);
    }

    [TestMethod]
    public void ScenarioActivationWorkflow_WithScenarioJson_Activate_DoesNotMutateWhenPolicyDenies()
    {
        var service = new ScenarioActivationWorkflowService();
        var serializer = new ScenarioSnapshotSerializer();
        var json = serializer.SerializeScenario(CreateScenarioFixture());

        var currentSession = new TableSession { Id = "current-session", Title = "Current" };
        var context = new SnapshotImportApplyContext { CurrentTableSession = currentSession };

        var policy = new SnapshotImportApplyPolicy
        {
            AllowCreateScenarioFromImport = true,
            AllowActivateScenarioCandidate = false
        };

        var response = service.Run(json, context, ScenarioCandidateActivationMode.Activate, policy);

        Assert.IsFalse(response.IsSuccess);
        Assert.IsTrue(response.IsSupported);
        Assert.IsFalse(response.IsRuntimeStateMutated);
        Assert.IsTrue(response.PendingScenarioPlanCreated);
        Assert.IsTrue(response.CandidateCreated);
        Assert.AreSame(currentSession, context.CurrentTableSession);
        Assert.AreEqual(SnapshotImportErrorCode.ValidationFailure, response.ErrorCode);
    }

    [TestMethod]
    public void ScenarioActivationWorkflow_WithNonScenarioJson_ReturnsUnsupportedOrNonSuccessResponse()
    {
        var service = new ScenarioActivationWorkflowService();
        var serializer = new TableSessionSnapshotSerializer();

        var json = serializer.Save(new TableSession { Id = "session-1", Title = "Session" });
        var context = new SnapshotImportApplyContext { CurrentTableSession = new TableSession { Id = "current-session" } };

        var response = service.Run(json, context, ScenarioCandidateActivationMode.DryRun, SnapshotImportApplyPolicy.Default);

        Assert.IsFalse(response.IsSuccess);
        Assert.IsFalse(response.IsSupported);
        Assert.IsFalse(response.PendingScenarioPlanCreated);
        Assert.IsFalse(response.CandidateCreated);
        Assert.IsFalse(response.IsRuntimeStateMutated);
    }

    [TestMethod]
    public void ScenarioActivationWorkflow_WhenInputMalformed_ReturnsFailureResponse()
    {
        var service = new ScenarioActivationWorkflowService();
        var context = new SnapshotImportApplyContext { CurrentTableSession = new TableSession { Id = "current-session" } };

        var response = service.Run("{ \"Version\": 1, \"Scenario\":", context, ScenarioCandidateActivationMode.DryRun, SnapshotImportApplyPolicy.Default);

        Assert.IsFalse(response.IsSuccess);
        Assert.IsFalse(response.IsSupported);
        Assert.IsFalse(response.PendingScenarioPlanCreated);
        Assert.IsFalse(response.CandidateCreated);
        Assert.IsFalse(response.IsRuntimeStateMutated);
        Assert.AreEqual(SnapshotImportErrorCode.MalformedJson, response.ErrorCode);
        Assert.AreEqual(SnapshotImportFailureStage.Dispatch, response.FailureStage);
    }

    [TestMethod]
    public void ScenarioActivationWorkflow_WhenScenarioPathFailsCandidateCreation_ReturnsFailureResponse()
    {
        var service = new ScenarioActivationWorkflowService();
        var context = new SnapshotImportApplyContext { CurrentTableSession = new TableSession { Id = "current-session" } };

        var invalidScenarioJson = "{\"Version\":1,\"Scenario\":{\"Title\":\"Imported Scenario\",\"Surfaces\":[],\"Pieces\":[],\"Options\":null}}";

        var response = service.Run(invalidScenarioJson, context, ScenarioCandidateActivationMode.DryRun, SnapshotImportApplyPolicy.Default);

        Assert.IsFalse(response.IsSuccess);
        Assert.IsFalse(response.IsSupported);
        Assert.IsFalse(response.PendingScenarioPlanCreated);
        Assert.IsFalse(response.CandidateCreated);
        Assert.IsFalse(response.IsRuntimeStateMutated);
        Assert.IsFalse(string.IsNullOrWhiteSpace(response.ErrorMessage));
    }

    private static ScenarioExport CreateScenarioFixture()
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
