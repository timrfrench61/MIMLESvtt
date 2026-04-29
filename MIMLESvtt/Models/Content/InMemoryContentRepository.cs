namespace MIMLESvtt.src.Domain.Models.Content;

public class InMemoryContentRepository : IContentRepository
{
    private readonly Dictionary<ContentCategory, List<ContentEntry>> _entriesByCategory = new();

    public IReadOnlyList<ContentEntry> List(ContentCategory category)
    {
        return GetBucket(category).ToList();
    }

    public ContentEntry? GetById(ContentCategory category, string id)
    {
        return GetBucket(category)
            .FirstOrDefault(e => string.Equals(e.Metadata.Id, id, StringComparison.Ordinal));
    }

    public void Save(ContentEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        var bucket = GetBucket(entry.Category);
        var existingIndex = bucket.FindIndex(e => string.Equals(e.Metadata.Id, entry.Metadata.Id, StringComparison.Ordinal));
        if (existingIndex >= 0)
        {
            bucket[existingIndex] = entry;
            return;
        }

        bucket.Add(entry);
    }

    public bool Delete(ContentCategory category, string id)
    {
        var bucket = GetBucket(category);
        var removed = bucket.RemoveAll(e => string.Equals(e.Metadata.Id, id, StringComparison.Ordinal));
        return removed > 0;
    }

    public void Import(ContentCategory category, IEnumerable<ContentEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);

        foreach (var entry in entries)
        {
            if (entry.Category != category)
            {
                throw new InvalidOperationException("Imported content entry category does not match target import category.");
            }

            Save(entry);
        }
    }

    private List<ContentEntry> GetBucket(ContentCategory category)
    {
        if (!_entriesByCategory.TryGetValue(category, out var bucket))
        {
            bucket = [];
            _entriesByCategory[category] = bucket;
        }

        return bucket;
    }
}
