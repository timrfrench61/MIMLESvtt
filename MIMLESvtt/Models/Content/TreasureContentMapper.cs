namespace MIMLESvtt.src.Domain.Models.Content;

public static class TreasureContentMapper
{
    public static (TreasureContentEntry? entry, ContentValidationResult validation) FromCsvRow(TreasureCsvImportRow row)
    {
        var validation = new ContentValidationResult();
        if (row is null)
        {
            validation.Errors.Add("Treasure CSV row is required.");
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

        if (!string.Equals(row.Category?.Trim(), "Treasure", StringComparison.OrdinalIgnoreCase))
        {
            validation.Errors.Add("Category must be Treasure.");
        }

        if (string.IsNullOrWhiteSpace(row.TreasureType))
        {
            validation.Errors.Add("TreasureType is required.");
        }

        if (!TryParseDecimal(row.Value, out var value))
        {
            validation.Errors.Add("Value must parse as numeric.");
        }

        if (!TryParseInt(row.Quantity, out var quantity))
        {
            validation.Errors.Add("Quantity must parse as integer.");
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
        var treasureType = row.TreasureType?.Trim() ?? string.Empty;
        var source = string.IsNullOrWhiteSpace(row.Source) ? "Import" : row.Source.Trim();
        var currency = string.IsNullOrWhiteSpace(row.Currency) ? "gp" : row.Currency.Trim();

        var entry = new TreasureContentEntry
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
            TreasureType = treasureType,
            BaseValue = value,
            CurrencyOrValueUnit = currency,
            Quantity = quantity,
            Description = row.Description?.Trim() ?? string.Empty,
            EncounterReferenceId = row.EncounterReferenceId?.Trim() ?? string.Empty,
            ScenarioReferenceId = row.ScenarioReferenceId?.Trim() ?? string.Empty,
            PlacementContainerReferenceId = row.PlacementContainerReferenceId?.Trim() ?? string.Empty,
            Components = []
        };

        return (entry, validation);
    }

    private static bool TryParseDecimal(string value, out decimal number)
    {
        return decimal.TryParse(value?.Trim(), out number);
    }

    private static bool TryParseInt(string value, out int number)
    {
        return int.TryParse(value?.Trim(), out number);
    }
}
