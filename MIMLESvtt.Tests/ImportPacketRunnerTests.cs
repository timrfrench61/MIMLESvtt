using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MIMLESvtt.Tests;

[TestClass]
public class ImportPacketRunnerTests
{
    [TestMethod]
    public void BaselinePacket_RowCounts_MatchExpectedOutcome()
    {
        var runner = new ImportPacketRunner();
        var packetDirectory = Path.Combine(FindRepoRoot(), "MIMLESvtt", "docs", "03-persistence", "test-packets", "packet-baseline-valid-v1");

        var result = runner.EvaluatePacket(packetDirectory);

        Assert.IsTrue(result.Success, string.Join(Environment.NewLine, result.Messages));
        Assert.AreEqual(8, result.TotalRows);
        Assert.AreEqual(2, result.RowCountsByFile["monsters.csv"]);
        Assert.AreEqual(2, result.RowCountsByFile["treasure.csv"]);
        Assert.AreEqual(2, result.RowCountsByFile["equipment.csv"]);
        Assert.AreEqual(2, result.RowCountsByFile["magic-items.csv"]);
    }

    [TestMethod]
    public void EdgePacket_RowCounts_MatchExpectedOutcomeAndContainsWarningErrorExpectations()
    {
        var runner = new ImportPacketRunner();
        var packetDirectory = Path.Combine(FindRepoRoot(), "MIMLESvtt", "docs", "03-persistence", "test-packets", "packet-edge-cases-v1");

        var result = runner.EvaluatePacket(packetDirectory);

        Assert.IsTrue(result.Success, string.Join(Environment.NewLine, result.Messages));
        Assert.AreEqual(13, result.TotalRows);
        Assert.AreEqual(4, result.RowCountsByFile["monsters.csv"]);
        Assert.AreEqual(3, result.RowCountsByFile["treasure.csv"]);
        Assert.AreEqual(3, result.RowCountsByFile["equipment.csv"]);
        Assert.AreEqual(3, result.RowCountsByFile["magic-items.csv"]);

        Assert.IsTrue(result.ExpectedWarningsCount > 0);
        Assert.IsTrue(result.ExpectedErrorsCount > 0);
    }

    private static string FindRepoRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "MIMLESvtt.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Unable to locate repository root containing MIMLESvtt.sln.");
    }

    private sealed class ImportPacketRunner
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ImportPacketEvaluationResult EvaluatePacket(string packetDirectory)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(packetDirectory);

            var manifestPath = Path.Combine(packetDirectory, "manifest.json");
            var expectedOutcomePath = Path.Combine(packetDirectory, "expected-outcome.json");

            var manifest = LoadManifest(manifestPath);
            var expected = LoadExpectedOutcome(expectedOutcomePath);

            var rowCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var messages = new List<string>();
            var success = true;

            foreach (var file in manifest.Files)
            {
                var fullPath = Path.Combine(packetDirectory, file);
                if (!File.Exists(fullPath))
                {
                    messages.Add($"Missing file: {file}");
                    success = false;
                    continue;
                }

                var rows = CountDataRows(fullPath);
                rowCounts[file] = rows;

                if (expected.ExpectedByFile.TryGetValue(file, out var expectedFile)
                    && expectedFile.Rows.HasValue
                    && expectedFile.Rows.Value != rows)
                {
                    messages.Add($"Row count mismatch for {file}: expected {expectedFile.Rows.Value}, actual {rows}.");
                    success = false;
                }
            }

            var totalRows = rowCounts.Values.Sum();
            if (expected.ExpectedTotalRows.HasValue && expected.ExpectedTotalRows.Value != totalRows)
            {
                messages.Add($"Total row count mismatch: expected {expected.ExpectedTotalRows.Value}, actual {totalRows}.");
                success = false;
            }

            if (messages.Count == 0)
            {
                messages.Add("Packet evaluation passed.");
            }

            return new ImportPacketEvaluationResult
            {
                Success = success,
                TotalRows = totalRows,
                RowCountsByFile = rowCounts,
                Messages = messages,
                ExpectedWarningsCount = expected.ExpectedByFile.Values.Sum(v => v.ExpectedWarnings.Count) + expected.ExpectedSummary.Warnings,
                ExpectedErrorsCount = expected.ExpectedByFile.Values.Sum(v => v.ExpectedErrors.Count) + expected.ExpectedSummary.Errors
            };
        }

        private static ImportPacketManifest LoadManifest(string manifestPath)
        {
            if (!File.Exists(manifestPath))
            {
                throw new FileNotFoundException("Manifest file was not found.", manifestPath);
            }

            var json = File.ReadAllText(manifestPath);
            var manifest = JsonSerializer.Deserialize<ImportPacketManifest>(json, JsonOptions);
            if (manifest is null || manifest.Files.Count == 0)
            {
                throw new InvalidOperationException("Manifest content is invalid or has no files.");
            }

            return manifest;
        }

        private static ImportPacketExpectedOutcome LoadExpectedOutcome(string expectedOutcomePath)
        {
            if (!File.Exists(expectedOutcomePath))
            {
                throw new FileNotFoundException("Expected outcome file was not found.", expectedOutcomePath);
            }

            var json = File.ReadAllText(expectedOutcomePath);
            var expected = JsonSerializer.Deserialize<ImportPacketExpectedOutcome>(json, JsonOptions);
            if (expected is null)
            {
                throw new InvalidOperationException("Expected outcome content is invalid.");
            }

            return expected;
        }

        private static int CountDataRows(string csvFilePath)
        {
            var lines = File.ReadAllLines(csvFilePath)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToList();

            if (lines.Count <= 1)
            {
                return 0;
            }

            return lines.Count - 1;
        }
    }

    private sealed class ImportPacketEvaluationResult
    {
        public bool Success { get; set; }

        public int TotalRows { get; set; }

        public Dictionary<string, int> RowCountsByFile { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        public List<string> Messages { get; set; } = [];

        public int ExpectedWarningsCount { get; set; }

        public int ExpectedErrorsCount { get; set; }
    }

    private sealed class ImportPacketManifest
    {
        public string PacketId { get; set; } = string.Empty;

        public List<string> Files { get; set; } = [];
    }

    private sealed class ImportPacketExpectedOutcome
    {
        public int? ExpectedTotalRows { get; set; }

        public Dictionary<string, ImportPacketExpectedByFile> ExpectedByFile { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        public ImportPacketExpectedSummary ExpectedSummary { get; set; } = new();
    }

    private sealed class ImportPacketExpectedByFile
    {
        public int? Rows { get; set; }

        public List<string> ExpectedWarnings { get; set; } = [];

        public List<string> ExpectedErrors { get; set; } = [];
    }

    private sealed class ImportPacketExpectedSummary
    {
        public int Warnings { get; set; }

        public int Errors { get; set; }
    }
}
