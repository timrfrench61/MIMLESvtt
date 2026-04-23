using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.Components.Pages;
using MIMLESvtt.src.Domain.Models.Content;
using MIMLESvtt.src.Domain.Models.Content.Import;

namespace MIMLESvtt.Tests;

[TestClass]
public class ContentImportWorkflowShellTests
{
    [TestMethod]
    public void ValidationStage_TransitionsToSuccessSummary_WhenNoErrors()
    {
        var shell = new ContentImportWorkflowShell();

        shell.TestSetImportType("MagicItem");
        shell.TestSetDuplicatePolicy(MIMLESvtt.src.Domain.Models.Content.Import.CsvDuplicateHandlingPolicy.RejectDuplicate);
        shell.TestValidateImportPreview();
        shell.TestContinueFromValidation();

        Assert.AreEqual("Summarize", shell.TestCurrentStage);
        Assert.AreEqual(0, shell.TestResultState.Issues.Count(i => i.Severity == MIMLESvtt.src.Domain.Models.Content.Import.CsvImportIssueSeverity.Error));
    }

    [TestMethod]
    public void ValidationStage_TransitionsToErrorReview_WhenErrorsExist()
    {
        var shell = new ContentImportWorkflowShell();

        shell.TestSetImportType("Equipment");
        shell.TestSetDuplicatePolicy(MIMLESvtt.src.Domain.Models.Content.Import.CsvDuplicateHandlingPolicy.RejectDuplicate);
        shell.TestValidateImportPreview();
        shell.TestContinueFromValidation();

        Assert.AreEqual("Persist", shell.TestCurrentStage);
        Assert.IsTrue(shell.TestResultState.Issues.Any(i => i.Severity == MIMLESvtt.src.Domain.Models.Content.Import.CsvImportIssueSeverity.Error));
    }

    [TestMethod]
    public void ErrorReview_Retry_ReturnsToUploadStage()
    {
        var shell = new ContentImportWorkflowShell();

        shell.TestSetImportType("Equipment");
        shell.TestSetDuplicatePolicy(MIMLESvtt.src.Domain.Models.Content.Import.CsvDuplicateHandlingPolicy.RejectDuplicate);
        shell.TestValidateImportPreview();
        shell.TestContinueFromValidation();
        shell.TestRetryFromErrors();

        Assert.AreEqual("Upload", shell.TestCurrentStage);
    }

    [TestMethod]
    public void ValidationStage_SkipDuplicatePolicy_ProducesWarningAndSummaryProgression()
    {
        var shell = new ContentImportWorkflowShell();

        shell.TestSetImportType("Equipment");
        shell.TestSetDuplicatePolicy(MIMLESvtt.src.Domain.Models.Content.Import.CsvDuplicateHandlingPolicy.SkipDuplicate);
        shell.TestValidateImportPreview();
        shell.TestContinueFromValidation();

        Assert.AreEqual("Persist", shell.TestCurrentStage);
        Assert.IsTrue(shell.TestResultState.Issues.Any(i => i.Severity == MIMLESvtt.src.Domain.Models.Content.Import.CsvImportIssueSeverity.Warning));
    }

    [TestMethod]
    public void Pipeline_RejectDuplicatePolicy_ProducesErrorAndFailedCount()
    {
        var repository = new InMemoryContentRepository();
        repository.Save(new MonsterContentEntry
        {
            Metadata = new ContentMetadata
            {
                Id = "MON100",
                Name = "Existing Goblin",
                CategoryType = "Monster",
                Source = "Manual"
            },
            MonsterCategory = "Humanoid",
            LevelOrThreat = 1,
            HitPoints = 5,
            Movement = 9,
            ArmorOrDefense = 6,
            AttackProfile = "Scimitar"
        });

        var pipeline = new CsvImportPipelineService(repository,
        [
            new MonsterCsvImportAdapter()
        ]);

        var result = pipeline.Run(new CsvImportRequest
        {
            FileName = "monsters.csv",
            FileContent = "DefinitionId,Name,Category,LevelOrThreat,HitPoints,Movement,ArmorClass,AttackProfile\nMON100,Goblin,Monster,1,5,9,6,Scimitar",
            Category = ContentCategory.Monster,
            DuplicatePolicy = CsvDuplicateHandlingPolicy.RejectDuplicate
        });

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(1, result.FailedCount);
        Assert.IsTrue(result.Issues.Any(i => i.Severity == CsvImportIssueSeverity.Error && i.Field == "Id"));
    }

