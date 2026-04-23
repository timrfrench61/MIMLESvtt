namespace MIMLESvtt.src.Domain.Models.Content.Import;

public enum CsvImportStage
{
    Upload,
    Parse,
    Map,
    Validate,
    Persist,
    Summarize
}

public enum CsvImportIssueSeverity
{
    Info,
    Warning,
    Error
}

public enum CsvImportValidationLevel
{
    File,
    Row
}

public enum CsvImportFieldType
{
    Integer,
    Decimal,
    Boolean,
    Text
}

public enum CsvDuplicateHandlingPolicy
{
    RejectDuplicate,
    SkipDuplicate,
    UpdateDuplicate
}

public class CsvImportIssue
{
    public CsvImportIssueSeverity Severity { get; init; }

    public CsvImportValidationLevel ValidationLevel { get; init; }

    public CsvImportStage Stage { get; init; }

    public int? RowIndex { get; init; }

    public string Field { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;
}

public class CsvImportResult
{
    public bool IsSuccess { get; set; }

    public int TotalRows { get; set; }

    public int CreatedCount { get; set; }

    public int UpdatedCount { get; set; }

    public int SkippedCount { get; set; }

    public int FailedCount { get; set; }

    public List<string> Messages { get; set; } = [];

    public List<CsvImportIssue> Issues { get; set; } = [];
}

public class CsvImportRequest
{
    public string FileName { get; init; } = string.Empty;

    public string FileContent { get; init; } = string.Empty;

    public ContentCategory Category { get; init; }

    public CsvDuplicateHandlingPolicy DuplicatePolicy { get; init; } = CsvDuplicateHandlingPolicy.RejectDuplicate;

    public DateTime TimestampUtc { get; init; } = DateTime.UtcNow;

    public string OperatorContext { get; init; } = string.Empty;
}
