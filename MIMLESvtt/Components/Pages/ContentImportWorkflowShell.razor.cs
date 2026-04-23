using Microsoft.AspNetCore.Components;

namespace MIMLESvtt.Components.Pages;

public partial class ContentImportWorkflowShell
{
    private ImportStage currentStage = ImportStage.Upload;
    private string selectedFilePath = string.Empty;
    private string importType = "Monster";
    private ContentImportResultState resultState = new();

    private ImportStage CurrentStage => currentStage;

    private void SetStage(ImportStage stage)
    {
        currentStage = stage;
    }

    private void ValidateImportPreview()
    {
        resultState = BuildMockResultForType(importType);
        currentStage = ImportStage.Validation;
    }

    private void ContinueFromValidation()
    {
        if (resultState.ErrorCount > 0)
        {
            currentStage = ImportStage.ErrorReview;
            return;
        }

        currentStage = ImportStage.SuccessSummary;
    }

    private void RetryFromErrors()
    {
        currentStage = ImportStage.Upload;
    }

    private static ContentImportResultState BuildMockResultForType(string type)
    {
        return type switch
        {
            "Equipment" => new ContentImportResultState
            {
                TotalRows = 4,
                CreatedCount = 3,
                UpdatedCount = 0,
                SkippedCount = 0,
                FailedCount = 1,
                WarningCount = 1,
                ErrorCount = 1,
                ValidationMessages =
                [
                    "Row 4: Weight must be 0 or greater.",
                    "Row 2: Category normalized to known value."
                ]
            },
            "MagicItem" => new ContentImportResultState
            {
                TotalRows = 3,
                CreatedCount = 3,
                UpdatedCount = 0,
                SkippedCount = 0,
                FailedCount = 0,
                WarningCount = 0,
                ErrorCount = 0,
                ValidationMessages =
                [
                    "Validation passed."
                ]
            },
            "Treasure" => new ContentImportResultState
            {
                TotalRows = 2,
                CreatedCount = 2,
                UpdatedCount = 0,
                SkippedCount = 0,
                FailedCount = 0,
                WarningCount = 0,
                ErrorCount = 0,
                ValidationMessages =
                [
                    "Validation passed."
                ]
            },
            _ => new ContentImportResultState
            {
                TotalRows = 2,
                CreatedCount = 2,
                UpdatedCount = 0,
                SkippedCount = 0,
                FailedCount = 0,
                WarningCount = 0,
                ErrorCount = 0,
                ValidationMessages =
                [
                    "Validation passed."
                ]
            }
        };
    }

    internal void TestSetImportType(string value) => importType = value;
    internal void TestValidateImportPreview() => ValidateImportPreview();
    internal void TestContinueFromValidation() => ContinueFromValidation();
    internal void TestRetryFromErrors() => RetryFromErrors();
    internal string TestCurrentStage => currentStage.ToString();
    internal ContentImportResultState TestResultState => resultState;

    private enum ImportStage
    {
        Upload,
        Validation,
        ErrorReview,
        SuccessSummary
    }

    internal class ContentImportResultState
    {
        public int TotalRows { get; set; }
        public int CreatedCount { get; set; }
        public int UpdatedCount { get; set; }
        public int SkippedCount { get; set; }
        public int FailedCount { get; set; }
        public int WarningCount { get; set; }
        public int ErrorCount { get; set; }
        public List<string> ValidationMessages { get; set; } = [];
    }
}
