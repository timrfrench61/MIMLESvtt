using Microsoft.AspNetCore.Components;
using MIMLESvtt.src.Domain.Models.Content;
using MIMLESvtt.src.Domain.Models.Content.Import;

namespace MIMLESvtt.Components.Pages;

public partial class ContentImportWorkflowShell
{
    private CsvImportStage currentStage = CsvImportStage.Upload;
    private string selectedFilePath = string.Empty;
    private string importType = "Monster";
    private CsvImportResult resultState = new();
    private CsvDuplicateHandlingPolicy duplicatePolicy = CsvDuplicateHandlingPolicy.RejectDuplicate;

    private CsvImportStage CurrentStage => currentStage;

    private void SetStage(CsvImportStage stage)
    {
        currentStage = stage;
    }

    private void ValidateImportPreview()
    {
        var request = BuildRequest();
        var pipeline = BuildPipeline();
        resultState = pipeline.Run(request);
        currentStage = CsvImportStage.Validate;
    }

    private void ContinueFromValidation()
    {
        if (resultState.Issues.Any(i => i.Severity == CsvImportIssueSeverity.Error))
        {
            currentStage = CsvImportStage.Persist;
            return;
        }

        currentStage = CsvImportStage.Summarize;
    }

    private void RetryFromErrors()
    {
        currentStage = CsvImportStage.Upload;
    }

    private CsvImportPipelineService BuildPipeline()
    {
        return new CsvImportPipelineService(new InMemoryContentRepository(),
        [
            new MonsterCsvImportAdapter(),
            new TreasureCsvImportAdapter(),
            new EquipmentCsvImportAdapter(),
            new MagicItemCsvImportAdapter(),
            new UnitCounterCsvImportAdapter()
        ]);
    }

    private CsvImportRequest BuildRequest()
    {
        return new CsvImportRequest
        {
            FileName = string.IsNullOrWhiteSpace(selectedFilePath) ? "content-import.csv" : selectedFilePath,
            FileContent = BuildMockCsvByType(importType),
            Category = ParseCategory(importType),
            DuplicatePolicy = duplicatePolicy,
            OperatorContext = "ContentImportWorkflowShell"
        };
    }

    private static ContentCategory ParseCategory(string type)
    {
        return type switch
        {
            "Treasure" => ContentCategory.Treasure,
            "Equipment" => ContentCategory.Equipment,
            "MagicItem" => ContentCategory.MagicItem,
            "UnitCounter" => ContentCategory.UnitCounter,
            _ => ContentCategory.Monster
        };
    }

    private static string BuildMockCsvByType(string type)
    {
        return type switch
        {
            "Equipment" => "DefinitionId,Name,Category,Type,Value,Weight,WeightUnit\nEQ100,Longsword,Equipment,Weapon,15,3,lb\nEQ100,Shield,Equipment,Armor,10,6,lb\nEQ102,Bad Weight,Equipment,Armor,10,-1,lb",
            "MagicItem" => "DefinitionId,Name,Category,Type,Rarity,Value,AttunementRequired,Charges,Description\nMI100,Ring of Guard,MagicItem,Ring,Rare,1500,true,,Protective ring\nMI101,Wand Spark,MagicItem,Wand,Uncommon,500,false,3,Arcane spark",
            "Treasure" => "DefinitionId,Name,Category,TreasureType,Value,Currency,Quantity\nTR100,Cache,Treasure,Coin,50,gp,1\nTR101,Gems,Treasure,Bundle,75,gp,3",
            "UnitCounter" => "DefinitionId,Name,Category,UnitType,Side,Faction,StrengthOrValue,Movement,DefenseOrArmor,RangeOrReach\nUC100,Infantry,UnitCounter,Infantry,Blue,Northern,4,3,2,1",
            _ => "DefinitionId,Name,Category,LevelOrThreat,HitPoints,Movement,ArmorClass,AttackProfile\nMON100,Goblin,Monster,1,5,9,6,Scimitar"
        };
    }

    internal void TestSetImportType(string value) => importType = value;
    internal void TestSetDuplicatePolicy(CsvDuplicateHandlingPolicy policy) => duplicatePolicy = policy;
    internal void TestValidateImportPreview() => ValidateImportPreview();
    internal void TestContinueFromValidation() => ContinueFromValidation();
    internal void TestRetryFromErrors() => RetryFromErrors();
    internal string TestCurrentStage => currentStage.ToString();
    internal CsvImportResult TestResultState => resultState;
}
