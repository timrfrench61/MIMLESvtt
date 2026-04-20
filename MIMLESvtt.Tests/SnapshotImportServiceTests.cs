using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src;

namespace MIMLESvtt.Tests;

[TestClass]
public class SnapshotImportServiceTests
{
    [TestMethod]
    public void ImportDispatcher_WithTableSessionJson_RoutesToTableSessionFormat()
    {
        var tableSerializer = new TableSessionSnapshotSerializer();
        var service = new SnapshotImportService();

        var json = tableSerializer.SerializeSnapshot(new TableSessionSnapshot
        {
            Version = TableSessionSnapshotSerializer.CurrentVersion,
            TableSession = new TableSession { Id = "session-1", Title = "Session" }
        });

        var result = service.Import(json);

        Assert.AreEqual(SnapshotFormatKind.TableSessionSnapshot, result.FormatKind);
        Assert.IsInstanceOfType<TableSessionSnapshot>(result.Payload);
    }

    [TestMethod]
    public void ImportDispatcher_WithScenarioJson_RoutesToScenarioFormat()
    {
        var scenarioSerializer = new ScenarioSnapshotSerializer();
        var service = new SnapshotImportService();

        var json = scenarioSerializer.SerializeSnapshot(new ScenarioSnapshot
        {
            Version = ScenarioSnapshotSerializer.CurrentVersion,
            Scenario = new ScenarioExport { Title = "Scenario" }
        });

        var result = service.Import(json);

        Assert.AreEqual(SnapshotFormatKind.ScenarioSnapshot, result.FormatKind);
        Assert.IsInstanceOfType<ScenarioSnapshot>(result.Payload);
    }

    [TestMethod]
    public void ImportDispatcher_WithContentPackJson_RoutesToContentPackFormat()
    {
        var contentPackSerializer = new ContentPackSnapshotSerializer();
        var service = new SnapshotImportService();

        var json = contentPackSerializer.SerializeSnapshot(new ContentPackSnapshot
        {
            Version = ContentPackSnapshotSerializer.CurrentVersion,
            Manifest = new ContentPackManifest { Name = "Pack", Description = "Desc" },
            Definitions = [],
            Assets = []
        });

        var result = service.Import(json);

        Assert.AreEqual(SnapshotFormatKind.ContentPackSnapshot, result.FormatKind);
        Assert.IsInstanceOfType<ContentPackSnapshot>(result.Payload);
    }

    [TestMethod]
    public void ImportDispatcher_WithActionLogJson_RoutesToActionLogFormat()
    {
        var actionLogSerializer = new ActionLogSnapshotSerializer();
        var service = new SnapshotImportService();

        var json = actionLogSerializer.SerializeSnapshot(new ActionLogSnapshot
        {
            Version = ActionLogSnapshotSerializer.CurrentVersion,
            SessionId = "session-1",
            Actions = []
        });

        var result = service.Import(json);

        Assert.AreEqual(SnapshotFormatKind.ActionLogSnapshot, result.FormatKind);
        Assert.IsInstanceOfType<ActionLogSnapshot>(result.Payload);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("   ")]
    public void ImportDispatcher_WhenInputNullEmptyOrWhitespace_FailsClearly(string input)
    {
        var service = new SnapshotImportService();

        Assert.ThrowsException<ArgumentException>(() => service.Import(input));
    }

    [TestMethod]
    public void ImportDispatcher_WhenJsonMalformed_FailsClearly()
    {
        var service = new SnapshotImportService();

        var exception = Assert.ThrowsException<InvalidOperationException>(() => service.Import("{ \"Version\": 1, \"Scenario\":"));
        StringAssert.Contains(exception.Message, "malformed");
    }

    [TestMethod]
    public void ImportDispatcher_WhenFormatUnknown_FailsClearly()
    {
        var service = new SnapshotImportService();

        var exception = Assert.ThrowsException<InvalidOperationException>(() => service.Import("{\"Version\":1,\"Unknown\":{}}"));
        StringAssert.Contains(exception.Message, "known snapshot format");
    }
}
