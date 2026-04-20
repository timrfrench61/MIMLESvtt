using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src;

namespace MIMLESvtt.Tests;

[TestClass]
public class ContentPackSnapshotSerializerTests
{
    [TestMethod]
    public void ContentPackSerializeDeserialize_RoundTrip_PreservesCoreFields()
    {
        var serializer = new ContentPackSnapshotSerializer();
        var contentPack = CreateContentPackFixture();

        var json = serializer.SerializeContentPack(contentPack);
        var loaded = serializer.DeserializeContentPack(json);

        Assert.AreEqual(contentPack.Version, loaded.Version);
        Assert.AreEqual("Core Pack", loaded.Manifest!.Name);
        Assert.AreEqual("Minimal definitions and assets", loaded.Manifest.Description);

        Assert.AreEqual(1, loaded.Definitions!.Count);
        Assert.AreEqual("def-1", loaded.Definitions[0].Id);
        Assert.AreEqual("Piece", loaded.Definitions[0].Type);

        Assert.AreEqual(1, loaded.Assets!.Count);
        Assert.AreEqual("asset-1", loaded.Assets[0].Id);
        Assert.AreEqual("images/piece.png", loaded.Assets[0].AssetPath);
    }

    [TestMethod]
    public void DeserializeContentPack_WhenManifestMissing_FailsClearly()
    {
        var serializer = new ContentPackSnapshotSerializer();
        var json = "{\"Version\":1,\"Definitions\":[],\"Assets\":[]}";

        var exception = Assert.ThrowsException<InvalidOperationException>(() => serializer.DeserializeContentPack(json));
        StringAssert.Contains(exception.Message, "Manifest");
    }

    [TestMethod]
    public void DeserializeContentPack_WhenDefinitionsMissing_FailsClearly()
    {
        var serializer = new ContentPackSnapshotSerializer();
        var json = "{\"Version\":1,\"Manifest\":{\"Name\":\"Pack\",\"Description\":\"Desc\"},\"Assets\":[]}";

        var exception = Assert.ThrowsException<InvalidOperationException>(() => serializer.DeserializeContentPack(json));
        StringAssert.Contains(exception.Message, "Definitions");
    }

    [TestMethod]
    public void DeserializeContentPack_WhenAssetsMissing_FailsClearly()
    {
        var serializer = new ContentPackSnapshotSerializer();
        var json = "{\"Version\":1,\"Manifest\":{\"Name\":\"Pack\",\"Description\":\"Desc\"},\"Definitions\":[]}";

        var exception = Assert.ThrowsException<InvalidOperationException>(() => serializer.DeserializeContentPack(json));
        StringAssert.Contains(exception.Message, "Assets");
    }

    [TestMethod]
    public void DeserializeContentPack_WhenVersionMissingOrInvalid_FailsClearly()
    {
        var serializer = new ContentPackSnapshotSerializer();

        var missingVersionJson = "{\"Manifest\":{\"Name\":\"Pack\",\"Description\":\"Desc\"},\"Definitions\":[],\"Assets\":[]}";
        var invalidVersionJson = "{\"Version\":999,\"Manifest\":{\"Name\":\"Pack\",\"Description\":\"Desc\"},\"Definitions\":[],\"Assets\":[]}";

        var missingVersionException = Assert.ThrowsException<InvalidOperationException>(() => serializer.DeserializeContentPack(missingVersionJson));
        StringAssert.Contains(missingVersionException.Message, "Version");

        var invalidVersionException = Assert.ThrowsException<InvalidOperationException>(() => serializer.DeserializeContentPack(invalidVersionJson));
        StringAssert.Contains(invalidVersionException.Message, "Version");
    }

    [TestMethod]
    public void ContentPackFormat_IsSeparateFromSessionAndScenarioFormats()
    {
        var contentPackSerializer = new ContentPackSnapshotSerializer();
        var sessionSerializer = new TableSessionSnapshotSerializer();
        var scenarioSerializer = new ScenarioSnapshotSerializer();

        var contentPackJson = contentPackSerializer.SerializeContentPack(CreateContentPackFixture());
        var sessionJson = sessionSerializer.Save(CreateTableSessionFixture());
        var scenarioJson = scenarioSerializer.SerializeScenario(CreateScenarioFixture());

        StringAssert.Contains(contentPackJson, "\"Manifest\"");
        StringAssert.Contains(contentPackJson, "\"Definitions\"");
        StringAssert.Contains(contentPackJson, "\"Assets\"");
        Assert.IsFalse(contentPackJson.Contains("\"TableSession\"", StringComparison.Ordinal));
        Assert.IsFalse(contentPackJson.Contains("\"Scenario\"", StringComparison.Ordinal));

        StringAssert.Contains(sessionJson, "\"TableSession\"");
        Assert.IsFalse(sessionJson.Contains("\"Manifest\"", StringComparison.Ordinal));

        StringAssert.Contains(scenarioJson, "\"Scenario\"");
        Assert.IsFalse(scenarioJson.Contains("\"Manifest\"", StringComparison.Ordinal));
    }

    private static ContentPackSnapshot CreateContentPackFixture()
    {
        return new ContentPackSnapshot
        {
            Version = ContentPackSnapshotSerializer.CurrentVersion,
            Manifest = new ContentPackManifest
            {
                Name = "Core Pack",
                Description = "Minimal definitions and assets"
            },
            Definitions =
            [
                new ContentPackDefinition
                {
                    Id = "def-1",
                    Type = "Piece"
                }
            ],
            Assets =
            [
                new ContentPackAsset
                {
                    Id = "asset-1",
                    AssetPath = "images/piece.png"
                }
            ]
        };
    }

    private static TableSession CreateTableSessionFixture()
    {
        return new TableSession
        {
            Id = "session-1",
            Title = "Session Test"
        };
    }

    private static ScenarioExport CreateScenarioFixture()
    {
        return new ScenarioExport
        {
            Title = "Scenario Test"
        };
    }
}
