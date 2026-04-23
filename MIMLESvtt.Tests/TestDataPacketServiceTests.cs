using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.src.Domain.Models.Content.Import;

namespace MIMLESvtt.Tests;

[TestClass]
public class TestDataPacketServiceTests
{
    [TestMethod]
    public void LoadManifest_BaselinePacket_IsValid()
    {
        var service = new TestDataPacketService();
        var packetRoot = Path.Combine(GetSolutionRoot(), "MIMLESvtt", "docs", "03-persistence", "test-packets", "packet-baseline-valid-v1");

        var loaded = service.LoadManifest(packetRoot);

        Assert.IsNotNull(loaded.Manifest);
        Assert.IsTrue(loaded.Validation.IsValid, string.Join(" | ", loaded.Validation.Errors));
        Assert.AreEqual("packet-baseline-valid-v1", loaded.Manifest!.PacketId);
        Assert.AreEqual(4, loaded.Manifest.Files.Count);
    }

    [TestMethod]
    public void LoadExpectedOutcome_BaselinePacket_IsStructurallyValid()
    {
        var service = new TestDataPacketService();
        var packetRoot = Path.Combine(GetSolutionRoot(), "MIMLESvtt", "docs", "03-persistence", "test-packets", "packet-baseline-valid-v1");

        var manifestLoad = service.LoadManifest(packetRoot);
        Assert.IsNotNull(manifestLoad.Manifest);

        var outcomeLoad = service.LoadExpectedOutcome(packetRoot, manifestLoad.Manifest!);

        Assert.IsNotNull(outcomeLoad.Outcome);
        Assert.IsTrue(outcomeLoad.Validation.IsValid, string.Join(" | ", outcomeLoad.Validation.Errors));
        Assert.AreEqual(8, outcomeLoad.Outcome!.ExpectedTotalRows);
        Assert.IsTrue(outcomeLoad.Outcome.ExpectedByFile.ContainsKey("monsters.csv"));
    }

    [TestMethod]
    public void LoadManifest_MissingPacketRoot_FailsClearly()
    {
        var service = new TestDataPacketService();

        var loaded = service.LoadManifest(Path.Combine(GetSolutionRoot(), "MIMLESvtt", "docs", "03-persistence", "test-packets", "packet-missing"));

        Assert.IsNull(loaded.Manifest);
        Assert.IsFalse(loaded.Validation.IsValid);
        StringAssert.Contains(loaded.Validation.Errors[0], "Packet root folder not found");
    }

    [TestMethod]
    public void LoadManifest_WithUnknownCsvType_FailsSchemaValidation()
    {
        var service = new TestDataPacketService();
        var tempPacket = CreateTempPacket(
            manifestJson: """
{
  "PacketId": "packet-temp-v1",
  "Name": "Temp",
  "Version": 1,
  "Intent": "Mixed",
  "Files": ["unknown.csv"],
  "ExpectedOutcomeRef": "expected-outcome.json"
}
""",
            csvFiles: new Dictionary<string, string>(),
            expectedOutcomeJson: "{ \"ExpectedTotalRows\": 0, \"ExpectedByFile\": {}, \"ExpectedSummary\": { \"Created\": 0, \"Updated\": 0, \"Skipped\": 0, \"Failed\": 0, \"Warnings\": 0, \"Errors\": 0 } }");

        try
        {
            var loaded = service.LoadManifest(tempPacket);

            Assert.IsFalse(loaded.Validation.IsValid);
            Assert.IsTrue(loaded.Validation.Errors.Any(e => e.Contains("not a known CSV packet type", StringComparison.OrdinalIgnoreCase)));
        }
        finally
        {
            TryDeleteDirectory(tempPacket);
        }
    }

    [TestMethod]
    public void ListKnownPacketRoots_ReturnsBaselineAndEdgeCaseFolders()
    {
        var service = new TestDataPacketService();
        var packetsRoot = Path.Combine(GetSolutionRoot(), "MIMLESvtt", "docs", "03-persistence", "test-packets");

        var known = service.ListKnownPacketRoots(packetsRoot);

        Assert.IsTrue(known.Contains("packet-baseline-valid-v1"));
        Assert.IsTrue(known.Contains("packet-edge-cases-v1"));
    }

    private static string GetSolutionRoot()
    {
        var current = AppContext.BaseDirectory;
        for (var i = 0; i < 8; i++)
        {
            var probe = Path.Combine(current, "MIMLESvtt.sln");
            if (File.Exists(probe))
            {
                return current;
            }

            var parent = Directory.GetParent(current);
            if (parent is null)
            {
                break;
            }

            current = parent.FullName;
        }

        return Directory.GetCurrentDirectory();
    }

    private static string CreateTempPacket(string manifestJson, Dictionary<string, string> csvFiles, string expectedOutcomeJson)
    {
        var root = Path.Combine(Path.GetTempPath(), "mimlesvtt-packet-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        File.WriteAllText(Path.Combine(root, "manifest.json"), manifestJson);
        File.WriteAllText(Path.Combine(root, "expected-outcome.json"), expectedOutcomeJson);

        foreach (var csv in csvFiles)
        {
            File.WriteAllText(Path.Combine(root, csv.Key), csv.Value);
        }

        return root;
    }

    private static void TryDeleteDirectory(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);
            }
        }
        catch
        {
            // no-op for cleanup best effort
        }
    }
}
