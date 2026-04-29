using MIMLESvtt.src.Domain.Models.Content;
using MIMLESvtt.src.Domain.Models.Content.Import;

namespace MIMLESvtt.Services;

public enum ContentImportStage
{
    Upload,
    Validate,
    Persist,
    Summarize
}

public enum ContentImportIssueSeverity
{
    Warning,
    Error
}

public enum ContentImportDuplicatePolicy
{
    RejectDuplicate,
    SkipDuplicate,
    UpdateDuplicate
}

public enum ContentImportCategory
{
    Monster,
    Treasure,
    Equipment,
    MagicItem,
    UnitCounter
}

public sealed record ContentImportIssue(
    ContentImportIssueSeverity Severity,
    string Stage,
    int? RowIndex,
    string Message);

public sealed record ContentImportPreviewResult(
    int TotalRows,
    int CreatedCount,
    int UpdatedCount,
    int SkippedCount,
    int FailedCount,
    IReadOnlyList<string> Messages,
    IReadOnlyList<ContentImportIssue> Issues);

public sealed record ContentImportPreviewRequest(
    string FileName,
    string FileContent,
    ContentImportCategory Category,
    ContentImportDuplicatePolicy DuplicatePolicy,
    string OperatorContext);

public class ContentImportWorkflowService
{
    public ContentImportPreviewResult RunPreview(ContentImportPreviewRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var pipeline = new CsvImportPipelineService(new InMemoryContentRepository(),
        [
            new MonsterCsvImportAdapter(),
            new TreasureCsvImportAdapter(),
            new EquipmentCsvImportAdapter(),
            new MagicItemCsvImportAdapter(),
            new UnitCounterCsvImportAdapter()
        ]);

        var result = pipeline.Run(new CsvImportRequest
        {
            FileName = request.FileName,
            FileContent = request.FileContent,
            Category = MapCategory(request.Category),
            DuplicatePolicy = MapDuplicatePolicy(request.DuplicatePolicy),
            OperatorContext = request.OperatorContext
        });

        return new ContentImportPreviewResult(
            result.TotalRows,
            result.CreatedCount,
            result.UpdatedCount,
            result.SkippedCount,
            result.FailedCount,
            result.Messages.ToList(),
            result.Issues
                .Select(issue => new ContentImportIssue(
                    issue.Severity == CsvImportIssueSeverity.Error ? ContentImportIssueSeverity.Error : ContentImportIssueSeverity.Warning,
                    issue.Stage.ToString(),
                    issue.RowIndex,
                    issue.Message))
                .ToList());
    }

    private static ContentCategory MapCategory(ContentImportCategory category)
    {
        return category switch
        {
            ContentImportCategory.Treasure => ContentCategory.Treasure,
            ContentImportCategory.Equipment => ContentCategory.Equipment,
            ContentImportCategory.MagicItem => ContentCategory.MagicItem,
            ContentImportCategory.UnitCounter => ContentCategory.UnitCounter,
            _ => ContentCategory.Monster
        };
    }

    private static CsvDuplicateHandlingPolicy MapDuplicatePolicy(ContentImportDuplicatePolicy policy)
    {
        return policy switch
        {
            ContentImportDuplicatePolicy.SkipDuplicate => CsvDuplicateHandlingPolicy.SkipDuplicate,
            ContentImportDuplicatePolicy.UpdateDuplicate => CsvDuplicateHandlingPolicy.UpdateDuplicate,
            _ => CsvDuplicateHandlingPolicy.RejectDuplicate
        };
    }
}
