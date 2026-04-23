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
        page.TestSaveMonster();

        Assert.IsTrue(page.TestEditorSaveSucceeded);
        Assert.AreEqual("Saved new monster.", page.TestEditorSaveMessage);
        Assert.IsTrue(page.TestMonsterExists("OGRE001", "Ogre"));
    }

    [TestMethod]
    public void SaveMonster_InEditMode_UpdatesSelectedMonsterAndRetainsNavigationPath()
    {
        var page = new ContentMonsters();

        page.TestSelectMonster("GOB001");
        page.TestSetEditorFields("GOB001", "Goblin Veteran", "Humanoid");
        page.TestSaveMonster();

        Assert.IsTrue(page.TestEditorSaveSucceeded);
        Assert.AreEqual("Saved monster changes.", page.TestEditorSaveMessage);
        Assert.IsTrue(page.TestMonsterExists("GOB001", "Goblin Veteran"));
        Assert.AreEqual("Goblin Veteran", page.TestSelectedMonsterName);
    }

    [TestMethod]
    public void CancelEdit_InEditMode_RevertsEditorValuesToSelectedMonster()
    {
        var page = new ContentMonsters();

        page.TestSelectMonster("ORC001");
        page.TestSetEditorFields("ORC001", "Temp Name", "Temp Category");
        page.TestCancelEdit();

        Assert.IsTrue(page.TestEditorSaveSucceeded);
        Assert.AreEqual("Canceled edit changes.", page.TestEditorSaveMessage);
        Assert.AreEqual("Orc", page.TestSelectedMonsterName);
    }
}
