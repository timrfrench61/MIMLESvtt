namespace MIMLESvtt.src.Application.Content.ManualEntry;

public class ContentManualEntryWorkflowRegistry
{
    private static readonly IReadOnlyList<IContentManualEntryWorkflowAdapter> _adapters =
    [
        new ContentManualEntryWorkflowAdapter
        {
            ContentTypeName = "Monsters",
            ListRoute = "/content/monsters"
        },
        new ContentManualEntryWorkflowAdapter
        {
            ContentTypeName = "Treasure",
            ListRoute = "/content/treasure"
        },
        new ContentManualEntryWorkflowAdapter
        {
            ContentTypeName = "Equipment",
            ListRoute = "/content/equipment"
        },
        new ContentManualEntryWorkflowAdapter
        {
            ContentTypeName = "Magic Items",
            ListRoute = "/content/magic-items"
        }
    ];

    public IReadOnlyList<IContentManualEntryWorkflowAdapter> ListAdapters()
    {
        return _adapters;
    }
}
