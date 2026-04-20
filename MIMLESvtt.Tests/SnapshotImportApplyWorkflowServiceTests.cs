using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src;

namespace MIMLESvtt.Tests;

[TestClass]
public class SnapshotImportApplyWorkflowServiceTests
{
    [TestMethod]
    public void ApplyWorkflow_WithReplaceTableSessionDeniedByPolicy_ReturnsNonAppliedResponseAndDoesNotMutateContext()
    {
        var service = new SnapshotImportApplyWorkflowService();
        var serializer = new TableSessionSnapshotSerializer();

        var importedSession = CreateImportedTableSession();
        var json = serializer.Save(importedSession);

        var currentSession = new TableSession { Id = "current-session", Title = "Current" };
        var context = new SnapshotImportApplyContext
        {
            CurrentTableSession = currentSession
        };

        var policy = new SnapshotImportApplyPolicy
        {
            AllowReplaceTableSession = false,
            AllowCreateScenarioFromImport = true
        };

        var response = service.ImportAndApply(json, context, policy);

        Assert.IsTrue(response.IsSuccess);
        Assert.IsTrue(response.IsSupported);
        Assert.IsFalse(response.IsApplied);
        Assert.IsFalse(response.IsRuntimeStateMutated);
        Assert.AreEqual(SnapshotFormatKind.TableSessionSnapshot, response.FormatKind);
        Assert.AreEqual(SnapshotImportApplyOperationKind.ReplaceTableSession, response.OperationKind);
        Assert.AreSame(currentSession, context.CurrentTableSession);
        Assert.AreSame(currentSession, response.ResultingTableSession);
        StringAssert.Contains(response.Message!, "denied by current apply policy");
    }

    [TestMethod]
    public void ApplyWorkflow_WithCreateScenarioDeniedByPolicy_ReturnsNonAppliedResponse()
    {
        var service = new SnapshotImportApplyWorkflowService();
        var serializer = new ScenarioSnapshotSerializer();

        var json = serializer.SerializeScenario(CreateImportedScenario());

        var currentSession = new TableSession { Id = "current-session", Title = "Current" };
        var context = new SnapshotImportApplyContext
        {
            CurrentTableSession = currentSession
        };

        var policy = new SnapshotImportApplyPolicy
        {
            AllowReplaceTableSession = true,
            AllowCreateScenarioFromImport = false
        };

        var response = service.ImportAndApply(json, context, policy);

        Assert.IsTrue(response.IsSuccess);
        Assert.IsTrue(response.IsSupported);
        Assert.IsFalse(response.IsApplied);
        Assert.IsFalse(response.IsRuntimeStateMutated);
        Assert.AreEqual(SnapshotFormatKind.ScenarioSnapshot, response.FormatKind);
        Assert.AreEqual(SnapshotImportApplyOperationKind.CreateScenarioFromImport, response.OperationKind);
        Assert.AreSame(currentSession, context.CurrentTableSession);
        Assert.AreSame(currentSession, response.ResultingTableSession);
        Assert.IsNull(response.PendingScenarioPlan);
        StringAssert.Contains(response.Message!, "denied by current apply policy");
    }

    [TestMethod]
    public void ApplyWorkflow_WithTableSessionSnapshotJson_ReturnsSuccessAndReplacesCurrentTableSession()
    {
        var service = new SnapshotImportApplyWorkflowService();
        var serializer = new TableSessionSnapshotSerializer();

        var importedSession = CreateImportedTableSession();
        var json = serializer.Save(importedSession);

        var context = new SnapshotImportApplyContext
        {
            CurrentTableSession = new TableSession { Id = "current-session", Title = "Current" }
        };

        var response = service.ImportAndApply(json, context);

        Assert.IsTrue(response.IsSuccess);
        Assert.IsTrue(response.IsSupported);
        Assert.IsTrue(response.IsApplied);
        Assert.IsTrue(response.IsRuntimeStateMutated);
        Assert.AreEqual(SnapshotFormatKind.TableSessionSnapshot, response.FormatKind);
        Assert.AreEqual(SnapshotImportApplyOperationKind.ReplaceTableSession, response.OperationKind);
        Assert.IsNotNull(response.ResultingTableSession);
        Assert.AreEqual("imported-session", response.ResultingTableSession!.Id);
        Assert.AreEqual("imported-session", context.CurrentTableSession!.Id);
        Assert.IsFalse(string.IsNullOrWhiteSpace(response.RequestId));
        Assert.AreEqual(SnapshotImportErrorCode.None, response.ErrorCode);
        Assert.AreEqual(SnapshotImportFailureStage.None, response.FailureStage);
    }

