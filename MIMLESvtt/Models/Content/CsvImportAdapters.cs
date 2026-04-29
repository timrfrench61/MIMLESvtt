namespace MIMLESvtt.src.Domain.Models.Content.Import;

public class MonsterCsvImportAdapter : ICsvImportAdapter
{
    public ContentCategory Category => ContentCategory.Monster;

    public bool CanHandle(ContentCategory category) => category == Category;

    public ContentEntry? MapRow(IReadOnlyDictionary<string, string> row, int rowIndex, List<CsvImportIssue> issues)
    {
        CsvImportValidationUtilities.ValidateRequiredField(row, "DefinitionId", rowIndex, CsvImportStage.Validate, issues);
        CsvImportValidationUtilities.ValidateRequiredField(row, "Name", rowIndex, CsvImportStage.Validate, issues);
        CsvImportValidationUtilities.ValidateRequiredField(row, "Category", rowIndex, CsvImportStage.Validate, issues);
        CsvImportValidationUtilities.ValidateOptionalFieldType(row, "LevelOrThreat", CsvImportFieldType.Integer, rowIndex, CsvImportStage.Validate, issues);
        CsvImportValidationUtilities.ValidateOptionalFieldType(row, "HitPoints", CsvImportFieldType.Integer, rowIndex, CsvImportStage.Validate, issues);
        CsvImportValidationUtilities.ValidateOptionalFieldType(row, "Movement", CsvImportFieldType.Decimal, rowIndex, CsvImportStage.Validate, issues);
        CsvImportValidationUtilities.ValidateOptionalFieldType(row, "ArmorClass", CsvImportFieldType.Integer, rowIndex, CsvImportStage.Validate, issues);
        CsvImportValidationUtilities.ValidateOptionalExtensions(row, rowIndex, CsvImportStage.Validate, issues);

        if (issues.Any(i => i.RowIndex == rowIndex && i.Stage == CsvImportStage.Validate && i.Severity == CsvImportIssueSeverity.Error))
        {
            return null;
        }

        var mapped = MonsterContentMapper.FromCsvRow(new MonsterCsvImportRow
        {
            DefinitionId = GetValue(row, "DefinitionId"),
            Name = GetValue(row, "Name"),
            Category = GetValue(row, "Category"),
            LevelOrThreat = GetValue(row, "LevelOrThreat"),
            HitPoints = GetValue(row, "HitPoints"),
            Movement = GetValue(row, "Movement"),
            ArmorClass = GetValue(row, "ArmorClass"),
            AttackProfile = GetValue(row, "AttackProfile"),
            Source = GetValue(row, "Source"),
            Tags = GetValue(row, "Tags")
        });

        AddValidationIssues(mapped.validation, CsvImportStage.Map, rowIndex, issues);
        return mapped.entry;
    }

    private static string GetValue(IReadOnlyDictionary<string, string> row, string key)
        => row.TryGetValue(key, out var value) ? value : string.Empty;

    private static void AddValidationIssues(ContentValidationResult validation, CsvImportStage stage, int rowIndex, List<CsvImportIssue> issues)
    {
        foreach (var error in validation.Errors)
        {
            issues.Add(new CsvImportIssue
            {
                Severity = CsvImportIssueSeverity.Error,
                ValidationLevel = CsvImportValidationLevel.Row,
                Stage = stage,
                RowIndex = rowIndex,
                Message = error
            });
        }
    }
}

public class TreasureCsvImportAdapter : ICsvImportAdapter
{
    public ContentCategory Category => ContentCategory.Treasure;

    public bool CanHandle(ContentCategory category) => category == Category;

