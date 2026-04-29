namespace MIMLESvtt.src.Domain.Models.Content.Import;

public interface ICsvImportAdapter
{
    ContentCategory Category { get; }

    bool CanHandle(ContentCategory category);

    ContentEntry? MapRow(IReadOnlyDictionary<string, string> row, int rowIndex, List<CsvImportIssue> issues);
}
