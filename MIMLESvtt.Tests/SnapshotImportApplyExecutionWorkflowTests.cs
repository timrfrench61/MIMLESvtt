using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src;

namespace MIMLESvtt.Tests;

[TestClass]
public class SnapshotImportApplyExecutionWorkflowTests
{
    [TestMethod]
    public void EndToEnd_WithTableSessionSnapshotJson_ReplacesCurrentTableSession()
    {
        var workflow = new SnapshotImportWorkflowService();
        var intentService = new SnapshotImportIntentService();
        var executor = new SnapshotImportApplyExecutor();
        var serializer = new TableSessionSnapshotSerializer();

        var importedSession = CreateImportedTableSession();
        var json = serializer.Save(importedSession);

        var outcome = workflow.Import(json);
        var intent = intentService.CreateIntent(outcome);

        var context = new SnapshotImportApplyContext
        {
            CurrentTableSession = new TableSession { Id = "current-session", Title = "Current" }
        };

        var result = executor.Execute(intent, context);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(result.IsApplied);
        Assert.IsTrue(result.IsRuntimeStateMutated);
        Assert.AreEqual(SnapshotImportApplyOperationKind.ReplaceTableSession, result.OperationKind);
        Assert.IsNotNull(context.CurrentTableSession);
        Assert.AreEqual("imported-session", context.CurrentTableSession!.Id);
        Assert.AreEqual("Imported Session", context.CurrentTableSession.Title);
    }

    [TestMethod]
    public void EndToEnd_WithScenarioSnapshotJson_DoesNotMutateRuntimeStateAndReturnsPendingScenarioPlanResult()
    {
        var workflow = new SnapshotImportWorkflowService();
        var intentService = new SnapshotImportIntentService();
        var executor = new SnapshotImportApplyExecutor();
        var serializer = new ScenarioSnapshotSerializer();

        var json = serializer.SerializeScenario(CreateImportedScenario());

        var outcome = workflow.Import(json);
        var intent = intentService.CreateIntent(outcome);

        var currentSession = new TableSession { Id = "current-session", Title = "Current" };
        var context = new SnapshotImportApplyContext
        {
            CurrentTableSession = currentSession
        };

        var result = executor.Execute(intent, context);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(result.IsApplied);
        Assert.IsFalse(result.IsRuntimeStateMutated);
        Assert.AreEqual(SnapshotImportApplyOperationKind.CreateScenarioFromImport, result.OperationKind);
        Assert.AreSame(currentSession, context.CurrentTableSession);
        Assert.AreSame(currentSession, result.ResultingTableSession);
        Assert.IsNotNull(result.PendingScenarioPlan);
        Assert.AreEqual("Imported Scenario", result.PendingScenarioPlan!.ScenarioTitle);
        Assert.AreEqual(1, result.PendingScenarioPlan.SurfaceCount);
        Assert.AreEqual(1, result.PendingScenarioPlan.PieceCount);
        StringAssert.Contains(result.Message, "pending scenario application plan");
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
