namespace MIMLESvtt.src.Domain.Models.Content;

public interface IContentService
{
    IReadOnlyList<ContentEntry> List(ContentCategory category);

    ContentEntry? Detail(ContentCategory category, string id);

    ContentValidationResult Create(ContentEntry entry);

    ContentValidationResult Edit(ContentEntry entry);

    ContentValidationResult Import(ContentCategory category, IEnumerable<ContentEntry> entries);
}
