namespace MIMLESvtt.src.Application.Content.ManualEntry;

public sealed class ContentManualEntryWorkflowAdapter : IContentManualEntryWorkflowAdapter
{
    public required string ContentTypeName { get; init; }

    public required string ListRoute { get; init; }

    public string DetailLabel { get; init; } = "Detail / Review";

    public string EntryLabel { get; init; } = "Create / Edit";
}
