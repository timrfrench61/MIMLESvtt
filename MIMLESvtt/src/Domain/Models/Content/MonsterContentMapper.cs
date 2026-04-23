namespace MIMLESvtt.src.Domain.Models.Content;

public static class MonsterContentMapper
{
    public static (MonsterContentEntry? entry, ContentValidationResult validation) FromCsvRow(MonsterCsvImportRow row)
    {
        var validation = new ContentValidationResult();
        if (row is null)
        {
            validation.Errors.Add("Monster CSV row is required.");
            return (null, validation);
        }

        if (string.IsNullOrWhiteSpace(row.DefinitionId))
        {
            validation.Errors.Add("DefinitionId is required.");
        }

        if (string.IsNullOrWhiteSpace(row.Name))
        {
            validation.Errors.Add("Name is required.");
        }

        if (string.IsNullOrWhiteSpace(row.Category))
        {
            validation.Errors.Add("Category is required.");
        }

        if (!string.Equals(row.Category?.Trim(), "Monster", StringComparison.OrdinalIgnoreCase))
        {
            validation.Errors.Add("Category must be Monster.");
        }

        if (!TryParseInt(row.LevelOrThreat, out var levelOrThreat))
        {
            validation.Errors.Add("LevelOrThreat must parse as integer.");
        }

        if (!TryParseInt(row.HitPoints, out var hitPoints))
        {
            validation.Errors.Add("HitPoints must parse as integer.");
        }

        if (!TryParseDecimal(row.Movement, out var movement))
        {
            validation.Errors.Add("Movement must parse as numeric.");
        }

        if (!TryParseInt(row.ArmorClass, out var armorClass))
        {
            validation.Errors.Add("ArmorClass must parse as integer.");
        }

        if (!validation.IsValid)
        {
            return (null, validation);
        }

        var tags = string.IsNullOrWhiteSpace(row.Tags)
            ? []
            : row.Tags
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .ToList();

        var entry = new MonsterContentEntry
        {
            Metadata = new ContentMetadata
            {
                Id = row.DefinitionId.Trim(),
                Name = row.Name.Trim(),
                CategoryType = row.Category.Trim(),
                Source = string.IsNullOrWhiteSpace(row.Source) ? "Import" : row.Source.Trim(),
                Tags = tags,
                Extensions = new Dictionary<string, object>(StringComparer.Ordinal)
            },
            MonsterCategory = row.Category.Trim(),
            LevelOrThreat = levelOrThreat,
            HitPoints = hitPoints,
            Movement = movement,
            ArmorOrDefense = armorClass,
            AttackProfile = string.IsNullOrWhiteSpace(row.AttackProfile) ? "(none)" : row.AttackProfile.Trim()
        };

        return (entry, validation);
    }

    private static bool TryParseInt(string value, out int number)
    {
        return int.TryParse(value?.Trim(), out number);
    }

    private static bool TryParseDecimal(string value, out decimal number)
    {
        return decimal.TryParse(value?.Trim(), out number);
    }
}
