using Microsoft.AspNetCore.Components;
using MIMLESvtt.Services;

namespace MIMLESvtt.Components.Pages;

public partial class ContentImportWorkflowShell
{
    [Inject] public ContentImportWorkflowService ContentImportWorkflowService { get; set; } = default!;

    private ContentImportStage currentStage = ContentImportStage.Upload;
    private string selectedFilePath = string.Empty;
    private string importType = "Monster";
    private ContentImportPreviewResult resultState = new(0, 0, 0, 0, 0, [], []);
    private ContentImportDuplicatePolicy duplicatePolicy = ContentImportDuplicatePolicy.RejectDuplicate;
    private DateTimeOffset? lastRunUtc;
    private int validationRunCount;
    private readonly List<StageHistoryEntry> stageHistory = [];

    private ContentImportStage CurrentStage => currentStage;

    private IReadOnlyList<StageHistoryEntry> StageHistory => stageHistory;

    private int StageHistoryCount => stageHistory.Count;

    private string LastRunDisplay => lastRunUtc is null ? "(not run)" : lastRunUtc.Value.ToString("u");

    private int ErrorCount => resultState.Issues.Count(i => i.Severity == ContentImportIssueSeverity.Error);

    private int WarningCount => resultState.Issues.Count(i => i.Severity == ContentImportIssueSeverity.Warning);

    private bool HasBlockingErrors => ErrorCount > 0;

    private bool HasAnyIssues => resultState.Issues.Count > 0;

    private string CurrentStageGuidance => currentStage switch
    {
        ContentImportStage.Upload => "Configure source and run validation preview.",
        ContentImportStage.Validate => "Review validation output and continue based on issue severity.",
        ContentImportStage.Persist => "Inspect blocking and warning issues before restarting from Upload.",
        ContentImportStage.Summarize => "Review totals and decide whether to run another import cycle.",
        _ => string.Empty
    };

    private void SetStage(ContentImportStage stage)
    {
        currentStage = stage;
        RecordStageHistory($"Stage set to {stage}.");
    }

    private void ValidateImportPreview()
    {
        var request = BuildRequest();
        var workflowService = ContentImportWorkflowService ?? new ContentImportWorkflowService();
        resultState = workflowService.RunPreview(request);
        currentStage = ContentImportStage.Validate;
        validationRunCount++;
        lastRunUtc = DateTimeOffset.UtcNow;
        RecordStageHistory($"Validation run {validationRunCount} completed. Errors={ErrorCount}, Warnings={WarningCount}.");
    }

    private void ContinueFromValidation()
    {
        if (resultState.Issues.Any(i => i.Severity == ContentImportIssueSeverity.Error))
        {
            currentStage = ContentImportStage.Persist;
            RecordStageHistory("Continue moved to Persist due to blocking errors.");
            return;
        }

        currentStage = ContentImportStage.Summarize;
        RecordStageHistory("Continue moved to Summary.");
    }

    private void RetryFromErrors()
    {
        currentStage = ContentImportStage.Upload;
        RecordStageHistory("Workflow reset to Upload.");
    }

    private void ClearStageHistory()
    {
        stageHistory.Clear();
    }

    private string GetStageBadgeClass(ContentImportStage stage)
    {
        return stage switch
        {
            ContentImportStage.Upload => "text-bg-secondary",
            ContentImportStage.Validate => "text-bg-info",
            ContentImportStage.Persist => "text-bg-warning",
            ContentImportStage.Summarize => "text-bg-success",
            _ => "text-bg-secondary"
        };
    }

    private void RecordStageHistory(string message)
    {
        stageHistory.Insert(0, new StageHistoryEntry(DateTimeOffset.UtcNow, currentStage, message));

        if (stageHistory.Count > 12)
        {
            stageHistory.RemoveRange(12, stageHistory.Count - 12);
        }
    }

    private ContentImportPreviewRequest BuildRequest()
    {
        return new ContentImportPreviewRequest(
            FileName: string.IsNullOrWhiteSpace(selectedFilePath) ? "content-import.csv" : selectedFilePath,
            FileContent: BuildMockCsvByType(importType),
            Category: ParseCategory(importType),
            DuplicatePolicy: duplicatePolicy,
            OperatorContext: "ContentImportWorkflowShell");
    }

    private static ContentImportCategory ParseCategory(string type)
    {
        return type switch
        {
            "Treasure" => ContentImportCategory.Treasure,
            "Equipment" => ContentImportCategory.Equipment,
            "MagicItem" => ContentImportCategory.MagicItem,
            "UnitCounter" => ContentImportCategory.UnitCounter,
            _ => ContentImportCategory.Monster
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
    internal void TestSetDuplicatePolicy(ContentImportDuplicatePolicy policy) => duplicatePolicy = policy;
    internal void TestValidateImportPreview() => ValidateImportPreview();
    internal void TestContinueFromValidation() => ContinueFromValidation();
    internal void TestRetryFromErrors() => RetryFromErrors();
    internal string TestCurrentStage => currentStage.ToString();
    internal ContentImportPreviewResult TestResultState => resultState;

    private sealed record StageHistoryEntry(
        DateTimeOffset TimestampUtc,
        ContentImportStage Stage,
        string Message);
}
