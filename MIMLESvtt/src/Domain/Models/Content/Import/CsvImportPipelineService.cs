namespace MIMLESvtt.src.Domain.Models.Content.Import;

public class CsvImportPipelineService
{
    private readonly IContentRepository _repository;
    private readonly IReadOnlyList<ICsvImportAdapter> _adapters;

    public CsvImportPipelineService(IContentRepository repository, IEnumerable<ICsvImportAdapter> adapters)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _adapters = adapters?.ToList() ?? throw new ArgumentNullException(nameof(adapters));
    }

    public CsvImportResult Run(CsvImportRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = new CsvImportResult
        {
            IsSuccess = false
        };

        var issues = result.Issues;

        AddInfo(issues, CsvImportStage.Upload, null, "FileName", $"Import requested for file '{request.FileName}'.");

        if (string.IsNullOrWhiteSpace(request.FileContent))
        {
            AddError(issues, CsvImportStage.Parse, null, "FileContent", "Import file content is empty.");
            FinalizeSummary(result);
            return result;
        }

        var parsedRows = ParseRows(request.FileContent, issues);
        result.TotalRows = parsedRows.Count;

        var adapter = _adapters.FirstOrDefault(a => a.CanHandle(request.Category));
        if (adapter is null)
        {
            AddError(issues, CsvImportStage.Map, null, "Category", $"No import adapter registered for category {request.Category}.");
            FinalizeSummary(result);
            return result;
        }

        var mappedEntries = new List<(ContentEntry entry, int rowIndex)>();
        for (var i = 0; i < parsedRows.Count; i++)
        {
            var rowIndex = i + 1;
            var mapped = adapter.MapRow(parsedRows[i], rowIndex, issues);
            if (mapped is null)
            {
                AddError(issues, CsvImportStage.Map, rowIndex, null, "Row mapping failed.");
                continue;
            }

            mappedEntries.Add((mapped, rowIndex));
        }

        var validEntries = ValidateRows(mappedEntries, request, result, issues);

        PersistRows(validEntries, request, result, issues);

        FinalizeSummary(result);
        return result;
    }

    private static List<Dictionary<string, string>> ParseRows(string content, List<CsvImportIssue> issues)
    {
        var lines = content
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .ToList();

        if (lines.Count == 0)
        {
            AddError(issues, CsvImportStage.Parse, null, "FileContent", "No CSV rows were found.");
            return [];
        }

        var headers = lines[0]
            .Split(',')
            .Select(h => h.Trim())
            .ToArray();

        CsvImportValidationUtilities.ValidateRequiredHeaders(
            headers,
            ["DefinitionId", "Name", "Category"],
            CsvImportStage.Parse,
            issues);

        if (headers.Length == 0)
        {
            AddError(issues, CsvImportStage.Parse, null, "Header", "CSV header row is missing.");
            return [];
        }

        var rows = new List<Dictionary<string, string>>();

        for (var i = 1; i < lines.Count; i++)
        {
            var rowIndex = i;
            var cells = lines[i].Split(',');
            if (cells.Length != headers.Length)
            {
                AddError(issues, CsvImportStage.Parse, rowIndex, null, "CSV row column count does not match header count.");
                continue;
            }

            var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (var c = 0; c < headers.Length; c++)
            {
                row[headers[c]] = cells[c].Trim();
            }

            rows.Add(row);
        }

        return rows;
    }

    private List<(ContentEntry entry, int rowIndex)> ValidateRows(
        List<(ContentEntry entry, int rowIndex)> mappedEntries,
        CsvImportRequest request,
        CsvImportResult result,
        List<CsvImportIssue> issues)
    {
        var validEntries = new List<(ContentEntry entry, int rowIndex)>();

        foreach (var (entry, rowIndex) in mappedEntries)
        {
            var entryValidation = ContentValidationUtilities.ValidateEntry(entry);
            if (!entryValidation.IsValid)
            {
                foreach (var error in entryValidation.Errors)
                {
                    AddError(issues, CsvImportStage.Validate, rowIndex, null, error);
                }

                result.FailedCount++;
                continue;
            }

            var existing = _repository.GetById(request.Category, entry.Metadata.Id);
            if (existing is not null)
            {
                switch (request.DuplicatePolicy)
                {
                    case CsvDuplicateHandlingPolicy.RejectDuplicate:
                        AddError(issues, CsvImportStage.Validate, rowIndex, "Id", "Duplicate id rejected by import policy.");
                        result.FailedCount++;
                        continue;
                    case CsvDuplicateHandlingPolicy.SkipDuplicate:
                        AddWarning(issues, CsvImportStage.Validate, rowIndex, "Id", "Duplicate id skipped by import policy.");
                        result.SkippedCount++;
                        continue;
                    case CsvDuplicateHandlingPolicy.UpdateDuplicate:
                        AddInfo(issues, CsvImportStage.Validate, rowIndex, "Id", "Duplicate id will be updated by import policy.");
                        break;
                }
            }

            validEntries.Add((entry, rowIndex));
        }

        return validEntries;
    }

    private void PersistRows(
        List<(ContentEntry entry, int rowIndex)> validEntries,
        CsvImportRequest request,
        CsvImportResult result,
        List<CsvImportIssue> issues)
    {
        foreach (var (entry, rowIndex) in validEntries)
        {
            try
            {
                var exists = _repository.GetById(request.Category, entry.Metadata.Id) is not null;
                _repository.Save(entry);

                if (exists)
                {
                    result.UpdatedCount++;
                }
                else
                {
                    result.CreatedCount++;
                }
            }
            catch (Exception ex)
            {
                AddError(issues, CsvImportStage.Persist, rowIndex, null, $"Persist failed: {ex.Message}");
                result.FailedCount++;
            }
        }
    }

    private static void FinalizeSummary(CsvImportResult result)
    {
        var warningCount = result.Issues.Count(i => i.Severity == CsvImportIssueSeverity.Warning);
        var errorCount = result.Issues.Count(i => i.Severity == CsvImportIssueSeverity.Error);

        result.Messages.Add($"Processed rows: {result.TotalRows}");
        result.Messages.Add($"Created: {result.CreatedCount}");
        result.Messages.Add($"Updated: {result.UpdatedCount}");
        result.Messages.Add($"Skipped: {result.SkippedCount}");
        result.Messages.Add($"Failed: {result.FailedCount}");
        result.Messages.Add($"Warnings: {warningCount}");
        result.Messages.Add($"Errors: {errorCount}");

        result.IsSuccess = errorCount == 0;
    }

    private static void AddInfo(List<CsvImportIssue> issues, CsvImportStage stage, int? row, string? field, string message)
    {
        issues.Add(new CsvImportIssue
        {
            Severity = CsvImportIssueSeverity.Info,
            ValidationLevel = row.HasValue ? CsvImportValidationLevel.Row : CsvImportValidationLevel.File,
            Stage = stage,
            RowIndex = row,
            Field = field ?? string.Empty,
            Message = message
        });
    }

    private static void AddWarning(List<CsvImportIssue> issues, CsvImportStage stage, int? row, string? field, string message)
    {
        issues.Add(new CsvImportIssue
        {
            Severity = CsvImportIssueSeverity.Warning,
            ValidationLevel = row.HasValue ? CsvImportValidationLevel.Row : CsvImportValidationLevel.File,
            Stage = stage,
            RowIndex = row,
            Field = field ?? string.Empty,
            Message = message
        });
    }

    private static void AddError(List<CsvImportIssue> issues, CsvImportStage stage, int? row, string? field, string message)
    {
        issues.Add(new CsvImportIssue
        {
            Severity = CsvImportIssueSeverity.Error,
            ValidationLevel = row.HasValue ? CsvImportValidationLevel.Row : CsvImportValidationLevel.File,
            Stage = stage,
            RowIndex = row,
            Field = field ?? string.Empty,
            Message = message
        });
    }
}
