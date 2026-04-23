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
        page.TestSetEditorFields("MI900", "Bad Wand", "Wand", "Rare", false, -1);
        page.TestValidateEditorForm();

        Assert.IsFalse(page.TestEditorValidationPassed);
        Assert.AreEqual("Charges must be 0 or greater when provided.", page.TestEditorValidationMessage);
    }

    [TestMethod]
    public void SaveMagicItem_InCreateMode_WithValidFields_AddsMagicItemAndShowsSuccess()
    {
        var page = new ContentMagicItems();

        page.TestSetCreateMode();
        page.TestSetEditorFields("MI900", "Amulet of Dawn", "Amulet", "Rare", true, 3);
        page.TestSetOptionalFields("Radiant burst once per dawn", "Manual", "amulet,radiant");
        page.TestSetEffectMetadataRows(("Activated", "RadiantBurst", "1/day"), ("Duration", "BurstDuration", "1 round"));
        page.TestSaveMagicItem();

        Assert.IsTrue(page.TestEditorSaveSucceeded);
        Assert.AreEqual("Saved new magic item and returned to detail/list flow.", page.TestEditorSaveMessage);
        Assert.IsTrue(page.TestMagicItemExists("MI900", "Amulet of Dawn"));
        Assert.AreEqual(2, page.TestSelectedMagicItemEffectCount);
        Assert.AreEqual("1/day", page.TestSelectedMagicItemEffectValue("RadiantBurst"));
    }

    [TestMethod]
    public void SaveMagicItem_InEditMode_UpdatesSelectedMagicItemAndRetainsRoundTripSelection()
    {
        var page = new ContentMagicItems();

        page.TestSelectMagicItem("MI001");
        page.TestSetEditorFields("MI001", "Ring of Greater Protection", "Ring", "Very Rare", true, null);
        page.TestSetOptionalFields("Stronger passive defense bonus", "Manual", "ring,defense");
        page.TestSetEffectMetadataRows(("Passive", "DefenseBonus", "+2"));
        page.TestSaveMagicItem();

        Assert.IsTrue(page.TestEditorSaveSucceeded);
        Assert.AreEqual("Saved magic item changes and returned to detail/list flow.", page.TestEditorSaveMessage);
        Assert.IsTrue(page.TestMagicItemExists("MI001", "Ring of Greater Protection"));
        Assert.AreEqual("Ring of Greater Protection", page.TestSelectedMagicItemName);
        Assert.AreEqual(1, page.TestSelectedMagicItemEffectCount);
        Assert.AreEqual("+2", page.TestSelectedMagicItemEffectValue("DefenseBonus"));
    }

    [TestMethod]
    public void CancelEdit_InEditMode_RevertsEditorValuesToSelectedMagicItem()
    {
        var page = new ContentMagicItems();

        page.TestSelectMagicItem("MI002");
        page.TestSetEditorFields("MI002", "Temp Name", "Temp", "Temp", false, 1);
        page.TestSetOptionalFields("Temp", "Temp", "Temp");
        page.TestSetEffectMetadataRows(("Temp", "Temp", "Temp"));
        page.TestCancelEdit();

        Assert.IsTrue(page.TestEditorSaveSucceeded);
        Assert.AreEqual("Canceled edit changes. No mutation persisted; returned to detail/list flow.", page.TestEditorSaveMessage);
        Assert.AreEqual("Wand of Fireballs", page.TestSelectedMagicItemName);
    }

    [TestMethod]
    public void SaveMagicItem_InCreateMode_WithDuplicateId_IsRejected()
    {
        var page = new ContentMagicItems();

        page.TestSetCreateMode();
        page.TestSetEditorFields("MI001", "Duplicate Ring", "Ring", "Rare", false, null);
        page.TestSaveMagicItem();

        Assert.IsFalse(page.TestEditorSaveSucceeded);
        Assert.AreEqual("Save failed. Magic item id already exists.", page.TestEditorSaveMessage);
    }

    [TestMethod]
    public void SaveMagicItem_InEditMode_ChangingIdToDuplicate_IsRejectedAndSelectionRetained()
    {
        var page = new ContentMagicItems();

        page.TestSelectMagicItem("MI002");
        page.TestSetEditorFields("MI001", "Wand Duplicate", "Wand", "Very Rare", false, 7);
        page.TestSaveMagicItem();

        Assert.IsFalse(page.TestEditorSaveSucceeded);
        Assert.AreEqual("Save failed. Magic item id already exists.", page.TestEditorSaveMessage);
        Assert.AreEqual("MI002", page.TestSelectedMagicItemId);
    }

    [TestMethod]
    public void ValidateEditor_WithEffectMetadataValueButMissingKey_Fails()
    {
        var page = new ContentMagicItems();

        page.TestSetCreateMode();
        page.TestSetEditorFields("MI901", "Effect Error", "Wondrous", "Rare", false, null);
        page.TestSetEffectMetadataRows(("Passive", "", "missing-key"));
        page.TestValidateEditorForm();

        Assert.IsFalse(page.TestEditorValidationPassed);
        Assert.AreEqual("Effect metadata row 1 requires a non-empty key.", page.TestEditorValidationMessage);
    }
}