    [TestMethod]
    public void ApplyWorkflow_WithDefaultPolicy_PreservesCurrentReplaceTableSessionBehavior()
    {
        var service = new SnapshotImportApplyWorkflowService();
        var serializer = new TableSessionSnapshotSerializer();
        var json = serializer.Save(CreateImportedTableSession());

        var context = new SnapshotImportApplyContext
        {
            CurrentTableSession = new TableSession { Id = "current-session", Title = "Current" }
        };

        var response = service.ImportAndApply(json, context, SnapshotImportApplyPolicy.Default);

        Assert.IsTrue(response.IsSuccess);
        Assert.IsTrue(response.IsApplied);
        Assert.IsTrue(response.IsRuntimeStateMutated);
        Assert.AreEqual("imported-session", context.CurrentTableSession!.Id);
    }

    [TestMethod]
    public void ApplyWorkflow_WithDefaultPolicy_PreservesCurrentScenarioBehavior()
    {
        var service = new SnapshotImportApplyWorkflowService();
        var serializer = new ScenarioSnapshotSerializer();
        var json = serializer.SerializeScenario(CreateImportedScenario());

        var currentSession = new TableSession { Id = "current-session", Title = "Current" };
        var context = new SnapshotImportApplyContext
        {
            CurrentTableSession = currentSession
        };

        var response = service.ImportAndApply(json, context, SnapshotImportApplyPolicy.Default);

        Assert.IsTrue(response.IsSuccess);
        Assert.IsFalse(response.IsApplied);
        Assert.IsFalse(response.IsRuntimeStateMutated);
        Assert.AreEqual(SnapshotImportApplyOperationKind.CreateScenarioFromImport, response.OperationKind);
        Assert.AreSame(currentSession, context.CurrentTableSession);
    }

    [TestMethod]
    public void ApplyWorkflow_WithScenarioSnapshotJson_ReturnsSuccessButDoesNotMutateRuntimeState()
    {
        var service = new SnapshotImportApplyWorkflowService();
        var serializer = new ScenarioSnapshotSerializer();

        var json = serializer.SerializeScenario(CreateImportedScenario());

        var currentSession = new TableSession { Id = "current-session", Title = "Current" };
        var context = new SnapshotImportApplyContext
        {
            CurrentTableSession = currentSession
        };

        var response = service.ImportAndApply(json, context);

        Assert.IsTrue(response.IsSuccess);
        Assert.IsTrue(response.IsSupported);
        Assert.IsFalse(response.IsApplied);
        Assert.IsFalse(response.IsRuntimeStateMutated);
        Assert.AreEqual(SnapshotFormatKind.ScenarioSnapshot, response.FormatKind);
        Assert.AreEqual(SnapshotImportApplyOperationKind.CreateScenarioFromImport, response.OperationKind);
        Assert.AreSame(currentSession, response.ResultingTableSession);
        Assert.AreSame(currentSession, context.CurrentTableSession);
        Assert.AreEqual("Imported Scenario", response.PendingScenarioTitle);
        Assert.IsNotNull(response.PendingScenarioPlan);
        Assert.AreEqual("Imported Scenario", response.PendingScenarioPlan!.ScenarioTitle);
        Assert.AreEqual(1, response.PendingScenarioPlan.SurfaceCount);
        Assert.AreEqual(1, response.PendingScenarioPlan.PieceCount);
        Assert.IsTrue(response.PendingScenarioPlan.IsReadyForApply);
        Assert.IsFalse(string.IsNullOrWhiteSpace(response.RequestId));
        Assert.AreEqual(SnapshotImportErrorCode.None, response.ErrorCode);
        Assert.AreEqual(SnapshotImportFailureStage.None, response.FailureStage);
    }

