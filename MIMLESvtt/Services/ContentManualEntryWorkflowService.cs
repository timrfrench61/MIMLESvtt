namespace MIMLESvtt.Services;

public sealed record ContentManualEntryWorkflowOption(
    string ContentTypeName,
    string ListRoute,
    string DetailLabel,
    string EntryLabel);

public class ContentManualEntryWorkflowService
{
    private static readonly IReadOnlyList<ContentManualEntryWorkflowOption> Options =
    [
        new("Monsters", "/content/monsters", "Detail / Review", "Create / Edit"),
        new("Treasure", "/content/treasure", "Detail / Review", "Create / Edit"),
        new("Equipment", "/content/equipment", "Detail / Review", "Create / Edit"),
        new("Magic Items", "/content/magic-items", "Detail / Review", "Create / Edit")
    ];

    public IReadOnlyList<ContentManualEntryWorkflowOption> ListOptions()
    {
        return Options;
    }
}
