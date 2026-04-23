using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.Components.Pages;

namespace MIMLESvtt.Tests;

[TestClass]
public class ContentTreasurePageTests
{
    [TestMethod]
    public void ValidateEditor_WithNegativeValue_Fails()
    {
        var page = new ContentTreasure();

        page.TestSetCreateMode();
        page.TestSetEditorFields("TRS900", "Bad Value", "Coin", -1, 1);
        page.TestValidateEditorForm();

        Assert.IsFalse(page.TestEditorValidationPassed);
        Assert.AreEqual("Value must be 0 or greater.", page.TestEditorValidationMessage);
    }

    [TestMethod]
    public void SaveTreasure_InCreateMode_WithValidFields_AddsTreasureAndShowsSuccess()
    {
        var page = new ContentTreasure();

        page.TestSetCreateMode();
        page.TestSetEditorFields("TRS900", "Royal Cache", "Hoard", 1000, 1);
        page.TestSetComponentRows(("CMP001", "Gold Bar", "Bullion", 2, 500));
        page.TestSaveTreasure();

        Assert.IsTrue(page.TestEditorSaveSucceeded);
        Assert.AreEqual("Saved new treasure and returned to detail/list flow.", page.TestEditorSaveMessage);
        Assert.IsTrue(page.TestTreasureExists("TRS900", "Royal Cache"));
        Assert.AreEqual(1, page.TestSelectedTreasureComponentCount);
    }

    [TestMethod]
    public void SaveTreasure_InEditMode_UpdatesSelectedTreasureAndRetainsRoundTripSelection()
    {
        var page = new ContentTreasure();

        page.TestSelectTreasure("TRS001");
        page.TestSetEditorFields("TRS001", "Goblin Coin Hoard", "Coin", 40, 2);
        page.TestSetComponentRows(("CMP010", "Coin Bundle", "Coin", 2, 20));
        page.TestSaveTreasure();

        Assert.IsTrue(page.TestEditorSaveSucceeded);
        Assert.AreEqual("Saved treasure changes and returned to detail/list flow.", page.TestEditorSaveMessage);
        Assert.IsTrue(page.TestTreasureExists("TRS001", "Goblin Coin Hoard"));
        Assert.AreEqual("Goblin Coin Hoard", page.TestSelectedTreasureName);
        Assert.AreEqual(1, page.TestSelectedTreasureComponentCount);
    }

    [TestMethod]
    public void CancelEdit_InEditMode_RevertsEditorValuesToSelectedTreasure()
    {
        var page = new ContentTreasure();

        page.TestSelectTreasure("TRS002");
        page.TestSetEditorFields("TRS002", "Temp Treasure", "Temp", 1, 1);
        page.TestSetComponentRows(("TEMP", "Temp", "Temp", 1, 1));
        page.TestCancelEdit();

        Assert.IsTrue(page.TestEditorSaveSucceeded);
        Assert.AreEqual("Canceled edit changes. No mutation persisted; returned to detail/list flow.", page.TestEditorSaveMessage);
        Assert.AreEqual("Small Gem Cache", page.TestSelectedTreasureName);
    }

    [TestMethod]
    public void SaveTreasure_InCreateMode_WithDuplicateId_IsRejected()
    {
        var page = new ContentTreasure();

        page.TestSetCreateMode();
        page.TestSetEditorFields("TRS001", "Duplicate Treasure", "Coin", 10, 1);
        page.TestSaveTreasure();

        Assert.IsFalse(page.TestEditorSaveSucceeded);
        Assert.AreEqual("Save failed. Treasure id already exists.", page.TestEditorSaveMessage);
    }

    [TestMethod]
    public void ValidateEditor_WithInvalidComponentRows_FailsRowLevelValidation()
    {
        var page = new ContentTreasure();

        page.TestSetCreateMode();
        page.TestSetEditorFields("TRS901", "Component Error", "Bundle", 100, 1);
        page.TestSetComponentRows(("", "", "Gem", 1, 10));
        page.TestValidateEditorForm();

        Assert.IsFalse(page.TestEditorValidationPassed);
        Assert.AreEqual("Component row 1 requires a Component Id or Name.", page.TestEditorValidationMessage);
    }
}
