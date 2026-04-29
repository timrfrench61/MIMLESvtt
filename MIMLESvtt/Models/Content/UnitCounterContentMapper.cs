namespace MIMLESvtt.src.Domain.Models.Content;

public static class UnitCounterContentMapper
{
    public static (UnitCounterContentEntry? entry, ContentValidationResult validation) FromCsvRow(UnitCounterCsvImportRow row)
    {
        var validation = new ContentValidationResult();
        if (row is null)
        {
            validation.Errors.Add("Unit/counter CSV row is required.");
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

        if (string.IsNullOrWhiteSpace(row.UnitType))
        {
            validation.Errors.Add("UnitType is required.");
        }

        if (string.IsNullOrWhiteSpace(row.Side))
        {
            validation.Errors.Add("Side is required.");
        }

        if (!TryParseDecimal(row.StrengthOrValue, out var strengthOrValue))
        {
            validation.Errors.Add("StrengthOrValue must parse as numeric.");
        }

        if (!TryParseDecimal(row.Movement, out var movement))
        {
            validation.Errors.Add("Movement must parse as numeric.");
        }

        if (!TryParseDecimal(row.DefenseOrArmor, out var defenseOrArmor))
        {
            validation.Errors.Add("DefenseOrArmor must parse as numeric.");
        }

        if (!TryParseNullableDecimal(row.RangeOrReach, out var rangeOrReach))
        {
            validation.Errors.Add("RangeOrReach must parse as numeric when provided.");
        }

        if (string.Equals(row.ScenarioMode?.Trim(), "Scenario", StringComparison.OrdinalIgnoreCase)
            && string.IsNullOrWhiteSpace(row.Faction))
        {
            validation.Errors.Add("Faction is required when ScenarioMode is Scenario.");
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

        var entry = new UnitCounterContentEntry
        {
            Metadata = new ContentMetadata
            {
                Id = row.DefinitionId.Trim(),
                Name = row.Name.Trim(),
                CategoryType = string.IsNullOrWhiteSpace(row.Category) ? "UnitCounter" : row.Category.Trim(),
                Source = string.IsNullOrWhiteSpace(row.Source) ? "Import" : row.Source.Trim(),
                Tags = tags,
                Extensions = new Dictionary<string, object>(StringComparer.Ordinal)
            },
            UnitType = row.UnitType.Trim(),
            Side = row.Side.Trim(),
            Faction = row.Faction?.Trim() ?? string.Empty,
            StrengthOrValue = strengthOrValue,
            Movement = movement,
            DefenseOrArmor = defenseOrArmor,
            RangeOrReach = rangeOrReach,
            Description = row.Description?.Trim() ?? string.Empty
        };

        return (entry, validation);
    }

    public static UnitCounterListItemDto ToListItemDto(UnitCounterContentEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        return new UnitCounterListItemDto
        {
            Id = entry.Metadata.Id,
            Name = entry.Metadata.Name,
            UnitType = entry.UnitType,
            Side = entry.Side,
            Faction = entry.Faction,
            StrengthOrValue = entry.StrengthOrValue,
            Movement = entry.Movement
        };
    }

    public static UnitCounterDetailDto ToDetailDto(UnitCounterContentEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        return new UnitCounterDetailDto
        {
            Id = entry.Metadata.Id,
            Name = entry.Metadata.Name,
            UnitType = entry.UnitType,
            Side = entry.Side,
            Faction = entry.Faction,
            StrengthOrValue = entry.StrengthOrValue,
            Movement = entry.Movement,
            DefenseOrArmor = entry.DefenseOrArmor,
            RangeOrReach = entry.RangeOrReach,
            Description = entry.Description,
            Source = entry.Metadata.Source,
            Tags = entry.Metadata.Tags.ToList(),
            Extensions = new Dictionary<string, object>(entry.Metadata.Extensions, StringComparer.Ordinal)
        };
    }

    public static UnitCounterContentEntry FromEditDto(UnitCounterEditDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new UnitCounterContentEntry
        {
            Metadata = new ContentMetadata
            {
                Id = dto.Id,
                Name = dto.Name,
                CategoryType = "UnitCounter",
                Source = dto.Source,
                Tags = dto.Tags.ToList(),
                Extensions = new Dictionary<string, object>(dto.Extensions, StringComparer.Ordinal)
            },
            UnitType = dto.UnitType,
            Side = dto.Side,
            Faction = dto.Faction,
            StrengthOrValue = dto.StrengthOrValue,
            Movement = dto.Movement,
            DefenseOrArmor = dto.DefenseOrArmor,
            RangeOrReach = dto.RangeOrReach,
            Description = dto.Description
        };
    }

    private static bool TryParseDecimal(string value, out decimal number)
    {
        return decimal.TryParse(value?.Trim(), out number);
    }

    private static bool TryParseNullableDecimal(string value, out decimal? number)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            number = null;
            return true;
        }

        if (decimal.TryParse(value.Trim(), out var parsed))
        {
            number = parsed;
            return true;
        }

        number = null;
        return false;
    }
}
