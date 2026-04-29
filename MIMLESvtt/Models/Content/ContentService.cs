namespace MIMLESvtt.src.Domain.Models.Content;

public class ContentService : IContentService
{
    private readonly IContentRepository _repository;

    public ContentService(IContentRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public IReadOnlyList<ContentEntry> List(ContentCategory category)
    {
        return _repository.List(category);
    }

    public ContentEntry? Detail(ContentCategory category, string id)
    {
        return _repository.GetById(category, id);
    }

    public ContentValidationResult Create(ContentEntry entry)
    {
        var validation = ContentValidationUtilities.ValidateEntry(entry);
        if (!validation.IsValid)
        {
            return validation;
        }

        if (_repository.GetById(entry.Category, entry.Metadata.Id) is not null)
        {
            validation.Errors.Add("Content Id already exists.");
            return validation;
        }

        _repository.Save(entry);
        return validation;
    }

    public ContentValidationResult Edit(ContentEntry entry)
    {
        var validation = ContentValidationUtilities.ValidateEntry(entry);
        if (!validation.IsValid)
        {
            return validation;
        }

        _repository.Save(entry);
        return validation;
    }

    public ContentValidationResult Import(ContentCategory category, IEnumerable<ContentEntry> entries)
    {
        var validation = new ContentValidationResult();

        var imported = entries?.ToList() ?? [];
        foreach (var entry in imported)
        {
            var entryValidation = ContentValidationUtilities.ValidateEntry(entry);
            if (!entryValidation.IsValid)
            {
                validation.Errors.AddRange(entryValidation.Errors.Select(error => $"{entry.Metadata.Id}: {error}"));
                continue;
            }

            if (entry.Category != category)
            {
                validation.Errors.Add($"{entry.Metadata.Id}: category mismatch for import target {category}.");
            }
        }

        if (!validation.IsValid)
        {
            return validation;
        }

        _repository.Import(category, imported);
        return validation;
    }
}
