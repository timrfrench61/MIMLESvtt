namespace MIMLESvtt.src.Domain.Models.Content;

public static class MagicItemContentMapper
{
    public static (MagicItemContentEntry? entry, ContentValidationResult validation) FromCsvRow(MagicItemCsvImportRow row)
    {
        var validation = new ContentValidationResult();
        if (row is null)
        {
            validation.Errors.Add("Magic item CSV row is required.");
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

        if (!string.Equals(row.Category?.Trim(), "MagicItem", StringComparison.OrdinalIgnoreCase))
        {
            validation.Errors.Add("Category must be MagicItem.");
        }

        if (string.IsNullOrWhiteSpace(row.Type))
        {
            validation.Errors.Add("Type is required.");
        }

        if (!TryParseDecimal(row.Value, out var value))
        {
            validation.Errors.Add("Value must parse as numeric.");
        }

        if (!TryParseNullableInt(row.Charges, out var chargesOrUses))
        {
            validation.Errors.Add("Charges must parse as integer when provided.");
        }

        if (!TryParseBool(row.AttunementRequired, out var attunementRequired))
        {
            validation.Errors.Add("AttunementRequired must parse as boolean when provided.");
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

        var definitionId = row.DefinitionId?.Trim() ?? string.Empty;
        var name = row.Name?.Trim() ?? string.Empty;
        var category = row.Category?.Trim() ?? string.Empty;
        var type = row.Type?.Trim() ?? string.Empty;
        var source = string.IsNullOrWhiteSpace(row.Source) ? "Import" : row.Source.Trim();

        var entry = new MagicItemContentEntry
        {
            Metadata = new ContentMetadata
            {
                Id = definitionId,
                Name = name,
                CategoryType = category,
                Source = source,
                Tags = tags,
                Extensions = new Dictionary<string, object>(StringComparer.Ordinal)
            },
            ItemType = type,
            Rarity = string.IsNullOrWhiteSpace(row.Rarity) ? "Unspecified" : row.Rarity.Trim(),
            AttunementRequired = attunementRequired,
            ChargesOrUses = chargesOrUses,
            Value = value,
            Description = row.Description?.Trim() ?? string.Empty,
            EffectMetadata = new MagicItemEffectMetadata()
        };

        return (entry, validation);
    }

    public static MagicItemDetailDto ToDetailDto(MagicItemContentEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        return new MagicItemDetailDto
        {
            Id = entry.Metadata.Id,
            Name = entry.Metadata.Name,
            ItemType = entry.ItemType,
            Rarity = entry.Rarity,
            AttunementRequired = entry.AttunementRequired,
            ChargesOrUses = entry.ChargesOrUses,
            Value = entry.Value,
            Description = entry.Description,
            Source = entry.Metadata.Source,
            Tags = entry.Metadata.Tags.ToList(),
            EffectMetadata = CloneEffectMetadata(entry.EffectMetadata),
            Extensions = new Dictionary<string, object>(entry.Metadata.Extensions, StringComparer.Ordinal)
        };
    }

    public static MagicItemContentEntry FromEditDto(MagicItemEditDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new MagicItemContentEntry
        {
            Metadata = new ContentMetadata
            {
                Id = dto.Id,
                Name = dto.Name,
                CategoryType = "MagicItem",
                Source = dto.Source,
                Tags = dto.Tags.ToList(),
                Extensions = new Dictionary<string, object>(dto.Extensions, StringComparer.Ordinal)
            },
            ItemType = dto.ItemType,
            Rarity = dto.Rarity,
            AttunementRequired = dto.AttunementRequired,
            ChargesOrUses = dto.ChargesOrUses,
            Value = dto.Value,
            Description = dto.Description,
            EffectMetadata = CloneEffectMetadata(dto.EffectMetadata)
        };
    }

    private static MagicItemEffectMetadata CloneEffectMetadata(MagicItemEffectMetadata metadata)
    {
        return new MagicItemEffectMetadata
        {
            PassiveBonuses = metadata.PassiveBonuses.ToList(),
            ActivatedEffects = metadata.ActivatedEffects.ToList(),
            TriggerConditions = metadata.TriggerConditions.ToList(),
            DurationOrExpiryHint = metadata.DurationOrExpiryHint,
            SaveOrCheckReferences = metadata.SaveOrCheckReferences.ToList()
        };
    }

    private static bool TryParseDecimal(string value, out decimal number)
    {
        return decimal.TryParse(value?.Trim(), out number);
    }

    private static bool TryParseNullableInt(string value, out int? number)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            number = null;
            return true;
        }

        if (int.TryParse(value.Trim(), out var parsed))
        {
            number = parsed;
            return true;
        }

        number = null;
        return false;
    }

    private static bool TryParseBool(string value, out bool parsed)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            parsed = false;
            return true;
        }

        if (bool.TryParse(value.Trim(), out parsed))
        {
            return true;
        }

        if (string.Equals(value.Trim(), "yes", StringComparison.OrdinalIgnoreCase))
        {
            parsed = true;
            return true;
        }

        if (string.Equals(value.Trim(), "no", StringComparison.OrdinalIgnoreCase))
        {
            parsed = false;
            return true;
        }

        parsed = false;
        return false;
    }
}
