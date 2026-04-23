namespace MIMLESvtt.src.Domain.Models.Content.Import;

public class TestDataPacketManifest
{
    public string PacketId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int Version { get; set; }

    public string Intent { get; set; } = string.Empty;

    public List<string> Files { get; set; } = [];

    public string Notes { get; set; } = string.Empty;

    public string ExpectedOutcomeRef { get; set; } = string.Empty;
}

public class TestDataPacketExpectedOutcome
{
    public int ExpectedTotalRows { get; set; }

    public Dictionary<string, TestDataPacketExpectedFileOutcome> ExpectedByFile { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public TestDataPacketExpectedSummary ExpectedSummary { get; set; } = new();
}

public class TestDataPacketExpectedFileOutcome
{
    public int Rows { get; set; }

    public int Created { get; set; }

    public int Updated { get; set; }

    public int Skipped { get; set; }

    public int Failed { get; set; }

    public List<string> ExpectedWarnings { get; set; } = [];

    public List<string> ExpectedErrors { get; set; } = [];
}

public class TestDataPacketExpectedSummary
{
    public int Created { get; set; }

    public int Updated { get; set; }

    public int Skipped { get; set; }

    public int Failed { get; set; }

    public int Warnings { get; set; }

    public int Errors { get; set; }
}

public class TestDataPacketValidationResult
{
    public bool IsValid => Errors.Count == 0;

    public List<string> Errors { get; } = [];

    public List<string> Warnings { get; } = [];
}