    [TestMethod]
    public void Pipeline_SkipDuplicatePolicy_ProducesWarningAndSkippedCount()
    {
        var repository = new InMemoryContentRepository();
        repository.Save(new MonsterContentEntry
        {
            Metadata = new ContentMetadata
            {
                Id = "MON100",
                Name = "Existing Goblin",
                CategoryType = "Monster",
                Source = "Manual"
            },
            MonsterCategory = "Humanoid",
            LevelOrThreat = 1,
            HitPoints = 5,
            Movement = 9,
            ArmorOrDefense = 6,
            AttackProfile = "Scimitar"
        });

        var pipeline = new CsvImportPipelineService(repository,
        [
            new MonsterCsvImportAdapter()
        ]);

        var result = pipeline.Run(new CsvImportRequest
        {
            FileName = "monsters.csv",
            FileContent = "DefinitionId,Name,Category,LevelOrThreat,HitPoints,Movement,ArmorClass,AttackProfile\nMON100,Goblin,Monster,1,5,9,6,Scimitar",
            Category = ContentCategory.Monster,
            DuplicatePolicy = CsvDuplicateHandlingPolicy.SkipDuplicate
        });

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(1, result.SkippedCount);
        Assert.IsTrue(result.Issues.Any(i => i.Severity == CsvImportIssueSeverity.Warning && i.Field == "Id"));
    }

    [TestMethod]
    public void Pipeline_UpdateDuplicatePolicy_ProducesInfoAndUpdatedCount()
    {
        var repository = new InMemoryContentRepository();
        repository.Save(new MonsterContentEntry
        {
            Metadata = new ContentMetadata
            {
                Id = "MON100",
                Name = "Existing Goblin",
                CategoryType = "Monster",
                Source = "Manual"
            },
            MonsterCategory = "Humanoid",
            LevelOrThreat = 1,
            HitPoints = 5,
            Movement = 9,
            ArmorOrDefense = 6,
            AttackProfile = "Scimitar"
        });

        var pipeline = new CsvImportPipelineService(repository,
        [
            new MonsterCsvImportAdapter()
        ]);

        var result = pipeline.Run(new CsvImportRequest
        {
            FileName = "monsters.csv",
            FileContent = "DefinitionId,Name,Category,LevelOrThreat,HitPoints,Movement,ArmorClass,AttackProfile\nMON100,Goblin Veteran,Monster,2,8,9,6,Scimitar +1",
            Category = ContentCategory.Monster,
            DuplicatePolicy = CsvDuplicateHandlingPolicy.UpdateDuplicate
        });

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(1, result.UpdatedCount);
        Assert.IsTrue(result.Issues.Any(i => i.Severity == CsvImportIssueSeverity.Info && i.Field == "Id"));
    }

    [TestMethod]
    public void Pipeline_RowAndFileLevelIssues_AreClassifiedConsistently()
    {
        var repository = new InMemoryContentRepository();
        var pipeline = new CsvImportPipelineService(repository,
        [
            new MonsterCsvImportAdapter()
        ]);

        var result = pipeline.Run(new CsvImportRequest
        {
            FileName = "monsters.csv",
            FileContent = "DefinitionId,Name\nMON100,Goblin",
            Category = ContentCategory.Monster,
            DuplicatePolicy = CsvDuplicateHandlingPolicy.RejectDuplicate
        });

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(i => i.ValidationLevel == CsvImportValidationLevel.File && i.Field == "Category"));
        Assert.IsTrue(result.Issues.Any(i => i.ValidationLevel == CsvImportValidationLevel.Row));
    }
}
