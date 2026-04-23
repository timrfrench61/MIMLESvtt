using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.Components.Pages;

namespace MIMLESvtt.Tests;

[TestClass]
public class ContentMagicItemsPageTests
{
    [TestMethod]
    public void ValidateEditor_WithNegativeCharges_Fails()
    {
        var page = new ContentMagicItems();

        page.TestSetCreateMode();
        page.TestSetEditorFields("MI900", "Bad Wand", "Wand", "Rare", false, -1, "Invalid charges test");
        page.TestValidateEditorForm();

        Assert.IsFalse(page.TestEditorValidationPassed);
        Assert.AreEqual("Charges must be 0 or greater when provided.", page.TestEditorValidationMessage);
    }

    [TestMethod]
    public void SaveMagicItem_InCreateMode_WithValidFields_AddsMagicItemAndShowsSuccess()
    {
        var page = new ContentMagicItems();

        page.TestSetCreateMode();
        page.TestSetEditorFields("MI900", "Amulet of Dawn", "Amulet", "Rare", true, 3, "Radiant burst once per dawn");
        page.TestSaveMagicItem();

        Assert.IsTrue(page.TestEditorSaveSucceeded);
        Assert.AreEqual("Saved new magic item.", page.TestEditorSaveMessage);
        Assert.IsTrue(page.TestMagicItemExists("MI900", "Amulet of Dawn"));
    }

    [TestMethod]
    public void SaveMagicItem_InEditMode_UpdatesSelectedMagicItemAndRetainsRoundTripSelection()
    {
        var page = new ContentMagicItems();

        page.TestSelectMagicItem("MI001");
        page.TestSetEditorFields("MI001", "Ring of Greater Protection", "Ring", "Very Rare", true, null, "Stronger passive defense bonus");
        page.TestSaveMagicItem();

        Assert.IsTrue(page.TestEditorSaveSucceeded);
        Assert.AreEqual("Saved magic item changes.", page.TestEditorSaveMessage);
        Assert.IsTrue(page.TestMagicItemExists("MI001", "Ring of Greater Protection"));
        Assert.AreEqual("Ring of Greater Protection", page.TestSelectedMagicItemName);
    }

    [TestMethod]
    public void CancelEdit_InEditMode_RevertsEditorValuesToSelectedMagicItem()
    {
        var page = new ContentMagicItems();

        page.TestSelectMagicItem("MI002");
        page.TestSetEditorFields("MI002", "Temp Name", "Temp", "Temp", false, 1, "Temp");
        page.TestCancelEdit();

        Assert.IsTrue(page.TestEditorSaveSucceeded);
        Assert.AreEqual("Canceled edit changes.", page.TestEditorSaveMessage);
        Assert.AreEqual("Wand of Fireballs", page.TestSelectedMagicItemName);
    }
}
