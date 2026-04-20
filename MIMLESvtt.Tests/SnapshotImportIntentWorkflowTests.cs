using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src;

namespace MIMLESvtt.Tests;

[TestClass]
public class SnapshotImportIntentWorkflowTests
{
    [TestMethod]
    public void Workflow_WithTableSessionSnapshotJson_ProducesReplaceTableSessionIntent()
    {
        var workflow = new SnapshotImportWorkflowService();
        var intentService = new SnapshotImportIntentService();
        var serializer = new TableSessionSnapshotSerializer();

        var json = serializer.Save(CreateTableSessionFixture());

        var outcome = workflow.Import(json);
        var intent = intentService.CreateIntent(outcome);

        Assert.IsTrue(intent.IsSupported);
        Assert.AreEqual(SnapshotImportApplyOperationKind.ReplaceTableSession, intent.OperationKind);
        Assert.IsInstanceOfType<TableSession>(intent.Payload);
    }

    [TestMethod]
    public void Workflow_WithScenarioSnapshotJson_ProducesCreateScenarioFromImportIntent()
    {
        var workflow = new SnapshotImportWorkflowService();
        var intentService = new SnapshotImportIntentService();
        var serializer = new ScenarioSnapshotSerializer();

        var json = serializer.SerializeScenario(CreateScenarioFixture());

        var outcome = workflow.Import(json);
        var intent = intentService.CreateIntent(outcome);

        Assert.IsTrue(intent.IsSupported);
        Assert.AreEqual(SnapshotImportApplyOperationKind.CreateScenarioFromImport, intent.OperationKind);
        Assert.IsInstanceOfType<ScenarioExport>(intent.Payload);
    }

    [TestMethod]
    public void Workflow_WithContentPackSnapshotJson_ProducesUnsupportedIntent()
    {
        var workflow = new SnapshotImportWorkflowService();
        var intentService = new SnapshotImportIntentService();
        var serializer = new ContentPackSnapshotSerializer();

        var json = serializer.SerializeSnapshot(new ContentPackSnapshot
        {
            Version = ContentPackSnapshotSerializer.CurrentVersion,
            Manifest = new ContentPackManifest { Name = "Pack", Description = "Desc" },
            Definitions = [],
            Assets = []
        });

        var outcome = workflow.Import(json);
        var intent = intentService.CreateIntent(outcome);

        Assert.IsFalse(intent.IsSupported);
        Assert.AreEqual(SnapshotImportApplyOperationKind.Unsupported, intent.OperationKind);
        Assert.IsNull(intent.Payload);
    }

    [TestMethod]
    public void Workflow_WithActionLogSnapshotJson_ProducesUnsupportedIntent()
    {
        var workflow = new SnapshotImportWorkflowService();
        var intentService = new SnapshotImportIntentService();
        var serializer = new ActionLogSnapshotSerializer();

        var json = serializer.SerializeSnapshot(new ActionLogSnapshot
        {
            Version = ActionLogSnapshotSerializer.CurrentVersion,
            SessionId = "session-1",
            Actions = []
        });

        var outcome = workflow.Import(json);
        var intent = intentService.CreateIntent(outcome);

        Assert.IsFalse(intent.IsSupported);
        Assert.AreEqual(SnapshotImportApplyOperationKind.Unsupported, intent.OperationKind);
        Assert.IsNull(intent.Payload);
    }

    private static TableSession CreateTableSessionFixture()
    {
        return new TableSession
        {
            Id = "session-1",
            Title = "Session",
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

    private static ScenarioExport CreateScenarioFixture()
    {
        return new ScenarioExport
        {
            Title = "Scenario",
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
                        Coordinate = new Coordinate { X = 2, Y = 2 }
                    }
                }
            ]
        };
    }
}