    public ContentEntry? MapRow(IReadOnlyDictionary<string, string> row, int rowIndex, List<CsvImportIssue> issues)
    {
        CsvImportValidationUtilities.ValidateRequiredField(row, "DefinitionId", rowIndex, CsvImportStage.Validate, issues);
        CsvImportValidationUtilities.ValidateRequiredField(row, "Name", rowIndex, CsvImportStage.Validate, issues);
        CsvImportValidationUtilities.ValidateRequiredField(row, "Category", rowIndex, CsvImportStage.Validate, issues);
        CsvImportValidationUtilities.ValidateOptionalFieldType(row, "Value", CsvImportFieldType.Decimal, rowIndex, CsvImportStage.Validate, issues);
        CsvImportValidationUtilities.ValidateOptionalFieldType(row, "Quantity", CsvImportFieldType.Integer, rowIndex, CsvImportStage.Validate, issues);
        CsvImportValidationUtilities.ValidateOptionalExtensions(row, rowIndex, CsvImportStage.Validate, issues);

        if (issues.Any(i => i.RowIndex == rowIndex && i.Stage == CsvImportStage.Validate && i.Severity == CsvImportIssueSeverity.Error))
        {
            return null;
        }

        var mapped = TreasureContentMapper.FromCsvRow(new TreasureCsvImportRow
        {
            DefinitionId = GetValue(row, "DefinitionId"),
            Name = GetValue(row, "Name"),
            Category = GetValue(row, "Category"),
            TreasureType = GetValue(row, "TreasureType"),
            Value = GetValue(row, "Value"),
            Currency = GetValue(row, "Currency"),
            Quantity = GetValue(row, "Quantity"),
            Description = GetValue(row, "Description"),
            Source = GetValue(row, "Source"),
            Tags = GetValue(row, "Tags"),
            EncounterReferenceId = GetValue(row, "EncounterReferenceId"),
            ScenarioReferenceId = GetValue(row, "ScenarioReferenceId"),
            PlacementContainerReferenceId = GetValue(row, "PlacementContainerReferenceId")
        });

        AddValidationIssues(mapped.validation, CsvImportStage.Map, rowIndex, issues);
        return mapped.entry;
    }

    private static string GetValue(IReadOnlyDictionary<string, string> row, string key)
        => row.TryGetValue(key, out var value) ? value : string.Empty;

    private static void AddValidationIssues(ContentValidationResult validation, CsvImportStage stage, int rowIndex, List<CsvImportIssue> issues)
    {
        foreach (var error in validation.Errors)
        {
            issues.Add(new CsvImportIssue
            {
                Severity = CsvImportIssueSeverity.Error,
                ValidationLevel = CsvImportValidationLevel.Row,
                Stage = stage,
                RowIndex = rowIndex,
                Message = error
            });
        }
    }
}

public class EquipmentCsvImportAdapter : ICsvImportAdapter
{
    public ContentCategory Category => ContentCategory.Equipment;

    public bool CanHandle(ContentCategory category) => category == Category;

    public ContentEntry? MapRow(IReadOnlyDictionary<string, string> row, int rowIndex, List<CsvImportIssue> issues)
    {
        CsvImportValidationUtilities.ValidateRequiredField(row, "DefinitionId", rowIndex, CsvImportStage.Validate, issues);
        CsvImportValidationUtilities.ValidateRequiredField(row, "Name", rowIndex, CsvImportStage.Validate, issues);
        CsvImportValidationUtilities.ValidateRequiredField(row, "Category", rowIndex, CsvImportStage.Validate, issues);
        CsvImportValidationUtilities.ValidateOptionalFieldType(row, "Value", CsvImportFieldType.Decimal, rowIndex, CsvImportStage.Validate, issues);
        CsvImportValidationUtilities.ValidateOptionalFieldType(row, "Weight", CsvImportFieldType.Decimal, rowIndex, CsvImportStage.Validate, issues);
        CsvImportValidationUtilities.ValidateOptionalExtensions(row, rowIndex, CsvImportStage.Validate, issues);

        if (issues.Any(i => i.RowIndex == rowIndex && i.Stage == CsvImportStage.Validate && i.Severity == CsvImportIssueSeverity.Error))
        {
            return null;
        }

        var mapped = EquipmentContentMapper.FromCsvRow(new EquipmentCsvImportRow
        {
            DefinitionId = GetValue(row, "DefinitionId"),
            Name = GetValue(row, "Name"),
            Category = GetValue(row, "Category"),
            Type = GetValue(row, "Type"),
            Value = GetValue(row, "Value"),
            Weight = GetValue(row, "Weight"),
            WeightUnit = GetValue(row, "WeightUnit"),
            Description = GetValue(row, "Description"),
            Source = GetValue(row, "Source"),
            Tags = GetValue(row, "Tags")
        });

        AddValidationIssues(mapped.validation, CsvImportStage.Map, rowIndex, issues);
        return mapped.entry;
    }