    [TestMethod]
    public void ApplyWorkflow_WithScenarioSnapshotJson_WhenAllowed_ReturnsSuccessWithPendingScenarioPlan()
    {
        var service = new SnapshotImportApplyWorkflowService();
        var serializer = new ScenarioSnapshotSerializer();
        var json = serializer.SerializeScenario(CreateImportedScenario());

        var context = new SnapshotImportApplyContext
        {
            CurrentTableSession = new TableSession { Id = "current-session", Title = "Current" }
        };

        var policy = new SnapshotImportApplyPolicy
        {
            AllowReplaceTableSession = true,
            AllowCreateScenarioFromImport = true
        };

        var response = service.ImportAndApply(json, context, policy);

        Assert.IsTrue(response.IsSuccess);
        Assert.IsTrue(response.IsSupported);
        Assert.IsFalse(response.IsApplied);
        Assert.IsFalse(response.IsRuntimeStateMutated);
        Assert.IsNotNull(response.PendingScenarioPlan);
        Assert.AreEqual("Imported Scenario", response.PendingScenarioPlan!.ScenarioTitle);
        Assert.AreEqual(1, response.PendingScenarioPlan.SurfaceCount);
        Assert.AreEqual(1, response.PendingScenarioPlan.PieceCount);
        Assert.AreEqual(SnapshotImportApplyOperationKind.CreateScenarioFromImport, response.PendingScenarioPlan.IntendedOperationKind);
    }

    [TestMethod]
    public void ApplyWorkflow_WithScenarioSnapshotJson_WhenAllowed_DoesNotMutateCurrentTableSession()
    {
        var service = new SnapshotImportApplyWorkflowService();
        var serializer = new ScenarioSnapshotSerializer();
        var json = serializer.SerializeScenario(CreateImportedScenario());

        var currentSession = new TableSession { Id = "current-session", Title = "Current" };
        var context = new SnapshotImportApplyContext
        {
            CurrentTableSession = currentSession
        };

        var policy = new SnapshotImportApplyPolicy
        {
            AllowReplaceTableSession = true,
            AllowCreateScenarioFromImport = true
        };

        var response = service.ImportAndApply(json, context, policy);

        Assert.IsFalse(response.IsApplied);
        Assert.IsFalse(response.IsRuntimeStateMutated);
        Assert.AreSame(currentSession, context.CurrentTableSession);
        Assert.AreSame(currentSession, response.ResultingTableSession);
        Assert.IsNotNull(response.PendingScenarioPlan);
    }

    [TestMethod]
    public void ApplyWorkflow_WithContentPackSnapshotJson_ReturnsNonAppliedUnsupportedResponse()
    {
        var service = new SnapshotImportApplyWorkflowService();
        var serializer = new ContentPackSnapshotSerializer();

        var json = serializer.SerializeSnapshot(new ContentPackSnapshot
        {
            Version = ContentPackSnapshotSerializer.CurrentVersion,
            Manifest = new ContentPackManifest { Name = "Pack", Description = "Desc" },
            Definitions = [],
            Assets = []
        });

        var context = new SnapshotImportApplyContext
        {
            CurrentTableSession = new TableSession { Id = "current-session" }
        };

        var response = service.ImportAndApply(json, context);

        Assert.IsTrue(response.IsSuccess);
        Assert.IsFalse(response.IsSupported);
        Assert.IsFalse(response.IsApplied);
        Assert.IsFalse(response.IsRuntimeStateMutated);
        Assert.AreEqual(SnapshotFormatKind.ContentPackSnapshot, response.FormatKind);
        Assert.AreEqual(SnapshotImportApplyOperationKind.Unsupported, response.OperationKind);
        Assert.AreSame(context.CurrentTableSession, response.ResultingTableSession);
        Assert.IsFalse(string.IsNullOrWhiteSpace(response.RequestId));
    }

