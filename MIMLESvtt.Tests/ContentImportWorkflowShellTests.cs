using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.Components.Pages;

namespace MIMLESvtt.Tests;

[TestClass]
public class ContentImportWorkflowShellTests
{
    [TestMethod]
    public void ValidationStage_TransitionsToSuccessSummary_WhenNoErrors()
    {
        var shell = new ContentImportWorkflowShell();

        shell.TestSetImportType("MagicItem");
        shell.TestValidateImportPreview();
        shell.TestContinueFromValidation();

        Assert.AreEqual("SuccessSummary", shell.TestCurrentStage);
        Assert.AreEqual(0, shell.TestResultState.ErrorCount);
    }

    [TestMethod]
    public void ValidationStage_TransitionsToErrorReview_WhenErrorsExist()
    {
        var shell = new ContentImportWorkflowShell();

        shell.TestSetImportType("Equipment");
        shell.TestValidateImportPreview();
        shell.TestContinueFromValidation();

        Assert.AreEqual("ErrorReview", shell.TestCurrentStage);
        Assert.IsTrue(shell.TestResultState.ErrorCount > 0);
    }

    [TestMethod]
    public void ErrorReview_Retry_ReturnsToUploadStage()
    {
        var shell = new ContentImportWorkflowShell();

        shell.TestSetImportType("Equipment");
        shell.TestValidateImportPreview();
        shell.TestContinueFromValidation();
        shell.TestRetryFromErrors();

        Assert.AreEqual("Upload", shell.TestCurrentStage);
    }
}
