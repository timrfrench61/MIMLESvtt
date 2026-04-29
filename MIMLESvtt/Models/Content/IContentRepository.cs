namespace MIMLESvtt.src.Domain.Models.Content;

public interface IContentRepository
{
    IReadOnlyList<ContentEntry> List(ContentCategory category);

    ContentEntry? GetById(ContentCategory category, string id);

    void Save(ContentEntry entry);

    bool Delete(ContentCategory category, string id);

    void Import(ContentCategory category, IEnumerable<ContentEntry> entries);
}
