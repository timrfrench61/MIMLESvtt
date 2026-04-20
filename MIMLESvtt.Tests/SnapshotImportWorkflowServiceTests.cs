using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src;

namespace MIMLESvtt.Tests;

[TestClass]
public class SnapshotImportWorkflowServiceTests
{
    [TestMethod]
    public void ImportWorkflow_WithExportedTableSessionSnapshotJson_ReturnsSupportedOutcomeAndTableSessionPayload()
    {
        var workflow = new SnapshotImportWorkflowService();
        var serializer = new TableSessionSnapshotSerializer();
        var exportedSession = CreateTableSessionFixture();

        var json = serializer.Save(exportedSession);

        var outcome = workflow.Import(json);

        Assert.AreEqual(SnapshotFormatKind.TableSessionSnapshot, outcome.FormatKind);
        Assert.IsTrue(outcome.IsSupported);
        Assert.IsInstanceOfType<TableSession>(outcome.Payload);

        var importedSession = (TableSession)outcome.Payload!;
        Assert.AreEqual(exportedSession.Id, importedSession.Id);
        Assert.AreEqual(exportedSession.Title, importedSession.Title);
        Assert.AreEqual(1, importedSession.Pieces.Count);
        Assert.AreEqual("piece-1", importedSession.Pieces[0].Id);
        Assert.AreEqual(90f, importedSession.Pieces[0].Rotation.Degrees);
        CollectionAssert.AreEqual(new List<string> { "marker-hidden" }, importedSession.Pieces[0].MarkerIds);
    }

    [TestMethod]
    public void ImportWorkflow_WithExportedScenarioSnapshotJson_ReturnsSupportedOutcomeAndScenarioPayload()
    {
        var workflow = new SnapshotImportWorkflowService();
        var serializer = new ScenarioSnapshotSerializer();
        var exportedScenario = CreateScenarioFixture();

        var json = serializer.SerializeScenario(exportedScenario);

        var outcome = workflow.Import(json);

        Assert.AreEqual(SnapshotFormatKind.ScenarioSnapshot, outcome.FormatKind);
        Assert.IsTrue(outcome.IsSupported);
        Assert.IsInstanceOfType<ScenarioExport>(outcome.Payload);

        var importedScenario = (ScenarioExport)outcome.Payload!;
        Assert.AreEqual(exportedScenario.Title, importedScenario.Title);
        Assert.AreEqual(1, importedScenario.Surfaces.Count);
        Assert.AreEqual(1, importedScenario.Pieces.Count);
        Assert.AreEqual(exportedScenario.Pieces[0].Location.SurfaceId, importedScenario.Pieces[0].Location.SurfaceId);
    }

    [TestMethod]
    public void ImportWorkflow_WithTableSessionJson_ReturnsSupportedTableSessionOutcome()
    {
        var workflow = new SnapshotImportWorkflowService();
        var serializer = new TableSessionSnapshotSerializer();

        var json = serializer.SerializeSnapshot(new TableSessionSnapshot
        {
            Version = TableSessionSnapshotSerializer.CurrentVersion,
            TableSession = new TableSession
            {
                Id = "session-1",
                Title = "Session"
            }
        });

        var outcome = workflow.Import(json);

        Assert.AreEqual(SnapshotFormatKind.TableSessionSnapshot, outcome.FormatKind);
        Assert.IsTrue(outcome.IsSupported);
        Assert.IsInstanceOfType<TableSession>(outcome.Payload);
    }

    [TestMethod]
    public void ImportWorkflow_WithScenarioJson_ReturnsSupportedScenarioOutcome()
    {
        var workflow = new SnapshotImportWorkflowService();
        var serializer = new ScenarioSnapshotSerializer();

        var json = serializer.SerializeSnapshot(new ScenarioSnapshot
        {
            Version = ScenarioSnapshotSerializer.CurrentVersion,
            Scenario = new ScenarioExport
            {
                Title = "Scenario"
            }
        });

        var outcome = workflow.Import(json);

        Assert.AreEqual(SnapshotFormatKind.ScenarioSnapshot, outcome.FormatKind);
        Assert.IsTrue(outcome.IsSupported);
        Assert.IsInstanceOfType<ScenarioExport>(outcome.Payload);
    }

    [TestMethod]
    public void ImportWorkflow_WithContentPackJson_ReturnsUnsupportedOutcome()
    {
        var workflow = new SnapshotImportWorkflowService();
        var serializer = new ContentPackSnapshotSerializer();

        var json = serializer.SerializeSnapshot(new ContentPackSnapshot
        {
            Version = ContentPackSnapshotSerializer.CurrentVersion,
            Manifest = new ContentPackManifest { Name = "Pack", Description = "Desc" },
            Definitions = [],
            Assets = []
        });

        var outcome = workflow.Import(json);

        Assert.AreEqual(SnapshotFormatKind.ContentPackSnapshot, outcome.FormatKind);
        Assert.IsFalse(outcome.IsSupported);
        Assert.IsNull(outcome.Payload);
    }

    [TestMethod]
    public void ImportWorkflow_WithActionLogJson_ReturnsUnsupportedOutcome()
    {
        var workflow = new SnapshotImportWorkflowService();
        var serializer = new ActionLogSnapshotSerializer();

        var json = serializer.SerializeSnapshot(new ActionLogSnapshot
        {
            Version = ActionLogSnapshotSerializer.CurrentVersion,
            SessionId = "session-1",
            Actions = []
        });

        var outcome = workflow.Import(json);

        Assert.AreEqual(SnapshotFormatKind.ActionLogSnapshot, outcome.FormatKind);
        Assert.IsFalse(outcome.IsSupported);
        Assert.IsNull(outcome.Payload);
    }

    [TestMethod]
    public void ImportWorkflow_WhenInputMalformed_FailsClearly()
    {
        var workflow = new SnapshotImportWorkflowService();

        var exception = Assert.ThrowsException<InvalidOperationException>(() => workflow.Import("{ \"Version\": 1, \"Scenario\":"));
        StringAssert.Contains(exception.Message, "malformed");
    }

    [TestMethod]
    public void ImportWorkflow_WhenFormatUnknown_FailsClearly()
    {
        var workflow = new SnapshotImportWorkflowService();

        var exception = Assert.ThrowsException<InvalidOperationException>(() => workflow.Import("{\"Version\":1,\"Unknown\":{}}"));
        StringAssert.Contains(exception.Message, "known snapshot format");
    }

    private static TableSession CreateTableSessionFixture()
    {
        return new TableSession
        {
            Id = "session-1",
            Title = "Workflow Session",
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
                        Coordinate = new Coordinate
                        {
                            X = 3,
                            Y = 4
                        }
                    },
                    Rotation = new Rotation
                    {
                        Degrees = 90f
                    },
                    MarkerIds = ["marker-hidden"]
                }
            ],
            Options = new TableOptions
            {
                EnableFog = true,
                EnableTurnTracker = true
            }
        };
    }

    private static ScenarioExport CreateScenarioFixture()
    {
        return new ScenarioExport
        {
            Title = "Workflow Scenario",
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
                        Coordinate = new Coordinate
                        {
                            X = 1,
                            Y = 2
                        }
                    }
                }
            ],
            Options = new TableOptions
            {
                EnableFog = false,
                EnableTurnTracker = true
            }
        };
    }
}
