using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src;

namespace MIMLESvtt.Tests;

[TestClass]
public class SnapshotImportApplicationServiceTests
{
    [TestMethod]
    public void ImportApplication_WithTableSessionSnapshot_ReturnsSupportedOutcomeAndTableSession()
    {
        var service = new SnapshotImportApplicationService();
        var result = new SnapshotImportResult(
            SnapshotFormatKind.TableSessionSnapshot,
            new TableSessionSnapshot
            {
                Version = TableSessionSnapshotSerializer.CurrentVersion,
                TableSession = new TableSession
                {
                    Id = "session-1",
                    Title = "Session"
                }
            });

        var outcome = service.Apply(result);

        Assert.AreEqual(SnapshotFormatKind.TableSessionSnapshot, outcome.FormatKind);
        Assert.IsTrue(outcome.IsSupported);
        Assert.IsInstanceOfType<TableSession>(outcome.Payload);

        var tableSession = (TableSession)outcome.Payload!;
        Assert.AreEqual("session-1", tableSession.Id);
    }

    [TestMethod]
    public void ImportApplication_WithScenarioSnapshot_ReturnsSupportedOutcomeAndScenario()
    {
        var service = new SnapshotImportApplicationService();
        var result = new SnapshotImportResult(
            SnapshotFormatKind.ScenarioSnapshot,
            new ScenarioSnapshot
            {
                Version = ScenarioSnapshotSerializer.CurrentVersion,
                Scenario = new ScenarioExport
                {
                    Title = "Scenario"
                }
            });

        var outcome = service.Apply(result);

        Assert.AreEqual(SnapshotFormatKind.ScenarioSnapshot, outcome.FormatKind);
        Assert.IsTrue(outcome.IsSupported);
        Assert.IsInstanceOfType<ScenarioExport>(outcome.Payload);

        var scenario = (ScenarioExport)outcome.Payload!;
        Assert.AreEqual("Scenario", scenario.Title);
    }

    [TestMethod]
    public void ImportApplication_WithContentPackSnapshot_ReturnsUnsupportedOutcome()
    {
        var service = new SnapshotImportApplicationService();
        var result = new SnapshotImportResult(
            SnapshotFormatKind.ContentPackSnapshot,
            new ContentPackSnapshot
            {
                Version = ContentPackSnapshotSerializer.CurrentVersion,
                Manifest = new ContentPackManifest { Name = "Pack", Description = "Desc" },
                Definitions = [],
                Assets = []
            });

        var outcome = service.Apply(result);

        Assert.AreEqual(SnapshotFormatKind.ContentPackSnapshot, outcome.FormatKind);
        Assert.IsFalse(outcome.IsSupported);
        Assert.IsNull(outcome.Payload);
        StringAssert.Contains(outcome.Message, "not supported");
    }

    [TestMethod]
    public void ImportApplication_WithActionLogSnapshot_ReturnsUnsupportedOutcome()
    {
        var service = new SnapshotImportApplicationService();
        var result = new SnapshotImportResult(
            SnapshotFormatKind.ActionLogSnapshot,
            new ActionLogSnapshot
            {
                Version = ActionLogSnapshotSerializer.CurrentVersion,
                SessionId = "session-1",
                Actions = []
            });

        var outcome = service.Apply(result);

        Assert.AreEqual(SnapshotFormatKind.ActionLogSnapshot, outcome.FormatKind);
        Assert.IsFalse(outcome.IsSupported);
        Assert.IsNull(outcome.Payload);
        StringAssert.Contains(outcome.Message, "not supported");
    }

    [TestMethod]
    public void ImportApplication_WhenResultPayloadMissingOrMismatched_FailsClearly()
    {
        var service = new SnapshotImportApplicationService();
        var mismatched = new SnapshotImportResult(
            SnapshotFormatKind.TableSessionSnapshot,
            new ScenarioSnapshot
            {
                Version = ScenarioSnapshotSerializer.CurrentVersion,
                Scenario = new ScenarioExport { Title = "Scenario" }
            });

        var exception = Assert.ThrowsException<InvalidOperationException>(() => service.Apply(mismatched));
        StringAssert.Contains(exception.Message, "TableSessionSnapshot");
    }
}
