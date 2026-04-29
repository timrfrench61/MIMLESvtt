using System.Text.Json;

namespace MIMLESvtt.src.Domain.Models.Content.Import;

public class TestDataPacketService
{
    private static readonly HashSet<string> SupportedCsvFiles =
    [
        "monsters.csv",
        "treasure.csv",
        "equipment.csv",
        "magic-items.csv",
        "unit-counters.csv"
    ];

    public (TestDataPacketManifest? Manifest, TestDataPacketValidationResult Validation) LoadManifest(string packetRootPath)
    {
        var validation = new TestDataPacketValidationResult();
        var manifestPath = Path.Combine(packetRootPath, "manifest.json");

        if (!Directory.Exists(packetRootPath))
        {
            validation.Errors.Add($"Packet root folder not found: {packetRootPath}");
            return (null, validation);
        }

        if (!File.Exists(manifestPath))
        {
            validation.Errors.Add("manifest.json is missing in packet root.");
            return (null, validation);
        }

        TestDataPacketManifest? manifest;
        try
        {
            manifest = JsonSerializer.Deserialize<TestDataPacketManifest>(File.ReadAllText(manifestPath));
        }
        catch (Exception ex)
        {
            validation.Errors.Add($"Failed to parse manifest.json: {ex.Message}");
            return (null, validation);
        }

        if (manifest is null)
        {
            validation.Errors.Add("manifest.json produced null manifest model.");
            return (null, validation);
        }

        ValidateManifestFields(manifest, validation);
        ValidateManifestFiles(packetRootPath, manifest, validation);

        return (manifest, validation);
    }

    public (TestDataPacketExpectedOutcome? Outcome, TestDataPacketValidationResult Validation) LoadExpectedOutcome(
        string packetRootPath,
        TestDataPacketManifest manifest)
    {
        var validation = new TestDataPacketValidationResult();

        if (string.IsNullOrWhiteSpace(manifest.ExpectedOutcomeRef))
        {
            validation.Warnings.Add("ExpectedOutcomeRef is empty; packet has no expected-outcome validation file.");
            return (null, validation);
        }

        var outcomePath = Path.Combine(packetRootPath, manifest.ExpectedOutcomeRef);
        if (!File.Exists(outcomePath))
        {
            validation.Errors.Add($"Expected outcome file not found: {manifest.ExpectedOutcomeRef}");
            return (null, validation);
        }

        TestDataPacketExpectedOutcome? outcome;
        try
        {
            outcome = JsonSerializer.Deserialize<TestDataPacketExpectedOutcome>(File.ReadAllText(outcomePath));
        }
        catch (Exception ex)
        {
            validation.Errors.Add($"Failed to parse expected outcome file: {ex.Message}");
            return (null, validation);
        }

        if (outcome is null)
        {
            validation.Errors.Add("Expected outcome file produced null model.");
            return (null, validation);
        }

        if (outcome.ExpectedByFile.Count == 0)
        {
            validation.Errors.Add("ExpectedByFile must include at least one file entry.");
        }

        foreach (var file in manifest.Files)
        {
            if (!outcome.ExpectedByFile.ContainsKey(file))
            {
                validation.Warnings.Add($"Expected outcome does not define file entry for '{file}'.");
            }
        }

        return (outcome, validation);
    }

    public IReadOnlyList<string> ListKnownPacketRoots(string packetsRootPath)
    {
        if (!Directory.Exists(packetsRootPath))
        {
            return [];
        }

        return Directory.GetDirectories(packetsRootPath)
            .Select(Path.GetFileName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name!)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static void ValidateManifestFields(TestDataPacketManifest manifest, TestDataPacketValidationResult validation)
    {
        if (string.IsNullOrWhiteSpace(manifest.PacketId))
        {
            validation.Errors.Add("Manifest PacketId is required.");
        }

        if (string.IsNullOrWhiteSpace(manifest.Name))
        {
            validation.Errors.Add("Manifest Name is required.");
        }

        if (manifest.Version <= 0)
        {
            validation.Errors.Add("Manifest Version must be greater than 0.");
        }

        if (string.IsNullOrWhiteSpace(manifest.Intent))
        {
            validation.Errors.Add("Manifest Intent is required.");
        }

        if (!manifest.PacketId.StartsWith("packet-", StringComparison.OrdinalIgnoreCase))
        {
            validation.Warnings.Add("PacketId should follow naming pattern 'packet-<name>-v<version>'.");
        }

        if (manifest.Files.Count == 0)
        {
            validation.Errors.Add("Manifest Files must include at least one CSV file.");
        }
    }

    private static void ValidateManifestFiles(string packetRootPath, TestDataPacketManifest manifest, TestDataPacketValidationResult validation)
    {
        foreach (var file in manifest.Files)
        {
            if (!SupportedCsvFiles.Contains(file))
            {
                validation.Errors.Add($"Manifest file '{file}' is not a known CSV packet type.");
                continue;
            }

            var filePath = Path.Combine(packetRootPath, file);
            if (!File.Exists(filePath))
            {
                validation.Errors.Add($"Manifest file '{file}' was not found in packet root.");
                continue;
            }

            try
            {
                using var stream = File.OpenRead(filePath);
                _ = stream.Length;
            }
            catch (Exception ex)
            {
                validation.Errors.Add($"Manifest file '{file}' is not readable: {ex.Message}");
            }
        }
    }
}
