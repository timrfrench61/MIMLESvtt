namespace MIMLESvtt.src.Domain.Models.Content.Import;

public static class CsvImportValidationUtilities
{
    public static void ValidateRequiredField(
        IReadOnlyDictionary<string, string> row,
        string field,
        int rowIndex,
        CsvImportStage stage,
        List<CsvImportIssue> issues)
    {
        var value = row.TryGetValue(field, out var existing) ? existing : string.Empty;
        if (!string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        issues.Add(new CsvImportIssue
        {
            Severity = CsvImportIssueSeverity.Error,
            ValidationLevel = CsvImportValidationLevel.Row,
            Stage = stage,
            RowIndex = rowIndex,
            Field = field,
            Message = $"Required field '{field}' is missing."
        });
    }

    public static void ValidateOptionalFieldType(
        IReadOnlyDictionary<string, string> row,
        string field,
        CsvImportFieldType fieldType,
        int rowIndex,
        CsvImportStage stage,
        List<CsvImportIssue> issues)
    {
        if (!row.TryGetValue(field, out var value) || string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        var isValid = fieldType switch
        {
            CsvImportFieldType.Integer => int.TryParse(value.Trim(), out _),
            CsvImportFieldType.Decimal => decimal.TryParse(value.Trim(), out _),
            CsvImportFieldType.Boolean => bool.TryParse(value.Trim(), out _)
                || string.Equals(value.Trim(), "yes", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value.Trim(), "no", StringComparison.OrdinalIgnoreCase),
            _ => true
        };

        if (isValid)
        {
            return;
        }

        issues.Add(new CsvImportIssue
        {
            Severity = CsvImportIssueSeverity.Error,
            ValidationLevel = CsvImportValidationLevel.Row,
            Stage = stage,
            RowIndex = rowIndex,
            Field = field,
            Message = $"Optional field '{field}' failed {fieldType} parsing."
        });
    }

    public static void ValidateOptionalExtensions(
        IReadOnlyDictionary<string, string> row,
        int rowIndex,
        CsvImportStage stage,
        List<CsvImportIssue> issues)
    {
        foreach (var kvp in row)
        {
            if (!kvp.Key.StartsWith("Ext.", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(kvp.Key.Replace("Ext.", string.Empty, StringComparison.OrdinalIgnoreCase)))
            {
                issues.Add(new CsvImportIssue
                {
                    Severity = CsvImportIssueSeverity.Error,
                    ValidationLevel = CsvImportValidationLevel.Row,
                    Stage = stage,
                    RowIndex = rowIndex,
                    Field = kvp.Key,
                    Message = "Extension metadata key must be non-empty."
                });
            }
        }
    }

    public static void ValidateRequiredHeaders(
        IReadOnlyList<string> headers,
        IEnumerable<string> requiredHeaders,
        CsvImportStage stage,
        List<CsvImportIssue> issues)
    {
        foreach (var required in requiredHeaders)
        {
            if (headers.Any(h => string.Equals(h, required, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            issues.Add(new CsvImportIssue
            {
                Severity = CsvImportIssueSeverity.Error,
                ValidationLevel = CsvImportValidationLevel.File,
                Stage = stage,
                RowIndex = null,
                Field = required,
                Message = $"Required header '{required}' is missing."
            });
        }
    }
}
