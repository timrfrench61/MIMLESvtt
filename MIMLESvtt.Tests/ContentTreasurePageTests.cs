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
        page.TestSaveTreasure();

        Assert.IsTrue(page.TestEditorSaveSucceeded);
        Assert.AreEqual("Saved new treasure.", page.TestEditorSaveMessage);
        Assert.IsTrue(page.TestTreasureExists("TRS900", "Royal Cache"));
    }

    [TestMethod]
    public void SaveTreasure_InEditMode_UpdatesSelectedTreasureAndRetainsRoundTripSelection()
    {
        var page = new ContentTreasure();

        page.TestSelectTreasure("TRS001");
        page.TestSetEditorFields("TRS001", "Goblin Coin Hoard", "Coin", 40, 2);
        page.TestSaveTreasure();

        Assert.IsTrue(page.TestEditorSaveSucceeded);
        Assert.AreEqual("Saved treasure changes.", page.TestEditorSaveMessage);
        Assert.IsTrue(page.TestTreasureExists("TRS001", "Goblin Coin Hoard"));
        Assert.AreEqual("Goblin Coin Hoard", page.TestSelectedTreasureName);
    }

    [TestMethod]
    public void CancelEdit_InEditMode_RevertsEditorValuesToSelectedTreasure()
    {
        var page = new ContentTreasure();

        page.TestSelectTreasure("TRS002");
        page.TestSetEditorFields("TRS002", "Temp Treasure", "Temp", 1, 1);
        page.TestCancelEdit();

        Assert.IsTrue(page.TestEditorSaveSucceeded);
        Assert.AreEqual("Canceled edit changes.", page.TestEditorSaveMessage);
        Assert.AreEqual("Small Gem Cache", page.TestSelectedTreasureName);
    }
}
