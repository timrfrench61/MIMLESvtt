using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src;

namespace MIMLESvtt.Tests;

[TestClass]
public class SnapshotImportIntentServiceTests
{
    [TestMethod]
    public void IntentService_WithSupportedTableSessionOutcome_ReturnsReplaceTableSessionIntent()
    {
        var service = new SnapshotImportIntentService();
        var tableSession = new TableSession { Id = "session-1", Title = "Session" };

        var applicationOutcome = new SnapshotImportApplicationOutcome(
            SnapshotFormatKind.TableSessionSnapshot,
            true,
            tableSession,
            "supported");

        var intent = service.CreateIntent(applicationOutcome);

        Assert.IsTrue(intent.IsSupported);
        Assert.AreEqual(SnapshotFormatKind.TableSessionSnapshot, intent.SourceFormat);
        Assert.AreEqual(SnapshotImportApplyOperationKind.ReplaceTableSession, intent.OperationKind);
        Assert.AreSame(tableSession, intent.Payload);
    }

    [TestMethod]
    public void IntentService_WithSupportedScenarioOutcome_ReturnsCreateScenarioFromImportIntent()
    {
        var service = new SnapshotImportIntentService();
        var scenario = new ScenarioExport { Title = "Scenario" };

        var applicationOutcome = new SnapshotImportApplicationOutcome(
            SnapshotFormatKind.ScenarioSnapshot,
            true,
            scenario,
            "supported");

        var intent = service.CreateIntent(applicationOutcome);

        Assert.IsTrue(intent.IsSupported);
        Assert.AreEqual(SnapshotFormatKind.ScenarioSnapshot, intent.SourceFormat);
        Assert.AreEqual(SnapshotImportApplyOperationKind.CreateScenarioFromImport, intent.OperationKind);
        Assert.AreSame(scenario, intent.Payload);
    }

    [TestMethod]
    public void IntentService_WithUnsupportedContentPackOutcome_ReturnsUnsupportedIntent()
    {
        var service = new SnapshotImportIntentService();
        var applicationOutcome = new SnapshotImportApplicationOutcome(
            SnapshotFormatKind.ContentPackSnapshot,
            false,
            null,
            "ContentPackSnapshot is not supported for application in this pass.");

        var intent = service.CreateIntent(applicationOutcome);

        Assert.IsFalse(intent.IsSupported);
        Assert.AreEqual(SnapshotFormatKind.ContentPackSnapshot, intent.SourceFormat);
        Assert.AreEqual(SnapshotImportApplyOperationKind.Unsupported, intent.OperationKind);
        Assert.IsNull(intent.Payload);
    }

    [TestMethod]
    public void IntentService_WithUnsupportedActionLogOutcome_ReturnsUnsupportedIntent()
    {
        var service = new SnapshotImportIntentService();
        var applicationOutcome = new SnapshotImportApplicationOutcome(
            SnapshotFormatKind.ActionLogSnapshot,
            false,
            null,
            "ActionLogSnapshot is not supported for application in this pass.");

        var intent = service.CreateIntent(applicationOutcome);

        Assert.IsFalse(intent.IsSupported);
        Assert.AreEqual(SnapshotFormatKind.ActionLogSnapshot, intent.SourceFormat);
        Assert.AreEqual(SnapshotImportApplyOperationKind.Unsupported, intent.OperationKind);
        Assert.IsNull(intent.Payload);
    }

    [TestMethod]
    public void IntentService_WithSupportedTableSessionOutcomeAndMissingPayload_FailsClearly()
    {
        var service = new SnapshotImportIntentService();
        var applicationOutcome = new SnapshotImportApplicationOutcome(
            SnapshotFormatKind.TableSessionSnapshot,
            true,
            null!,
            "supported");

        var exception = Assert.ThrowsException<InvalidOperationException>(() => service.CreateIntent(applicationOutcome));
        StringAssert.Contains(exception.Message, "TableSession");
    }

    [TestMethod]
    public void IntentService_WithSupportedScenarioOutcomeAndWrongPayloadType_FailsClearly()
    {
        var service = new SnapshotImportIntentService();
        var applicationOutcome = new SnapshotImportApplicationOutcome(
            SnapshotFormatKind.ScenarioSnapshot,
            true,
            new TableSession { Id = "session-1" },
            "supported");

        var exception = Assert.ThrowsException<InvalidOperationException>(() => service.CreateIntent(applicationOutcome));
        StringAssert.Contains(exception.Message, "ScenarioExport");
    }
}