    private static string GetValue(IReadOnlyDictionary<string, string> row, string key)
        => row.TryGetValue(key, out var value) ? value : string.Empty;

    private static void AddValidationIssues(ContentValidationResult validation, CsvImportStage stage, int rowIndex, List<CsvImportIssue> issues)
    {
        foreach (var error in validation.Errors)
        {
            issues.Add(new CsvImportIssue
            {
                Severity = CsvImportIssueSeverity.Error,
                ValidationLevel = CsvImportValidationLevel.Row,
                Stage = stage,
                RowIndex = rowIndex,
                Message = error
            });
        }
    }
}

public class MagicItemCsvImportAdapter : ICsvImportAdapter
{
    public ContentCategory Category => ContentCategory.MagicItem;

    public bool CanHandle(ContentCategory category) => category == Category;

    public ContentEntry? MapRow(IReadOnlyDictionary<string, string> row, int rowIndex, List<CsvImportIssue> issues)
    {
        CsvImportValidationUtilities.ValidateRequiredField(row, "DefinitionId", rowIndex, CsvImportStage.Validate, issues);
        CsvImportValidationUtilities.ValidateRequiredField(row, "Name", rowIndex, CsvImportStage.Validate, issues);
        CsvImportValidationUtilities.ValidateRequiredField(row, "Category", rowIndex, CsvImportStage.Validate, issues);
        CsvImportValidationUtilities.ValidateOptionalFieldType(row, "Value", CsvImportFieldType.Decimal, rowIndex, CsvImportStage.Validate, issues);
        CsvImportValidationUtilities.ValidateOptionalFieldType(row, "Charges", CsvImportFieldType.Integer, rowIndex, CsvImportStage.Validate, issues);
        CsvImportValidationUtilities.ValidateOptionalFieldType(row, "AttunementRequired", CsvImportFieldType.Boolean, rowIndex, CsvImportStage.Validate, issues);
        CsvImportValidationUtilities.ValidateOptionalExtensions(row, rowIndex, CsvImportStage.Validate, issues);

        if (issues.Any(i => i.RowIndex == rowIndex && i.Stage == CsvImportStage.Validate && i.Severity == CsvImportIssueSeverity.Error))
        {
            return null;
        }

        var mapped = MagicItemContentMapper.FromCsvRow(new MagicItemCsvImportRow
        {
            DefinitionId = GetValue(row, "DefinitionId"),
            Name = GetValue(row, "Name"),
            Category = GetValue(row, "Category"),
            Type = GetValue(row, "Type"),
            Rarity = GetValue(row, "Rarity"),
            Value = GetValue(row, "Value"),
            AttunementRequired = GetValue(row, "AttunementRequired"),
            Charges = GetValue(row, "Charges"),
            Description = GetValue(row, "Description"),
            Source = GetValue(row, "Source"),
            Tags = GetValue(row, "Tags")
        });

        AddValidationIssues(mapped.validation, CsvImportStage.Map, rowIndex, issues);
        return mapped.entry;
    }

    private static string GetValue(IReadOnlyDictionary<string, string> row, string key)
        => row.TryGetValue(key, out var value) ? value : string.Empty;

