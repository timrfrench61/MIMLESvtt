using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src;

namespace MIMLESvtt.Tests;

[TestClass]
public class SnapshotImportApplyExecutorTests
{
    [TestMethod]
    public void Execute_WithReplaceTableSessionIntent_ReplacesCurrentTableSessionAndReturnsAppliedResult()
    {
        var executor = new SnapshotImportApplyExecutor();
        var importedSession = new TableSession { Id = "imported-session", Title = "Imported" };
        var intent = new SnapshotImportApplyIntent(
            SnapshotFormatKind.TableSessionSnapshot,
            SnapshotImportApplyOperationKind.ReplaceTableSession,
            true,
            importedSession,
            "ReplaceTableSession intent");

        var context = new SnapshotImportApplyContext
        {
            CurrentTableSession = new TableSession { Id = "current-session", Title = "Current" }
        };

        var result = executor.Execute(intent, context);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(result.IsApplied);
        Assert.IsTrue(result.IsRuntimeStateMutated);
        Assert.AreEqual(SnapshotImportApplyOperationKind.ReplaceTableSession, result.OperationKind);
        Assert.AreSame(importedSession, result.ResultingTableSession);
        Assert.AreSame(importedSession, context.CurrentTableSession);
    }

    [TestMethod]
    public void Execute_WithReplaceTableSessionIntentAndMissingPayload_FailsClearly()
    {
        var executor = new SnapshotImportApplyExecutor();
        var intent = new SnapshotImportApplyIntent(
            SnapshotFormatKind.TableSessionSnapshot,
            SnapshotImportApplyOperationKind.ReplaceTableSession,
            true,
            null,
            "ReplaceTableSession intent");

        var context = new SnapshotImportApplyContext
        {
            CurrentTableSession = new TableSession { Id = "current-session" }
        };

        var exception = Assert.ThrowsException<InvalidOperationException>(() => executor.Execute(intent, context));
        StringAssert.Contains(exception.Message, "TableSession");
        Assert.AreEqual("current-session", context.CurrentTableSession!.Id);
    }

    [TestMethod]
    public void Execute_WithReplaceTableSessionIntentAndWrongPayloadType_FailsClearly()
    {
        var executor = new SnapshotImportApplyExecutor();
        var intent = new SnapshotImportApplyIntent(
            SnapshotFormatKind.TableSessionSnapshot,
            SnapshotImportApplyOperationKind.ReplaceTableSession,
            true,
            new ScenarioExport { Title = "Scenario" },
            "ReplaceTableSession intent");

        var context = new SnapshotImportApplyContext
        {
            CurrentTableSession = new TableSession { Id = "current-session" }
        };

        var exception = Assert.ThrowsException<InvalidOperationException>(() => executor.Execute(intent, context));
        StringAssert.Contains(exception.Message, "TableSession");
        Assert.AreEqual("current-session", context.CurrentTableSession!.Id);
    }

    [TestMethod]
    public void Execute_WithUnsupportedIntent_ReturnsNonAppliedResult()
    {
        var executor = new SnapshotImportApplyExecutor();
        var current = new TableSession { Id = "current-session" };
        var intent = new SnapshotImportApplyIntent(
            SnapshotFormatKind.ContentPackSnapshot,
            SnapshotImportApplyOperationKind.Unsupported,
            false,
            null,
            "Unsupported");

        var context = new SnapshotImportApplyContext
        {
            CurrentTableSession = current
        };

        var result = executor.Execute(intent, context);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsFalse(result.IsApplied);
        Assert.IsFalse(result.IsRuntimeStateMutated);
        Assert.AreEqual(SnapshotImportApplyOperationKind.Unsupported, result.OperationKind);
        Assert.AreSame(current, result.ResultingTableSession);
        Assert.AreSame(current, context.CurrentTableSession);
    }

    [TestMethod]
    public void Execute_WithCreateScenarioFromImportIntent_WhenAllowed_ReturnsPendingScenarioPlanAndDoesNotMutateRuntimeState()
    {
        var executor = new SnapshotImportApplyExecutor();
        var current = new TableSession { Id = "current-session", Title = "Current" };
        var importedScenario = new ScenarioExport
        {
            Title = "Imported Scenario",
            Surfaces =
            [
                new SurfaceInstance { Id = "surface-1", DefinitionId = "def-surface-1" },
                new SurfaceInstance { Id = "surface-2", DefinitionId = "def-surface-2" }
            ],
            Pieces =
            [
                new PieceInstance { Id = "piece-1", DefinitionId = "def-piece-1" },
                new PieceInstance { Id = "piece-2", DefinitionId = "def-piece-2" },
                new PieceInstance { Id = "piece-3", DefinitionId = "def-piece-3" }
            ]
        };

        var intent = new SnapshotImportApplyIntent(
            SnapshotFormatKind.ScenarioSnapshot,
            SnapshotImportApplyOperationKind.CreateScenarioFromImport,
            true,
            importedScenario,
            "CreateScenarioFromImport intent");

        var context = new SnapshotImportApplyContext
        {
            CurrentTableSession = current
        };

        var result = executor.Execute(intent, context);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(result.IsApplied);
        Assert.IsFalse(result.IsRuntimeStateMutated);
        Assert.AreEqual(SnapshotImportApplyOperationKind.CreateScenarioFromImport, result.OperationKind);
        Assert.AreSame(current, result.ResultingTableSession);
        Assert.AreSame(current, context.CurrentTableSession);
        Assert.IsNotNull(result.PendingScenarioPlan);
        Assert.AreEqual("Imported Scenario", result.PendingScenarioPlan!.ScenarioTitle);
        Assert.AreSame(importedScenario, result.PendingScenarioPlan.Scenario);
        Assert.AreEqual(SnapshotImportApplyOperationKind.CreateScenarioFromImport, result.PendingScenarioPlan.IntendedOperationKind);
        Assert.IsTrue(result.PendingScenarioPlan.IsReadyForApply);
        Assert.AreEqual(2, result.PendingScenarioPlan.SurfaceCount);
        Assert.AreEqual(3, result.PendingScenarioPlan.PieceCount);
        StringAssert.Contains(result.Message, "pending scenario application plan");
    }
}