    [TestMethod]
    public void ApplyWorkflow_WithActionLogSnapshotJson_ReturnsNonAppliedUnsupportedResponse()
    {
        var service = new SnapshotImportApplyWorkflowService();
        var serializer = new ActionLogSnapshotSerializer();

        var json = serializer.SerializeSnapshot(new ActionLogSnapshot
        {
            Version = ActionLogSnapshotSerializer.CurrentVersion,
            SessionId = "session-1",
            Actions = []
        });

        var context = new SnapshotImportApplyContext
        {
            CurrentTableSession = new TableSession { Id = "current-session" }
        };

        var response = service.ImportAndApply(json, context);

        Assert.IsTrue(response.IsSuccess);
        Assert.IsFalse(response.IsSupported);
        Assert.IsFalse(response.IsApplied);
        Assert.IsFalse(response.IsRuntimeStateMutated);
        Assert.AreEqual(SnapshotFormatKind.ActionLogSnapshot, response.FormatKind);
        Assert.AreEqual(SnapshotImportApplyOperationKind.Unsupported, response.OperationKind);
        Assert.AreSame(context.CurrentTableSession, response.ResultingTableSession);
        Assert.IsFalse(string.IsNullOrWhiteSpace(response.RequestId));
    }

    [TestMethod]
    public void ApplyWorkflow_WhenInputMalformed_ReturnsFailureResponse()
    {
        var service = new SnapshotImportApplyWorkflowService();
        var context = new SnapshotImportApplyContext();

        var response = service.ImportAndApply("{ \"Version\": 1, \"Scenario\":", context);

        Assert.IsFalse(response.IsSuccess);
        Assert.IsFalse(response.IsSupported);
        Assert.IsFalse(response.IsApplied);
        Assert.IsFalse(response.IsRuntimeStateMutated);
        Assert.IsNull(response.FormatKind);
        Assert.IsNull(response.OperationKind);
        Assert.IsNull(response.ResultingTableSession);
        Assert.IsFalse(string.IsNullOrWhiteSpace(response.ErrorMessage));
        Assert.AreEqual(SnapshotImportErrorCode.MalformedJson, response.ErrorCode);
        Assert.AreEqual(SnapshotImportFailureStage.Dispatch, response.FailureStage);
        Assert.IsFalse(string.IsNullOrWhiteSpace(response.RequestId));
    }

    [TestMethod]
    public void ApplyWorkflow_WhenFormatUnknown_ReturnsFailureResponse()
    {
        var service = new SnapshotImportApplyWorkflowService();
        var context = new SnapshotImportApplyContext();

        var response = service.ImportAndApply("{\"Version\":1,\"Unknown\":{}}", context);

        Assert.IsFalse(response.IsSuccess);
        Assert.IsFalse(response.IsSupported);
        Assert.IsFalse(response.IsApplied);
        Assert.IsFalse(response.IsRuntimeStateMutated);
        Assert.IsNull(response.FormatKind);
        Assert.IsNull(response.OperationKind);
        Assert.IsNull(response.ResultingTableSession);
        Assert.IsFalse(string.IsNullOrWhiteSpace(response.ErrorMessage));
        Assert.AreEqual(SnapshotImportErrorCode.UnknownFormat, response.ErrorCode);
        Assert.AreEqual(SnapshotImportFailureStage.Dispatch, response.FailureStage);
        Assert.IsFalse(string.IsNullOrWhiteSpace(response.RequestId));
    }

    [TestMethod]
    public void ApplyWorkflow_WhenKnownFormatFailsValidation_ReturnsFailureResponse()
    {
        var service = new SnapshotImportApplyWorkflowService();
        var context = new SnapshotImportApplyContext();

        var response = service.ImportAndApply("{\"Version\":1,\"Scenario\":null}", context);

        Assert.IsFalse(response.IsSuccess);
        Assert.IsFalse(response.IsSupported);
        Assert.IsFalse(response.IsApplied);
        Assert.IsFalse(response.IsRuntimeStateMutated);
        Assert.IsNull(response.FormatKind);
        Assert.IsNull(response.OperationKind);
        Assert.IsNull(response.ResultingTableSession);
        Assert.IsFalse(string.IsNullOrWhiteSpace(response.ErrorMessage));
        Assert.AreEqual(SnapshotImportErrorCode.ValidationFailure, response.ErrorCode);
        Assert.AreEqual(SnapshotImportFailureStage.FormatValidation, response.FailureStage);
        Assert.IsFalse(string.IsNullOrWhiteSpace(response.RequestId));
    }

    private static TableSession CreateImportedTableSession()
    {
        return new TableSession
        {
            Id = "imported-session",
            Title = "Imported Session",
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
                        Coordinate = new Coordinate { X = 5, Y = 5 }
                    }
                }
            ]
        };
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
            ]
        };
    }
}