    private static void AddValidationIssues(ContentValidationResult validation, CsvImportStage stage, int rowIndex, List<CsvImportIssue> issues)
    {
        foreach (var error in validation.Errors)
        {
            issues.Add(new CsvImportIssue
            {
                Severity = CsvImportIssueSeverity.Error,
                ValidationLevel = CsvImportValidationLevel.Row,
                Stage = stage,
                RowIndex = rowIndex,
                Message = error
            });
        }
    }
}

public class UnitCounterCsvImportAdapter : ICsvImportAdapter
{
    public ContentCategory Category => ContentCategory.UnitCounter;

    public bool CanHandle(ContentCategory category) => category == Category;

    public ContentEntry? MapRow(IReadOnlyDictionary<string, string> row, int rowIndex, List<CsvImportIssue> issues)
    {
        CsvImportValidationUtilities.ValidateRequiredField(row, "DefinitionId", rowIndex, CsvImportStage.Validate, issues);
        CsvImportValidationUtilities.ValidateRequiredField(row, "Name", rowIndex, CsvImportStage.Validate, issues);
        CsvImportValidationUtilities.ValidateRequiredField(row, "UnitType", rowIndex, CsvImportStage.Validate, issues);
        CsvImportValidationUtilities.ValidateRequiredField(row, "Side", rowIndex, CsvImportStage.Validate, issues);
        CsvImportValidationUtilities.ValidateOptionalFieldType(row, "StrengthOrValue", CsvImportFieldType.Decimal, rowIndex, CsvImportStage.Validate, issues);
        CsvImportValidationUtilities.ValidateOptionalFieldType(row, "Movement", CsvImportFieldType.Decimal, rowIndex, CsvImportStage.Validate, issues);
        CsvImportValidationUtilities.ValidateOptionalFieldType(row, "DefenseOrArmor", CsvImportFieldType.Decimal, rowIndex, CsvImportStage.Validate, issues);
        CsvImportValidationUtilities.ValidateOptionalFieldType(row, "RangeOrReach", CsvImportFieldType.Decimal, rowIndex, CsvImportStage.Validate, issues);
        CsvImportValidationUtilities.ValidateOptionalExtensions(row, rowIndex, CsvImportStage.Validate, issues);

        if (issues.Any(i => i.RowIndex == rowIndex && i.Stage == CsvImportStage.Validate && i.Severity == CsvImportIssueSeverity.Error))
        {
            return null;
        }

        var mapped = UnitCounterContentMapper.FromCsvRow(new UnitCounterCsvImportRow
        {
            DefinitionId = GetValue(row, "DefinitionId"),
            Name = GetValue(row, "Name"),
            Category = GetValue(row, "Category"),
            UnitType = GetValue(row, "UnitType"),
            Side = GetValue(row, "Side"),
            Faction = GetValue(row, "Faction"),
            StrengthOrValue = GetValue(row, "StrengthOrValue"),
            Movement = GetValue(row, "Movement"),
            DefenseOrArmor = GetValue(row, "DefenseOrArmor"),
            RangeOrReach = GetValue(row, "RangeOrReach"),
            Description = GetValue(row, "Description"),
            Source = GetValue(row, "Source"),
            Tags = GetValue(row, "Tags"),
            ScenarioMode = GetValue(row, "ScenarioMode")
        });

        AddValidationIssues(mapped.validation, CsvImportStage.Map, rowIndex, issues);
        return mapped.entry;
    }

    private static string GetValue(IReadOnlyDictionary<string, string> row, string key)
        => row.TryGetValue(key, out var value) ? value : string.Empty;

    private static void AddValidationIssues(ContentValidationResult validation, CsvImportStage stage, int rowIndex, List<CsvImportIssue> issues)
    {
        foreach (var error in validation.Errors)
        {
            issues.Add(new CsvImportIssue
            {
                Severity = CsvImportIssueSeverity.Error,
                ValidationLevel = CsvImportValidationLevel.Row,
                Stage = stage,
                RowIndex = rowIndex,
                Message = error
            });
        }
    }
}
