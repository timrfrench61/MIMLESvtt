namespace MIMLESvtt.src.Domain.Models.Content;

public static class EquipmentContentMapper
{
    public static (EquipmentContentEntry? entry, ContentValidationResult validation) FromCsvRow(EquipmentCsvImportRow row)
    {
        var validation = new ContentValidationResult();
        if (row is null)
        {
            validation.Errors.Add("Equipment CSV row is required.");
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

        if (!string.Equals(row.Category?.Trim(), "Equipment", StringComparison.OrdinalIgnoreCase))
        {
            validation.Errors.Add("Category must be Equipment.");
        }

        if (string.IsNullOrWhiteSpace(row.Type))
        {
            validation.Errors.Add("Type is required.");
        }

        if (!TryParseDecimal(row.Value, out var value))
        {
            validation.Errors.Add("Value must parse as numeric.");
        }

        if (!TryParseDecimal(row.Weight, out var weight))
        {
            validation.Errors.Add("Weight must parse as numeric.");
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
        var weightUnit = string.IsNullOrWhiteSpace(row.WeightUnit) ? "lb" : row.WeightUnit.Trim();

        var entry = new EquipmentContentEntry
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
            EquipmentType = type,
            EquipmentCategory = category,
            BaseCost = value,
            Weight = weight,
            WeightUnit = weightUnit,
            Description = row.Description?.Trim() ?? string.Empty
        };

        return (entry, validation);
    }

    public static EquipmentListItemDto ToListItemDto(EquipmentContentEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        return new EquipmentListItemDto
        {
            Id = entry.Metadata.Id,
            Name = entry.Metadata.Name,
            EquipmentType = entry.EquipmentType,
            BaseCost = entry.BaseCost,
            Weight = entry.Weight,
            WeightUnit = entry.WeightUnit
        };
    }

    public static EquipmentDetailDto ToDetailDto(EquipmentContentEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        return new EquipmentDetailDto
        {
            Id = entry.Metadata.Id,
            Name = entry.Metadata.Name,
            EquipmentType = entry.EquipmentType,
            EquipmentCategory = entry.EquipmentCategory,
            BaseCost = entry.BaseCost,
            Weight = entry.Weight,
            WeightUnit = entry.WeightUnit,
            Description = entry.Description,
            Source = entry.Metadata.Source,
            Tags = entry.Metadata.Tags.ToList(),
            Extensions = new Dictionary<string, object>(entry.Metadata.Extensions, StringComparer.Ordinal)
        };
    }

    public static EquipmentContentEntry FromEditDto(EquipmentEditDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new EquipmentContentEntry
        {
            Metadata = new ContentMetadata
            {
                Id = dto.Id,
                Name = dto.Name,
                CategoryType = dto.EquipmentCategory,
                Source = dto.Source,
                Tags = dto.Tags.ToList(),
                Extensions = new Dictionary<string, object>(dto.Extensions, StringComparer.Ordinal)
            },
            EquipmentType = dto.EquipmentType,
            EquipmentCategory = dto.EquipmentCategory,
            BaseCost = dto.BaseCost,
            Weight = dto.Weight,
            WeightUnit = dto.WeightUnit,
            Description = dto.Description
        };
    }

    private static bool TryParseDecimal(string value, out decimal number)
    {
        return decimal.TryParse(value?.Trim(), out number);
    }
}
