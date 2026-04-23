using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.Components.Pages;

namespace MIMLESvtt.Tests;

[TestClass]
public class ContentMonstersPageTests
{
    [TestMethod]
    public void ValidateEditor_WithMissingRequiredFields_Fails()
    {
        var page = new ContentMonsters();

        page.TestSetCreateMode();
        page.TestSetEditorFields(string.Empty, "", "");
        page.TestValidateEditorForm();

        Assert.IsFalse(page.TestEditorValidationPassed);
        Assert.AreEqual("Id is required.", page.TestEditorValidationMessage);
    }

    [TestMethod]
    public void SaveMonster_InCreateMode_WithValidFields_AddsMonsterAndShowsSuccess()
    {
        var page = new ContentMonsters();

        page.TestSetCreateMode();
        page.TestSetEditorFields("OGRE001", "Ogre", "Giantkin");
        page.TestSetOptionalFields(4, 19, 9, 5, "Greatclub +4", "Manual", "ogre,brute", "Large giantkin bruiser");
        page.TestSetExtensions(("AD&D1.THAC0", "17"), ("BRP.STR", "16"));
        page.TestSaveMonster();

        Assert.IsTrue(page.TestEditorSaveSucceeded);
        Assert.AreEqual("Saved new monster and returned to detail/list flow.", page.TestEditorSaveMessage);
        Assert.IsTrue(page.TestMonsterExists("OGRE001", "Ogre"));
        Assert.AreEqual("17", page.TestSelectedMonsterExtensionValue("AD&D1.THAC0"));
    }

    [TestMethod]
    public void SaveMonster_InEditMode_UpdatesSelectedMonsterAndRetainsNavigationPath()
    {
        var page = new ContentMonsters();

        page.TestSelectMonster("GOB001");
        page.TestSetEditorFields("GOB001", "Goblin Veteran", "Humanoid");
        page.TestSetOptionalFields(2, 8, 9, 6, "Scimitar +1", "Manual", "veteran", "Promoted goblin scout");
        page.TestSetExtensions(("AD&D1.THAC0", "18"));
        page.TestSaveMonster();

        Assert.IsTrue(page.TestEditorSaveSucceeded);
        Assert.AreEqual("Saved monster changes and returned to detail/list flow.", page.TestEditorSaveMessage);
        Assert.IsTrue(page.TestMonsterExists("GOB001", "Goblin Veteran"));
        Assert.AreEqual("Goblin Veteran", page.TestSelectedMonsterName);
        Assert.AreEqual("18", page.TestSelectedMonsterExtensionValue("AD&D1.THAC0"));
    }

    [TestMethod]
    public void CancelEdit_InEditMode_RevertsEditorValuesToSelectedMonster()
    {
        var page = new ContentMonsters();

        page.TestSelectMonster("ORC001");
        page.TestSetEditorFields("ORC001", "Temp Name", "Temp Category");
        page.TestSetOptionalFields(99, 99, 99, 99, "Temp", "Temp", "Temp", "Temp");
        page.TestSetExtensions(("Temp", "Value"));
        page.TestCancelEdit();

        Assert.IsTrue(page.TestEditorSaveSucceeded);
        Assert.AreEqual("Canceled edit changes. No mutation persisted; returned to detail/list flow.", page.TestEditorSaveMessage);
        Assert.AreEqual("Orc", page.TestSelectedMonsterName);
    }

    [TestMethod]
    public void SaveMonster_InCreateMode_WithDuplicateId_IsRejected()
    {
        var page = new ContentMonsters();

        page.TestSetCreateMode();
        page.TestSetEditorFields("GOB001", "Duplicate Goblin", "Humanoid");
        page.TestSetOptionalFields(1, 5, 9, 6, "Scimitar", "Manual", "dup", "duplicate test");
        page.TestSaveMonster();

        Assert.IsFalse(page.TestEditorSaveSucceeded);
        Assert.AreEqual("Save failed. Monster id already exists.", page.TestEditorSaveMessage);
    }

    [TestMethod]
    public void SaveMonster_InEditMode_ChangingIdToDuplicate_IsRejectedAndOriginalSelectionRetained()
    {
        var page = new ContentMonsters();

        page.TestSelectMonster("ORC001");
        page.TestSetEditorFields("GOB001", "Orc Renamed", "Humanoid");
        page.TestSetOptionalFields(2, 8, 9, 5, "Axe", "Manual", "orc", "duplicate id test");
        page.TestSaveMonster();

        Assert.IsFalse(page.TestEditorSaveSucceeded);
        Assert.AreEqual("Save failed. Monster id already exists.", page.TestEditorSaveMessage);
        Assert.AreEqual("ORC001", page.TestSelectedMonsterId);
    }

    [TestMethod]
    public void ValidateEditor_WithExtensionValueButMissingKey_Fails()
    {
        var page = new ContentMonsters();

        page.TestSetCreateMode();
        page.TestSetEditorFields("NEW001", "New Monster", "Humanoid");
        page.TestSetOptionalFields(1, 5, 9, 6, "Attack", "Manual", "tag", "desc");
        page.TestSetExtensions(("", "value-without-key"));
        page.TestValidateEditorForm();

        Assert.IsFalse(page.TestEditorValidationPassed);
        Assert.AreEqual("Extension row 1 requires a non-empty key.", page.TestEditorValidationMessage);
    }
}
