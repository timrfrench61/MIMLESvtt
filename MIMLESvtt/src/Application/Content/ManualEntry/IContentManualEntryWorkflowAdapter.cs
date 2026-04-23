namespace MIMLESvtt.src.Application.Content.ManualEntry;

public interface IContentManualEntryWorkflowAdapter
{
    string ContentTypeName { get; }

    string ListRoute { get; }

    string DetailLabel { get; }

    string EntryLabel { get; }
}
